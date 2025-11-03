using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BulletinBoard : MonoBehaviour, IInteractable
{
    [TextArea(3, 12)]
    public string avisos =
@"Comunicado interno nº 16/20:
Todos os pacientes devem comparecer à enfermaria para medicação às 16h.

Atenção: horários são fixos e devem ser respeitados para evitar colapsos de rotina.

Assinado,
Dr. Maurício — Supervisor Clínico
";

    public BoardPanelController panelPrefab;   // arraste o prefab
    public Canvas uiCanvas;                    // arraste o Canvas (ou deixe vazio)
    public GameObject highlight;               // arraste o filho "Highlight" (opcional)

    public string GetPrompt() => "Pressione [E] para ler o quadro";

    public void Interact(GameObject player)
    {
        if (uiCanvas == null) uiCanvas = EnsureCanvas();
        var panel = Instantiate(panelPrefab, uiCanvas.transform);
        Time.timeScale = 0f;     // pausar enquanto lê (opcional)
        panel.Init(avisos);
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
