using UnityEngine;
using UnityEngine.AI;

public class GuardPatrolNavMesh2D : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] waypoints; // Array de coordenadas para patrulha

    [Header("Configurações de Patrulha")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitTimeAtPoint = 2f;

    [Header("Configurações de Perseguição")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float visionRange = 8f;
    [SerializeField] private float visionAngle = 90f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Horários")]
    [SerializeField] private int horaFicarParado = 6; // Hora que fica parado
    [SerializeField] private int horaIniciarRonda = 20; // Hora que começa a ronda
    [SerializeField] private int horaFimRonda = 22; // Hora que termina

    // runtime
    private NavMeshAgent agent;
    private float waitTimer = 0f;
    private bool isChasing = false;
    private int currentWaypointIndex = 0;
    private Vector3 posicaoInicial;
    private enum State { Idle, Patrolling, Chasing, Returning }
    private State state = State.Patrolling;
    private Vector3 returnTarget;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // IMPORTANTE para 2D: desabilita física 3D
        agent.updatePosition = true; // Deixa o NavMesh controlar a posição

        // Remove ou desabilita Rigidbody2D se existir
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            // Opção 1: Tornar Kinematic
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            rb2d.simulated = false; // Desabilita completamente a física 2D
        }

        // Guarda a posição inicial
        posicaoInicial = transform.position;
        posicaoInicial.z = 0f;
    }

    void Start()
    {
        // Começa parado na posição inicial
        agent.speed = 0;
        agent.isStopped = true;

        Debug.Log($"[Guard] Iniciado. Waypoints: {(waypoints != null ? waypoints.Length : 0)}");

        // Verifica se o NavMesh foi gerado
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
        {
            Debug.Log("[Guard] NavMesh encontrado!");
        }
        else
        {
            Debug.LogError("[Guard] ERRO: NavMesh não encontrado! Certifique-se de fazer Bake no NavMeshSurface!");
        }
    }

    void Update()
    {
        if (TimerController.Instance == null || player == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();

        // Das 6h às 20h: FICA PARADO
        if (horaAtual >= horaFicarParado && horaAtual < horaIniciarRonda)
        {
            SetState(State.Idle);
        }
        // Das 20h às 22h: faz ronda OU persegue se ver o player
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
                    // Perdeu de vista -> retornar à posição inicial e reiniciar ronda
                    Debug.Log("[Guard] Perdeu o player de vista. Retornando para posição inicial.");
                    StartReturningToStart();
                }
                else if (state == State.Returning)
                {
                    // continuará no Update (ReturnUpdate chamado abaixo)
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

        // Executa comportamento atual
        switch (state)
        {
            case State.Idle:
                StayIdle();
                break;
            case State.Patrolling:
                PatrolBehavior();
                break;
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Returning:
                ReturnUpdate();
                break;
        }
    }

    void LateUpdate()
    {
        // Mantém Z = 0
        if (transform.position.z != 0f)
        {
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }
    }

    void SetState(State newState)
    {
        if (state == newState) return;
        state = newState;
    }

    void StayIdle()
    {
        if (!agent.isStopped)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        isChasing = false;
    }

    void PatrolBehavior()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            StayIdle();
            return;
        }

        if (agent.isStopped)
        {
            agent.isStopped = false;
            GoToNextWaypoint();
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (waitTimer <= 0f)
            {
                GoToNextWaypoint();
                waitTimer = waitTimeAtPoint;
            }
            else
            {
                waitTimer -= Time.deltaTime;
            }
        }
    }

    void GoToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // Vai para o próximo waypoint de forma aleatória
        int randomIndex = Random.Range(0, waypoints.Length);
        currentWaypointIndex = randomIndex;

        agent.speed = patrolSpeed;

        // Verifica se o waypoint está no NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(waypoints[currentWaypointIndex].position, out hit, 2f, NavMesh.AllAreas))
        {
            Vector3 dest = hit.position;
            dest.z = 0f;
            agent.SetDestination(dest);
            Debug.Log($"[Guard] Indo para waypoint {currentWaypointIndex}: {dest}");
        }
        else
        {
            Debug.LogWarning($"[Guard] Waypoint {currentWaypointIndex} não está no NavMesh!");
        }
    }

    void ChasePlayer()
    {
        isChasing = true;
        agent.isStopped = false;
        agent.speed = chaseSpeed;

        // Envia destino usando X e Y do player (Z = 0)
        Vector3 target = new Vector3(player.position.x, player.position.y, 0f);
        agent.SetDestination(target);
    }

    void StartReturningToStart()
    {
        isChasing = false;
        SetState(State.Returning);
        returnTarget = posicaoInicial;
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.SetDestination(returnTarget);
    }

    void ReturnUpdate()
    {
        // Se alcançou returnTarget
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Chegou ao inicial -> voltar a patrulhar
            Debug.Log("[Guard] Chegou à posição inicial. Reiniciando ronda.");
            SetState(State.Patrolling);
            GoToNextWaypoint();
        }
    }

    bool CanSeePlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("[Guard] Player é NULL!");
            return false;
        }

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        Debug.Log($"[Guard] Distância até player: {distanceToPlayer:F2} | Range: {visionRange}");

        if (distanceToPlayer > visionRange)
        {
            Debug.Log($"[Guard] Player fora do alcance!");
            return false;
        }

        // Direção baseada no localScale.x
        Vector3 forward = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Debug.Log($"[Guard] Forward direction: {forward} | LocalScale.x: {transform.localScale.x}");

        float angle = Vector3.Angle(forward, directionToPlayer);
        Debug.Log($"[Guard] Ângulo até player: {angle:F2}° | Max permitido: {visionAngle / 2f}°");

        if (angle > visionAngle / 2f)
        {
            Debug.Log($"[Guard] Player fora do ângulo de visão!");
            return false;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer);

        if (hit.collider != null)
        {
            Debug.Log($"[Guard] Raycast atingiu: {hit.collider.name} | É o player? {hit.collider.transform == player}");
        }
        else
        {
            Debug.Log($"[Guard] Raycast não atingiu nada - PLAYER VISÍVEL!");
        }

        if (hit.collider == null)
        {
            // sem colisão no meio -> considera visível
            return true;
        }

        // Se o primeiro collider atingido é o player -> visível, senão bloqueado
        if (hit.collider.transform == player) return true;

        return false;
    }
    void OnDrawGizmosSelected()
    {
        if (TimerController.Instance == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();

        if (horaAtual >= horaIniciarRonda && horaAtual < horaFimRonda)
        {
            Gizmos.color = isChasing ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, visionRange);

            Vector3 forward = transform.up;
            Vector3 leftBoundary = Quaternion.Euler(0, 0, visionAngle / 2f) * forward * visionRange;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -visionAngle / 2f) * forward * visionRange;
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

            if (waypoints != null && waypoints.Length > 0)
            {
                Gizmos.color = Color.cyan;
                foreach (Transform waypoint in waypoints)
                {
                    if (waypoint != null)
                        Gizmos.DrawWireSphere(waypoint.position, 0.3f);
                }
            }
        }
        else
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
