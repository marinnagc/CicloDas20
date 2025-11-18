using UnityEngine;
using UnityEngine.UI;

public class ScanLineScroll : MonoBehaviour
{
    public float speed = 0.1f;

    private Material mat;
    private float offset;

    void Awake()
    {
        var img = GetComponent<Image>();
        mat = new Material(img.material);
        img.material = mat;
    }

    void Update()
    {
        offset += speed * Time.deltaTime;
        mat.mainTextureOffset = new Vector2(0f, offset);
    }
}
