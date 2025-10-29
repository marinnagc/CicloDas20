using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HighlightBorder2D : MonoBehaviour
{
    public float thickness = 0.05f;
    public float padding = 0.02f; // “folga” para a borda ficar um pouco maior que o collider
    public string sortingLayerName = "Default";
    public int sortingOrder = 50;

    void Awake() { Build(); }
    void OnValidate() { if (Application.isEditor) Build(); }

    void Build()
    {
        var lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.startWidth = lr.endWidth = thickness;
        lr.sortingLayerName = sortingLayerName;
        lr.sortingOrder = sortingOrder;

        var bc = GetComponentInParent<BoxCollider2D>();
        var size = bc ? bc.size : new Vector2(1f, 1f);
        size += Vector2.one * (padding * 2f);
        var h = size * 0.5f;

        lr.positionCount = 4;
        lr.SetPositions(new Vector3[] {
            new Vector3(-h.x, -h.y, 0),
            new Vector3(-h.x,  h.y, 0),
            new Vector3( h.x,  h.y, 0),
            new Vector3( h.x, -h.y, 0),
        });
    }
}
