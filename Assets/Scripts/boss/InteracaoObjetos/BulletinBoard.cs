using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BulletinBoard : MonoBehaviour, IInteractable
{
    [TextArea(3, 12)]
    public string turnos =
@"Seg: 08–16 Ana | 16–00 Marcelo
Ter: 08–16 Júlia | 16–00 Roberto
Qua: 08–16 Pedro | 16–00 Carla
Obs: Troca de turno às 19:45.";

    [TextArea(3, 12)]
    public string avisos =
@"Impressoras Setor B com pouca tinta.
Câmeras ala Leste em manutenção 19:40–20:00.
Perdido: cartão magnético prata #27.";

    public BoardPanelController panelPrefab;   // arraste o prefab
    public Canvas uiCanvas;                    // arraste o Canvas (ou deixe vazio)
    public GameObject highlight;               // arraste o filho "Highlight" (opcional)

    public string GetPrompt() => "Pressione [E] para ler o quadro";

    public void Interact(GameObject player)
    {
        if (uiCanvas == null) uiCanvas = EnsureCanvas();
        var panel = Instantiate(panelPrefab, uiCanvas.transform);
        Time.timeScale = 0f;     // pausar enquanto lê (opcional)
        panel.Init(turnos, avisos);
    }

    // Liga/desliga a borda branca quando o player entra/sai
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && highlight) highlight.SetActive(true);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && highlight) highlight.SetActive(false);
    }
    void OnDisable()
    {
        if (highlight) highlight.SetActive(false);
    }

    Canvas EnsureCanvas()
    {
        var existing = FindObjectOfType<Canvas>();
        if (existing) return existing;

        var go = new GameObject("UICanvas",
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var c = go.GetComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;

        if (!FindObjectOfType<EventSystem>())
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        return c;
    }
}
