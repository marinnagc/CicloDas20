using UnityEngine;
using UnityEngine.UI;

public class BlinkArrow : MonoBehaviour
{
    public float speed = 2f; // velocidade do piscar
    private Image arrowImage;

    void Start()
    {
        arrowImage = GetComponent<Image>();
    }

    void Update()
    {
        float alpha = (Mathf.Sin(Time.time * speed) + 1f) / 2f; // valor oscilando entre 0 e 1
        arrowImage.color = new Color(arrowImage.color.r, arrowImage.color.g, arrowImage.color.b, alpha);
    }
}
