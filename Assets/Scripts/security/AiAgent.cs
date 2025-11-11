using UnityEngine;
using Pathfinding;

public class AiAgent : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;

    [Header("Configurações de Patrulha")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float waitTimeAtPoint = 2f;
    [SerializeField] private float nextWaypointDistance = 0.5f; // Distância para considerar que chegou no waypoint

    [Header("Configurações de Perseguição")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float visionRange = 8f;
    [SerializeField] private float visionAngle = 90f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Horários")]
    [SerializeField] private int horaInicioPatrulha = 6;
    [SerializeField] private int horaFimPatrulha = 20;
    [SerializeField] private int horaInicioPerseguicao = 20;
    [SerializeField] private int horaFimPerseguicao = 22;

    // Componentes A*
    private AIPath path;
    private Seeker seeker;

    // Variáveis privadas
    private float waitTimer = 0f;
    private bool isChasing = false;
    private Vector3 patrolCenter;
    private Vector3 currentPatrolTarget;

    void Awake()
    {
        path = GetComponent<AIPath>();
        if (path == null)
            path = gameObject.AddComponent<AIPath>();

        seeker = GetComponent<Seeker>();
        if (seeker == null)
            seeker = gameObject.AddComponent<Seeker>();

        patrolCenter = transform.position;

        // Configurações do AIPath
        path.maxSpeed = patrolSpeed;
        path.slowdownDistance = 0.5f;
        path.endReachedDistance = 0.3f;
    }

    void Start()
    {
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
                isChasing = false;
                path.maxSpeed = patrolSpeed;
                GoToRandomPatrolPoint();
            }
            else
            {
                PatrolBehavior();
            }
        }
        // Das 6h às 20h: apenas patrulha
        else if (horaAtual >= horaInicioPatrulha && horaAtual < horaFimPatrulha)
        {
            if (isChasing)
            {
                isChasing = false;
                path.maxSpeed = patrolSpeed;
            }
            PatrolBehavior();
        }
    }

    void PatrolBehavior()
    {
        // Verifica se chegou no destino usando a distância
        float distanceToTarget = Vector3.Distance(transform.position, currentPatrolTarget);

        if (distanceToTarget < nextWaypointDistance)
        {
            if (waitTimer <= 0f)
            {
                GoToRandomPatrolPoint();
                waitTimer = waitTimeAtPoint;
            }
            else
            {
                waitTimer -= Time.deltaTime;
                path.maxSpeed = 0f; // Para enquanto espera
            }
        }
        else
        {
            path.maxSpeed = patrolSpeed;
        }
    }

    void GoToRandomPatrolPoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle * patrolRadius;
        currentPatrolTarget = patrolCenter + new Vector3(randomDirection.x, randomDirection.y, 0);

        path.maxSpeed = patrolSpeed;
        path.destination = currentPatrolTarget;
    }

    void ChasePlayer()
    {
        isChasing = true;
        path.maxSpeed = chaseSpeed;
        path.destination = player.position;
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Verifica distância
        if (distanceToPlayer > visionRange)
            return false;

        // Verifica ângulo (assumindo que o sprite olha para cima/direita por padrão)
        Vector3 forward = transform.up;
        float angle = Vector3.Angle(forward, directionToPlayer);

        if (angle > visionAngle / 2f)
            return false;

        // Verifica obstáculos
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer);

        if (hit.collider != null && hit.collider.transform != player)
            return false;

        return true;
    }

    // Visualização no Editor
    void OnDrawGizmosSelected()
    {
        if (TimerController.Instance == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();

        if (horaAtual >= horaInicioPerseguicao && horaAtual < horaFimPerseguicao)
        {
            Gizmos.color = isChasing ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, visionRange);

            Vector3 forward = transform.up;
            Vector3 leftBoundary = Quaternion.Euler(0, 0, visionAngle / 2f) * forward * visionRange;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -visionAngle / 2f) * forward * visionRange;

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(patrolCenter, patrolRadius);
        }

        // Mostra o destino atual
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(currentPatrolTarget, 0.3f);
    }
}