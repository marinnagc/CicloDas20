using UnityEngine;

public class YSort : MonoBehaviour
{
    public int multiplier = 100;
    public int offset = 0;

    private SpriteRenderer[] renderers;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void LateUpdate()
    {
        int order = (int)(-transform.position.y * multiplier) + offset;
        foreach (var r in renderers)
            if (r != null)
                r.sortingOrder = order;
    }
}
