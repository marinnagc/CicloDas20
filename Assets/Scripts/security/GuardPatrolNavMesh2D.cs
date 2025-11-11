using UnityEngine;
using UnityEngine.AI;

public class GuardPatrolNavMesh2D : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;

    [Header("Configurações de Patrulha")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolRadius = 10f; // Raio para gerar pontos aleatórios
    [SerializeField] private float waitTimeAtPoint = 2f;

    [Header("Configurações de Perseguição")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float visionRange = 8f; // Distância que o guarda enxerga
    [SerializeField] private float visionAngle = 90f; // Ângulo de visão (em graus)
    [SerializeField] private LayerMask obstacleLayer; // Camada de obstáculos (paredes)

    [Header("Horários")]
    [SerializeField] private int horaInicioPatrulha = 6;
    [SerializeField] private int horaFimPatrulha = 20;
    [SerializeField] private int horaInicioPerseguicao = 20;
    [SerializeField] private int horaFimPerseguicao = 22;

    // Variáveis privadas
    private NavMeshAgent agent;
    private float waitTimer = 0f;
    private bool isChasing = false;
    private Vector3 patrolCenter;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Guarda a posição inicial como centro da patrulha
        patrolCenter = transform.position;
    }

    void Start()
    {
        // Começa patrulhando
        GoToRandomPatrolPoint();
    }

    void Update()
    {
        if (TimerController.Instance == null || player == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();

        // Das 20h às 22h: modo perseguição se vir o player
        if (horaAtual >= horaInicioPerseguicao && horaAtual < horaFimPerseguicao)
        {
            if (CanSeePlayer())
            {
                ChasePlayer();
            }
            else if (isChasing)
            {
                // Se estava perseguindo mas perdeu de vista, volta a patrulhar
                isChasing = false;
                agent.speed = patrolSpeed;
                GoToRandomPatrolPoint();
            }
            else
            {
                // Patrulha normal mesmo no período noturno se não vir o player
                PatrolBehavior();
            }
        }
        // Das 6h às 20h: apenas patrulha
        else if (horaAtual >= horaInicioPatrulha && horaAtual < horaFimPatrulha)
        {
            if (isChasing)
            {
                isChasing = false;
                agent.speed = patrolSpeed;
            }
            PatrolBehavior();
        }
    }

    void PatrolBehavior()
    {
        // Verifica se chegou no destino
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (waitTimer <= 0f)
            {
                GoToRandomPatrolPoint();
                waitTimer = waitTimeAtPoint;
            }
            else
            {
                waitTimer -= Time.deltaTime;
            }
        }
    }

    void GoToRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitCircle * patrolRadius;
        Vector3 randomPoint = patrolCenter + new Vector3(randomDirection.x, randomDirection.y, 0);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.speed = patrolSpeed;
            agent.SetDestination(hit.position);
        }
    }

    void ChasePlayer()
    {
        isChasing = true;
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

    // Método para visualizar o campo de visão no Editor (Debug)
    void OnDrawGizmosSelected()
    {
        if (TimerController.Instance == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();

        // Só desenha o cone de visão no período de perseguição
        if (horaAtual >= horaInicioPerseguicao && horaAtual < horaFimPerseguicao)
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
        }
        else
        {
            // Desenha o raio de patrulha
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(patrolCenter, patrolRadius);
        }
    }
}