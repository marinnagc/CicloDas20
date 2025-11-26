using UnityEngine;
using UnityEngine.EventSystems;

public class MobileMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("Direção desse botão: (-1,0), (1,0), (0,1) ou (0,-1)")]
    public int horizontal;
    public int vertical;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MobileInput.Instance == null) return;

        if (horizontal != 0)
            MobileInput.Instance.AddHorizontal(horizontal);
        if (vertical != 0)
            MobileInput.Instance.AddVertical(vertical);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (MobileInput.Instance == null) return;

        if (horizontal != 0)
            MobileInput.Instance.AddHorizontal(-horizontal);
        if (vertical != 0)
            MobileInput.Instance.AddVertical(-vertical);
    }
}
