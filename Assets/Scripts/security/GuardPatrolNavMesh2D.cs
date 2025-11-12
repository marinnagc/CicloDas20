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

    // Variáveis privadas
    private NavMeshAgent agent;
    private float waitTimer = 0f;
    private bool isChasing = false;
    private int currentWaypointIndex = 0;
    private Vector3 posicaoInicial;

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

            // Opção 2: Remover (descomente se preferir)
            // Destroy(rb2d);
        }

        // Guarda a posição inicial
        posicaoInicial = transform.position;
    }

    void Start()
    {
        // Começa parado na posição inicial
        agent.speed = 0;
        agent.isStopped = true;

        // Debug
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
            StayIdle();
        }
        // Das 20h às 22h: faz ronda OU persegue se ver o player
        else if (horaAtual >= horaIniciarRonda && horaAtual < horaFimRonda)
        {
            if (CanSeePlayer())
            {
                ChasePlayer();
            }
            else if (isChasing)
            {
                // Perdeu de vista, volta à ronda
                Debug.Log("[Guard] Perdeu o player de vista. Voltando para ronda.");
                isChasing = false;
                agent.speed = patrolSpeed;
                agent.isStopped = false;
                GoToNextWaypoint();
            }
            else
            {
                // Faz a ronda normal
                PatrolBehavior();
            }
        }
        else
        {
            // Fora do horário de trabalho, fica parado
            StayIdle();
        }

        // Debug visual no console (comentar depois de testar)
        /*
        if (Time.frameCount % 60 == 0) // A cada 60 frames
        {
            Debug.Log($"[Guard] Hora: {horaAtual}h | Velocidade: {agent.velocity.magnitude:F2} | isStopped: {agent.isStopped} | Destino: {agent.destination}");
        }
        */
    }

    void LateUpdate()
    {
        // CRÍTICO para 2D: mantém Z sempre em 0
        if (transform.position.z != 0)
        {
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }
    }

    void StayIdle()
    {
        // Para o agente completamente
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

        // Garante que o agente está ativo
        if (agent.isStopped)
        {
            agent.isStopped = false;
            GoToNextWaypoint();
        }

        // Verifica se chegou no waypoint
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
            agent.SetDestination(hit.position);
            Debug.Log($"[Guard] Indo para waypoint {currentWaypointIndex}: {hit.position}");
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
        agent.SetDestination(player.position);
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Verifica se está dentro do alcance de visão
        if (distanceToPlayer > visionRange)
            return false;

        // Verifica o ângulo de visão
        Vector3 forward = transform.up; // Em 2D, geralmente "up" é a frente
        float angle = Vector3.Angle(forward, directionToPlayer);

        if (angle > visionAngle / 2f)
            return false;

        // Verifica se há obstáculos entre o guarda e o player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer);

        if (hit.collider != null)
            return false; // Há um obstáculo bloqueando a visão

        return true;
    }

    // Método para visualizar o campo de visão no Editor
    void OnDrawGizmosSelected()
    {
        if (TimerController.Instance == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();

        // Das 20h às 22h: mostra cone de visão
        if (horaAtual >= horaIniciarRonda && horaAtual < horaFimRonda)
        {
            Gizmos.color = isChasing ? Color.red : Color.yellow;

            // Desenha o alcance de visão
            Gizmos.DrawWireSphere(transform.position, visionRange);

            // Desenha o cone de visão
            Vector3 forward = transform.up;
            Vector3 leftBoundary = Quaternion.Euler(0, 0, visionAngle / 2f) * forward * visionRange;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -visionAngle / 2f) * forward * visionRange;

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

            // Desenha os waypoints
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
            // Das 6h às 20h: mostra que está parado
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}