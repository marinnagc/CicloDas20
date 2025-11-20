using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    [Header("UI a ser exibida (post-it / bilhete)")]
    public GameObject dialogPanel;              // UI Image no Canvas

    [Header("Highlight")]
    public SpriteRenderer[] targetRenderers;    // todos os sprites do armário
    public Material outlineMaterial;           // OutlineMaterial

    [Header("Tecla de interação")]
    public KeyCode interactionKey = KeyCode.E;

    private bool playerInside = false;
    private bool dialogOpen = false;
    private Material[] originalMaterials;

    void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        if (targetRenderers != null && targetRenderers.Length > 0)
        {
            originalMaterials = new Material[targetRenderers.Length];
            for (int i = 0; i < targetRenderers.Length; i++)
                originalMaterials[i] = targetRenderers[i].material;
        }
    }

    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(interactionKey))
        {
            if (!dialogOpen) Abrir();
            else Fechar();
        }
    }

    void Abrir()
    {
        dialogOpen = true;
        if (dialogPanel != null)
            dialogPanel.SetActive(true);
    }

    void Fechar()
    {
        dialogOpen = false;
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        // liga outline em todas as partes do armário
        if (outlineMaterial != null && targetRenderers != null)
        {
            foreach (var r in targetRenderers)
                if (r != null) r.material = outlineMaterial;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        Fechar();

        // volta pros materiais originais
        if (originalMaterials != null && targetRenderers != null)
        {
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] != null && originalMaterials[i] != null)
                    targetRenderers[i].material = originalMaterials[i];
            }
        }
    }
}
