using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuArrowController : MonoBehaviour
{
    public RectTransform arrow;      // a setinha (ArrowSelector)
    public Button firstButton;       // primeiro botão (Novo Jogo)

    void Start()
    {
        // Define o botão inicial selecionado
        EventSystem.current.SetSelectedGameObject(firstButton.gameObject);

        // Garante que a seta comece na posição do primeiro botão
        if (arrow != null && firstButton != null)
        {
            arrow.position = GetArrowPosition(firstButton.transform as RectTransform);
        }
    }

    void Update()
    {
        // Pega o botão atualmente selecionado (pela seta do teclado OU mouse)
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected != null && arrow != null)
        {
            RectTransform btnRect = selected.GetComponent<RectTransform>();
            if (btnRect != null)
            {
                arrow.position = GetArrowPosition(btnRect);
            }
        }
    }

    Vector3 GetArrowPosition(RectTransform buttonRect)
    {
        // Mantém o X da seta fixo, só muda o Y pra alinhar com o botão
        return new Vector3(
            arrow.position.x,
            buttonRect.position.y,
            arrow.position.z
        );
    }
}
