using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public abstract class Interactable : MonoBehaviour
{
    public Vector2 interact_direction = new Vector2(0, 1);
    public void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }


    public abstract void Interact();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().OpenInteractIcon();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().CloseInteractIcon();
        }
    }
}
