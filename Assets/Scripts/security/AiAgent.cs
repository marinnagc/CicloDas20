using UnityEngine;

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

    // Variáveis privadas
    private float targetX;
    private float waitTimer = 0f;
    private bool isChasing = false;
    private Rigidbody2D rb2d;
    private Animator animator;
    private float currentSpeed = 0f;

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
        // Define Y fixo baseado na posição inicial se não configurado
        if (fixedY == 0)
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
            StayIdle();
        }
        // Das 20h às 22h: RONDA ou PERSEGUE
        else if (horaAtual >= horaIniciarRonda && horaAtual < horaFimRonda)
        {
            if (CanSeePlayer())
            {
                ChasePlayer();
            }
            else if (isChasing)
            {
                // Perdeu o player, volta a patrulhar
                isChasing = false;
                ChooseNewTarget();
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            StayIdle();
        }

        // Atualiza animação
        UpdateAnimator();
    }

    void LateUpdate()
    {
        // Força posição no chão e dentro dos limites
        Vector3 pos = transform.position;
        pos.y = fixedY;
        pos.z = 0;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Mantém rotação em 0 (sempre em pé)
        transform.rotation = Quaternion.identity;
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
            Debug.Log("[SecurityGuard] Descongelado para patrulha");
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

        // Move em direção ao target
        MoveTowardsTarget(targetX, patrolSpeed);
    }

    void ChasePlayer()
    {
        isChasing = true;

        // Descongela para permitir movimento
        if (rb2d != null && rb2d.constraints == RigidbodyConstraints2D.FreezeAll)
        {
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Move em direção ao player (só no eixo X)
        MoveTowardsTarget(player.position.x, chaseSpeed);
    }

    void MoveTowardsTarget(float targetXPos, float speed)
    {
        Vector3 pos = transform.position;

        // Calcula direção (esquerda ou direita)
        float direction = Mathf.Sign(targetXPos - pos.x);

        // Move
        pos.x += direction * speed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = fixedY;
        pos.z = 0;

        transform.position = pos;
        currentSpeed = speed;

        // Vira o sprite para a direção do movimento (opcional)
        if (direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Olhando direita
        }
        else if (direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Olhando esquerda
        }
    }

    void ChooseNewTarget()
    {
        // Escolhe uma nova posição X aleatória
        targetX = Random.Range(minX, maxX);
        Debug.Log($"[SecurityGuard] Novo destino: X = {targetX:F2}");
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Verifica distância
        if (distanceToPlayer > visionRange)
            return false;

        // Verifica ângulo
        Vector3 forward = transform.right; // Assuming sprite faces right by default
        float angle = Vector3.Angle(forward, directionToPlayer);

        if (angle > visionAngle / 2f)
            return false;

        // Verifica obstáculos
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer);

        if (hit.collider != null && hit.collider.transform != player)
            return false;

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
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public bool IsChasing()
    {
        return isChasing;
    }

    public float GetVisionRange()
    {
        return visionRange;
    }

    public float GetVisionAngle()
    {
        return visionAngle;
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