using UnityEngine;
using UnityEngine.SceneManagement;

public class AiAgente : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;

    [Header("Limites de Movimento")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float fixedY = -49f; // Altura fixa do chão

    [Header("Configurações de Patrulha")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitTimeAtPoint = 2f;

    [Header("Configurações de Perseguição")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float visionRange = 8f;
    [SerializeField] private float visionAngle = 90f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Horários")]
    [SerializeField] private int horaFicarParado = 6;
    [SerializeField] private int horaIniciarRonda = 20;
    [SerializeField] private int horaFimRonda = 22;

    [Header("Captura")]
    [SerializeField] private float captureDistance = 0.6f;   // distância para "pegou"
    private bool hasCaughtThisNight = false;                 // evita capturar várias vezes

    // Variáveis privadas
    private enum State { Idle, Patrolling, Chasing, Returning }
    private State state = State.Patrolling;

    private float targetX;
    private float waitTimer = 0f;
    private bool isChasing = false;
    private Rigidbody2D rb2d;
    private Animator animator;
    private float currentSpeed = 0f;
    private float lastPatrolTargetX; // Salva o último destino de patrulha
    private Vector3 startPosition;
    private Vector3 returnTarget;

    public Transform GetPlayer()
    {
        return player;
    }

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb2d == null)
        {
            rb2d = gameObject.AddComponent<Rigidbody2D>();
        }

        rb2d.bodyType = RigidbodyType2D.Kinematic;
        rb2d.gravityScale = 0;
    }

    void Start()
    {
        // Guarda posição inicial (usada para retornar)
        startPosition = transform.position;
        startPosition.z = 0f;

        // Define Y fixo baseado na posição inicial se não configurado
        if (Mathf.Approximately(fixedY, 0f))
        {
            fixedY = transform.position.y;
        }

        // Define posição inicial no chão
        Vector3 pos = transform.position;
        pos.y = fixedY;
        pos.z = 0;
        transform.position = pos;

        // Escolhe primeiro destino aleatório
        ChooseNewTarget();

        Debug.Log($"[SecurityGuard] Iniciado. Y fixo: {fixedY} | Limites X: {minX} a {maxX}");
    }

    void Update()
    {
        if (TimerController.Instance == null || player == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();

        // Das 6h às 20h: PARADO
        if (horaAtual >= horaFicarParado && horaAtual < horaIniciarRonda)
        {
            SetState(State.Idle);
        }
        // Das 20h às 22h: RONDA ou PERSEGUE
        else if (horaAtual >= horaIniciarRonda && horaAtual < horaFimRonda)
        {
            if (CanSeePlayer())
            {
                SetState(State.Chasing);
            }
            else
            {
                if (state == State.Chasing)
                {
                    // Perdeu o player de vista - VOLTA PARA O ÚLTIMO DESTINO
                    StartReturningToLastPatrol();
                }
                else if (state == State.Returning)
                {
                    // continua retornando
                }
                else
                {
                    SetState(State.Patrolling);
                }
            }
        }
        else
        {
            SetState(State.Idle);
        }

        // Executa lógica do estado
        switch (state)
        {
            case State.Idle: StayIdle(); break;
            case State.Patrolling: Patrol(); break;
            case State.Chasing: ChasePlayer(); break;
            case State.Returning: ReturnUpdate(); break;
        }

        // Atualiza animação
        UpdateAnimator();
    }

    void LateUpdate()
    {
        // IMPORTANTE: não forçar Y quando estiver perseguindo
        Vector3 pos = transform.position;

        if (state != State.Chasing)
        {
            // quando NÃO está em chase, fixa Y e mantém X dentro dos limites
            pos.y = fixedY;
        }
        // sempre mantém Z = 0 e limita X
        pos.z = 0f;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Mantém rotação em 0 (sempre em pé)
        transform.rotation = Quaternion.identity;
    }

    void SetState(State newState)
    {
        state = newState;
    }

    void StayIdle()
    {
        currentSpeed = 0f;

        // Congela completamente
        if (rb2d != null)
        {
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            rb2d.linearVelocity = Vector2.zero;
        }

        isChasing = false;
    }

    void Patrol()
    {
        // Descongela para permitir movimento
        if (rb2d != null && rb2d.constraints == RigidbodyConstraints2D.FreezeAll)
        {
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        float distanceToTarget = Mathf.Abs(transform.position.x - targetX);

        // Se chegou no destino
        if (distanceToTarget < 0.3f)
        {
            if (waitTimer <= 0f)
            {
                ChooseNewTarget();
                waitTimer = waitTimeAtPoint;
            }
            else
            {
                waitTimer -= Time.deltaTime;
                currentSpeed = 0f;
                return;
            }
        }

        // Move em direção ao target (apenas X enquanto patrulha; Y mantido)
        Vector3 patrolTarget = new Vector3(targetX, fixedY, 0f);
        MoveTowardsTarget(patrolTarget, patrolSpeed);
    }

    void ChasePlayer()
    {
        isChasing = true;

        // Descongela para permitir movimento
        if (rb2d != null && rb2d.constraints == RigidbodyConstraints2D.FreezeAll)
        {
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Perseguir usando X e Y do player (Y deixa de ser fixo)
        Vector3 playerTarget = new Vector3(player.position.x, player.position.y, 0f);
        MoveTowardsTarget(playerTarget, chaseSpeed);

        // 🔥 Verifica se "pegou" o player (encostou) durante a perseguição
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= captureDistance)
        {
            TryCatchPlayer();
        }
    }

    void StartReturningToLastPatrol()
    {
        isChasing = false;
        SetState(State.Returning);
        // volta para último destino de patrulha salvo (X) mantendo Y fixa
        returnTarget = new Vector3(lastPatrolTargetX, fixedY, 0f);
    }

    void ReturnUpdate()
    {
        MoveTowardsTarget(returnTarget, patrolSpeed);

        // Se chegou, reinicia patrulha
        float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.y),
                                      new Vector2(returnTarget.x, returnTarget.y));
        if (dist <= 0.35f)
        {
            ChooseNewTarget();
            SetState(State.Patrolling);
            Debug.Log("[SecurityGuard] Chegou ao destino de retorno — reiniciando patrulha.");
        }
    }

    // agora MoveTowardsTarget aceita Vector3 target (X e Y)
    void MoveTowardsTarget(Vector3 targetPos, float speed)
    {
        Vector3 pos = transform.position;
        Vector3 next = Vector3.MoveTowards(pos, targetPos, speed * Time.deltaTime);

        // Aplicar limites X
        next.x = Mathf.Clamp(next.x, minX, maxX);

        // Z sempre 0
        next.z = 0f;

        transform.position = next;

        // calcula currentSpeed como distância percorrida / deltaTime
        currentSpeed = (Vector3.Distance(next, pos) / (Mathf.Epsilon + Time.deltaTime));

        // Vira o sprite segundo movimento horizontal (opcional)
        float dx = next.x - pos.x;
        if (dx > 0.001f) transform.localScale = new Vector3(1, 1, 1);
        else if (dx < -0.001f) transform.localScale = new Vector3(-1, 1, 1);
    }

    void ChooseNewTarget()
    {
        // Escolhe uma nova posição X aleatória
        targetX = Random.Range(minX, maxX);
        lastPatrolTargetX = targetX; // Salva para caso precise voltar depois
        Debug.Log($"[SecurityGuard] Novo destino: X = {targetX:F2}");
    }

    bool CanSeePlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("[SecurityGuard] Player é NULL!");
            return false;
        }

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        Debug.Log($"[SecurityGuard] Distância até player: {distanceToPlayer:F2} | Range: {visionRange}");

        // 1) Verifica distância máxima de visão
        if (distanceToPlayer > visionRange)
        {
            Debug.Log("[SecurityGuard] Player fora do alcance!");
            return false;
        }

        // 2) Direção baseada no localScale.x (direita ou esquerda)
        Vector3 forward = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Debug.Log($"[SecurityGuard] Forward: {forward} | LocalScale.x: {transform.localScale.x}");

        float angle = Vector3.Angle(forward, directionToPlayer);
        Debug.Log($"[SecurityGuard] Ângulo até player: {angle:F2}° | Max permitido: {visionAngle / 2f}°");

        if (angle > visionAngle / 2f)
        {
            Debug.Log("[SecurityGuard] Player fora do ângulo de visão!");
            return false;
        }

        // 3) Verifica obstáculos entre guarda e player
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToPlayer.normalized,
            distanceToPlayer,
            obstacleLayer
        );

        if (hit.collider != null)
        {
            Debug.Log($"[SecurityGuard] Raycast atingiu: {hit.collider.name} | É o player? {hit.collider.transform == player}");

            if (hit.collider.transform != player)
            {
                Debug.Log("[SecurityGuard] Obstáculo bloqueando visão!");
                return false;
            }
        }
        else
        {
            Debug.Log("[SecurityGuard] Raycast limpo - PLAYER VISÍVEL!");
        }

        Debug.Log("[SecurityGuard] ✓ PLAYER DETECTADO!");

        // 🔥 4) Se estiver na ronda e praticamente colado no player → CAPTURA
        if (TimerController.Instance != null)
        {
            int hora = TimerController.Instance.GetHoraInteira();

            // Apenas das 20h às 22h
            if (hora >= horaIniciarRonda && hora < horaFimRonda)
            {
                // Aqui entra a regra que você pediu:
                // se distância ~ 0 OU ângulo ~ 0, considera que pegou
                if (distanceToPlayer <= 0.05f || angle <= 1f)
                {
                    Debug.Log($"[SecurityGuard] Dist {distanceToPlayer:F3} | Ângulo {angle:F2}° → CAPTURAR PLAYER");
                    TryCatchPlayer();
                }
            }
        }

        return true;
    }


    void UpdateAnimator()
    {
        if (animator == null) return;

        // Atualiza parâmetros do animator
        foreach (var param in animator.parameters)
        {
            if (param.name == "Speed" && param.type == AnimatorControllerParameterType.Float)
            {
                animator.SetFloat("Speed", currentSpeed);
            }
            else if (param.name == "IsMoving" && param.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool("IsMoving", currentSpeed > 0.1f);
            }
            else if (param.name == "IsWalking" && param.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool("IsWalking", currentSpeed > 0.1f);
            }
            else if (param.name == "Velocity" && param.type == AnimatorControllerParameterType.Float)
            {
                animator.SetFloat("Velocity", currentSpeed);
            }
        }
    }

    // Métodos públicos para a lanterna acessar
    public float GetCurrentSpeed() { return currentSpeed; }
    public bool IsChasing() { return isChasing; }
    public float GetVisionRange() { return visionRange; }
    public float GetVisionAngle() { return visionAngle; }

    // 🔥 Aqui é onde realmente "pega" o player e manda pro próximo dia
    void TryCatchPlayer()
    {
        if (hasCaughtThisNight) return;

        // Verifica horário — só entre 20h e < 22h
        if (TimerController.Instance != null)
        {
            int hora = TimerController.Instance.GetHoraInteira();

            if (hora < horaIniciarRonda || hora >= horaFimRonda)
            {
                Debug.Log("[SecurityGuard] Tentou capturar fora do horário. Ignorando.");
                return;
            }
        }
        else
        {
            Debug.LogWarning("[SecurityGuard] TimerController == null ao capturar!");
            return;
        }

        hasCaughtThisNight = true;

        Debug.Log("[SecurityGuard] PLAYER CAPTURADO ENTRE 20h E 22h → GAME OVER!");

        // 🔥 Vai direto para a tela de derrota
        SceneManager.LoadScene("PerdeuGO");
    }




    // Visualização no Editor
    void OnDrawGizmosSelected()
    {
        if (TimerController.Instance == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();
        float yPos = fixedY == 0 ? transform.position.y : fixedY;

        if (horaAtual >= horaIniciarRonda && horaAtual < horaFimRonda)
        {
            // Cone de visão
            Gizmos.color = isChasing ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, visionRange);

            // Limites de patrulha
            Gizmos.color = Color.green;
            Vector3 leftLimit = new Vector3(minX, yPos, 0);
            Vector3 rightLimit = new Vector3(maxX, yPos, 0);
            Gizmos.DrawLine(leftLimit + Vector3.up * 2, leftLimit - Vector3.up * 2);
            Gizmos.DrawLine(rightLimit + Vector3.up * 2, rightLimit - Vector3.up * 2);
            Gizmos.DrawLine(leftLimit, rightLimit);

            // Destino atual
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(new Vector3(targetX, yPos, 0), 0.3f);
        }
        else
        {
            // Parado
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            // Limites
            Gizmos.color = Color.cyan;
            Vector3 leftLimit = new Vector3(minX, yPos, 0);
            Vector3 rightLimit = new Vector3(maxX, yPos, 0);
            Gizmos.DrawLine(leftLimit, rightLimit);
        }
    }
}
