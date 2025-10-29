using UnityEngine;
// use TMP em vez do Text legacy:
using TMPro;

public class BoardPanelController : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text content;

    public void Init(string turnos, string avisos)
    {
        if (title) title.text = "Turnos & Avisos";
        if (content) content.text = $"<b>TURNOS</b>\n{turnos}\n\n<b>AVISOS</b>\n{avisos}";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            Close();
    }

    public void Close()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}
