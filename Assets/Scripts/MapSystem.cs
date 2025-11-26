using UnityEngine;
using UnityEngine.UI;

public class MapSystem : MonoBehaviour
{
    [Header("UI DO MAPA")]
    public GameObject mapWindow;      // MapPanel
    public Button closeMapButton;     // botão pra fechar o mapa

    void Start()
    {
        // começa fechado
        mapWindow.SetActive(false);

        // liga o botão de fechar
        closeMapButton.onClick.AddListener(CloseMap);
    }

    // chamado pelo MapButton (OnClick)
    public void OpenMap()
    {
        mapWindow.SetActive(true);
        // NÃO mexe no Time.timeScale aqui
    }

    public void CloseMap()
    {
        mapWindow.SetActive(false);
        // também não mexe no Time.timeScale
    }
}
