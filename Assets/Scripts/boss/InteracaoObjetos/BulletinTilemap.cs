using UnityEngine;
using UnityEngine.Tilemaps;

public class BulletinTilemap : MonoBehaviour
{
    public TileBase quadroTile;               // arraste o tile do quadro aqui
    public GameObject highlightPrefab;        // sprite da borda (um pequeno outline branco)
    public BoardPanelController panelPrefab;  // prefab do painel
    public Canvas uiCanvas;                   // seu Canvas da cena

    private Tilemap tm;
    private GameObject highlightObj;
    private Transform player;

    void Start()
    {
        tm = GetComponent<Tilemap>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (highlightPrefab)
        {
            highlightObj = Instantiate(highlightPrefab);
            highlightObj.SetActive(false);
        }
    }

    void Update()
    {
        Vector3Int cell = tm.WorldToCell(player.position);
        TileBase tile = tm.GetTile(cell);

        bool pertoDoQuadro = false;
        if (tile == quadroTile)
        {
            Vector3 tileCenter = tm.GetCellCenterWorld(cell);
            float dist = Vector2.Distance(player.position, tileCenter);
            pertoDoQuadro = dist < 1.0f; // ajuste o raio

            if (highlightObj)
            {
                highlightObj.SetActive(pertoDoQuadro);
                if (pertoDoQuadro)
                    highlightObj.transform.position = tileCenter;
            }
        }
        else if (highlightObj)
        {
            highlightObj.SetActive(false);
        }

        // Interação
        if (pertoDoQuadro && Input.GetKeyDown(KeyCode.E))
        {
            if (uiCanvas == null) uiCanvas = EnsureCanvas();
            var panel = Instantiate(panelPrefab, uiCanvas.transform);
            Time.timeScale = 0f;
            panel.Init(
                "Comunicado interno nº 16/20:\r\nTodos os pacientes devem comparecer à enfermaria para medicação às 16h.\r\n\r\nAtenção: horários são fixos e devem ser respeitados para evitar colapsos de rotina.\r\n\r\nAssinado,\r\nDr. Maurício — Supervisor Clínico\r\n"
            );
        }
    }

    Canvas EnsureCanvas()
    {
        var existing = FindObjectOfType<Canvas>();
        if (existing) return existing;
        var go = new GameObject("UICanvas", typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
        go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        if (!FindObjectOfType<UnityEngine.EventSystems.EventSystem>())
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        return go.GetComponent<Canvas>();
    }
}
