using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float radius = 0.9f;

    void Update()
    {
        // Tecla E (pra testar no PC)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    // === chamado pelo botão de interação ===
    public void OnInteractButton()
    {
        Debug.Log("BOTÃO DE INTERAÇÃO CLICADO");  // <-- LOG PRA TESTAR
        TryInteract();
    }

    void TryInteract()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, radius);
        Debug.Log($"TENTANDO INTERAGIR... Hits encontrados: {hits.Length}");

        foreach (var h in hits)
        {
            Debug.Log("Hit em: " + h.name);

            // procura em vários lugares (objeto, pai, filho)
            var it =
                h.GetComponent<IInteractable>() ??
                h.GetComponentInParent<IInteractable>() ??
                h.GetComponentInChildren<IInteractable>();

            if (it != null)
            {
                Debug.Log("ACHOU OBJETO INTERAGÍVEL: " + h.name);
                it.Interact(gameObject);
                break;
            }
        }
    }

}
