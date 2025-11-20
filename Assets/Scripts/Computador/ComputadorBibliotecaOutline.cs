using UnityEngine;

public class ComputadorBibliotecaOutline : MonoBehaviour
{
    public Material outlineMaterial;   // arrasta só o OutlineMaterial aqui
    private Material normalMaterial;   // NÃO precisa setar no Inspector
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // guarda o material original do sprite
        normalMaterial = sr.material;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && outlineMaterial != null)
        {
            sr.material = outlineMaterial;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player") && normalMaterial != null)
        {
            sr.material = normalMaterial;
        }
    }
}
