using UnityEngine;

public class MultiSpriteOutline : MonoBehaviour
{
    [Header("Sprites que vão ganhar borda")]
    public SpriteRenderer[] targetRenderers;   // partes do armário
    public Material outlineMaterial;          // OutlineMaterial

    private Material[] originalMaterials;

    void Start()
    {
        // Se não preencher manualmente, pega todos os SpriteRenderers filhos
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            targetRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        originalMaterials = new Material[targetRenderers.Length];
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            originalMaterials[i] = targetRenderers[i].material;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // liga outline em todas as partes
        if (outlineMaterial == null) return;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null)
                targetRenderers[i].material = outlineMaterial;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // volta para o material original
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null && originalMaterials[i] != null)
                targetRenderers[i].material = originalMaterials[i];
        }
    }
}
