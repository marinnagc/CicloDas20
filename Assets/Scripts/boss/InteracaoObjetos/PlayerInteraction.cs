using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float radius = 0.9f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (var h in hits)
            {
                var it = h.GetComponent<IInteractable>();
                if (it != null) { it.Interact(gameObject); break; }
            }
        }
    }
}
