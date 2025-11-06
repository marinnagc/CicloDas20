using UnityEngine;
using Pathfinding; // Certifique-se de que o pacote A* Pathfinding Project está instalado no seu projeto Unity

public class AiAgent : MonoBehaviour
{
    private AIPath path;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform target;

    private void Start()
    {
        path = GetComponent<AIPath>();
    }

    private void Update()
    {
        path.maxSpeed = moveSpeed;

        // random position if wandering
        path.destination = target.position;
    }
}
