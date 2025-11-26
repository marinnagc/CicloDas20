using UnityEngine;

public class GlobalInteractButton : MonoBehaviour
{
    public void OnClickInteract()
    {
        // Busca o player da cena atual
        PlayerInteraction player = FindObjectOfType<PlayerInteraction>();

        if (player != null)
        {
            player.OnInteractButton();  // Chama o método do player atual
        }
        else
        {
            Debug.LogWarning("Nenhum PlayerInteraction encontrado na cena.");
        }
    }
}
