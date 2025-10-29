using UnityEngine;
using UnityEngine.Tilemaps;

public class InteractTilemap : MonoBehaviour
{
    [Header("Tiles específicos")]
    public TileBase quadroTile;                 // arraste o tile do quadro (do seu tileset)
    [Header("UI")]
    public BoardPanelController panelPrefab;    // prefab do painel
    public Canvas uiCanvas;                     // seu canvas
    [Header("Highlight")]
    public GameObject highlightPrefab;          // um sprite quadrado branco (bordazinha)
    public Color highlightColor = Color.white;  // cor da borda
    public float highlightOffset = 0.01f;

    private Tilemap tm;
    private GameObject highlightObj;
    private Vector3Int currentCell;
    private bool playerInside;
    private Transform player;

    void Start()
    {
        tm = GetComponent<Tilemap>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // instancia a borda
        if (highlightPrefab)
        {
            highlightObj = Instantiate(highlightPrefab);
            highlightObj.SetActive(false);
            highlightObj.GetComponent<SpriteRenderer>().color = highlightColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        if (highlightObj) highlightObj.SetActive(false);
    }

    void Update()
    {
        if (!playerInside) return;

        // descobre em qual célula do INTERACT o player está
        Vector3Int cell = tm.WorldToCell(player.position);
        TileBase tile = tm.GetTile(cell);

        if (tile == quadroTile)
        {
            // desenha borda (highlight)
            if (highlightObj)
            {
                highlightObj.SetActive(true);
                highlightObj.transform.position = tm.GetCellCenterWorld(cell) + Vector3.forward * highlightOffset;
            }

            // apertou E → abre painel
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (uiCanvas == null) uiCanvas = EnsureCanvas();
                var panel = Instantiate(panelPrefab, uiCanvas.transform);
                Time.timeScale = 0f;
                panel.Init(
                    "Seg: 08–16 Ana | 16–00 Marcelo\nTroca às 19:45.",
                    "Câmeras ala Leste em manutenção 19:40–20:00."
                );
            }
        }
        else if (highlightObj)
        {
            highlightObj.SetActive(false);
        }
    }

    Canvas EnsureCanvas()
    {
        var existing = FindObjectOfType<Canvas>();
        if (existing) return existing;

        var go = new GameObject("UICanvas",
            typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
        go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        if (!FindObjectOfType<UnityEngine.EventSystems.EventSystem>())
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));

        return go.GetComponent<Canvas>();
    }
}
