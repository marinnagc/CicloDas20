using UnityEngine;
// use TMP em vez do Text legacy:
using TMPro;

public class BoardPanelController : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text content;

    public void Init(string avisos)
    {
        if (content) content.text = $"<b>AVISOS</b>\n{avisos}";
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
