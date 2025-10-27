using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;      // unidades por segundo

    [Header("Animator Parameter Names")]
    public string paramMoving = "Moving"; // bool (Idle <-> Walk)
    public string paramMoveX = "MoveX";  // optional (directions)
    public string paramMoveY = "MoveY";
    public string paramSpeed = "Speed";

    [Header("Sorting / Depth")]
    public bool useYSorting = true;
    public int sortingMultiplier = 100; // quanto maior, mais granular

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    Vector2 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // segurança: evitar rotação
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Input (setas ou WASD)
        movement.x = Input.GetAxisRaw("Horizontal"); // -1,0,1
        movement.y = Input.GetAxisRaw("Vertical");

        // Normaliza para evitar maior velocidade na diagonal
        movement = movement.normalized;

        // Animator params
        bool isMoving = movement.sqrMagnitude > 0.01f;
        if (anim != null)
        {
            if (!string.IsNullOrEmpty(paramMoving)) anim.SetBool(paramMoving, isMoving);
            if (!string.IsNullOrEmpty(paramMoveX)) anim.SetFloat(paramMoveX, movement.x);
            if (!string.IsNullOrEmpty(paramMoveY)) anim.SetFloat(paramMoveY, movement.y);
            if (!string.IsNullOrEmpty(paramSpeed)) anim.SetFloat(paramSpeed, movement.sqrMagnitude);
        }

        // flip horizontal (se seus sprites forem para a direita por padrão)
        if (movement.x < 0) sr.flipX = true;
        else if (movement.x > 0) sr.flipX = false;

        // sorting order por Y (simples)
        if (useYSorting)
            sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * sortingMultiplier);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
