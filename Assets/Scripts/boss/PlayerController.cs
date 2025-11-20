using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;      // velocidade base
    public static float globalSpeedMultiplier = 1f; // 1 = normal, <1 = mais lento

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
        rb.freezeRotation = true;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        bool isMoving = movement.sqrMagnitude > 0.01f;
        if (anim != null)
        {
            if (!string.IsNullOrEmpty(paramMoving)) anim.SetBool(paramMoving, isMoving);
            if (!string.IsNullOrEmpty(paramMoveX)) anim.SetFloat(paramMoveX, movement.x);
            if (!string.IsNullOrEmpty(paramMoveY)) anim.SetFloat(paramMoveY, movement.y);
            if (!string.IsNullOrEmpty(paramSpeed)) anim.SetFloat(paramSpeed, movement.sqrMagnitude);
        }

        if (movement.x < 0) sr.flipX = true;
        else if (movement.x > 0) sr.flipX = false;

        if (useYSorting)
            sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * sortingMultiplier);
    }

    void FixedUpdate()
    {
        float finalSpeed = moveSpeed * globalSpeedMultiplier;
        rb.MovePosition(rb.position + movement * finalSpeed * Time.fixedDeltaTime);
    }
}
