using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnHover : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // quando o mouse entra no botão, ele vira o "selecionado"
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
