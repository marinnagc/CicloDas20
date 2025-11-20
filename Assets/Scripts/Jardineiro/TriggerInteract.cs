using UnityEngine;

public class TriggerInteract : MonoBehaviour
{
    [Header("Objeto visual que recebe o outline")]
    public SpriteRenderer[] targetRenderers;

    [Header("Material do Outline")]
    public Material outlineMaterial;

    private Material[] originalMaterials;

    [Header("UI que abre quando interagir")]
    public GameObject dialogPanel;

    [Header("Tecla")]
    public KeyCode interactionKey = KeyCode.E;

    private bool playerInside = false;
    private bool dialogOpen = false;

    void Start()
    {
        // Guarda materiais originais
        if (targetRenderers != null && targetRenderers.Length > 0)
        {
            originalMaterials = new Material[targetRenderers.Length];
            for (int i = 0; i < targetRenderers.Length; i++)
                originalMaterials[i] = targetRenderers[i].material;
        }

        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(interactionKey))
        {
            if (!dialogOpen)
                OpenPanel();
            else
                ClosePanel();
        }
    }

    void OpenPanel()
    {
        dialogOpen = true;
        if (dialogPanel != null)
            dialogPanel.SetActive(true);
    }

    void ClosePanel()
    {
        dialogOpen = false;
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        // liga outline
        if (outlineMaterial != null)
        {
            for (int i = 0; i < targetRenderers.Length; i++)
                targetRenderers[i].material = outlineMaterial;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        ClosePanel();

        // volta material
        if (originalMaterials != null)
        {
            for (int i = 0; i < targetRenderers.Length; i++)
                targetRenderers[i].material = originalMaterials[i];
        }
    }
}
