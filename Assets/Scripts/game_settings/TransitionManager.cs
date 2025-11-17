using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [Header("Config (override in inspector if you want)")]
    public float fadeDuration = 1.0f;
    public float dayTextDuration = 1.8f;
    public string dayTextFormat = "DAY {0}";

    // References created at runtime (if not atribuídas)
    private Image fadeScreen;
    private TMP_Text dayText;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureUIExists();
    }

    // Checa se existe uma Image named "FadeScreen" e TMP "DayText" na cena; se não, cria uma simples.
    private void EnsureUIExists()
    {
        // tenta achar na cena (útil se você já criou no Canvas do menu)
        fadeScreen = GameObject.FindObjectOfType<Image>()?.gameObject.name == "FadeScreen"
            ? GameObject.Find("FadeScreen").GetComponent<Image>()
            : null;

        if (fadeScreen == null)
        {
            // cria Canvas -> Image
            GameObject canvasGO = new GameObject("TransitionCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasGO);

            GameObject imgGO = new GameObject("FadeScreen");
            imgGO.transform.SetParent(canvasGO.transform, false);
            fadeScreen = imgGO.AddComponent<Image>();
            fadeScreen.rectTransform.anchorMin = Vector2.zero;
            fadeScreen.rectTransform.anchorMax = Vector2.one;
            fadeScreen.rectTransform.offsetMin = Vector2.zero;
            fadeScreen.rectTransform.offsetMax = Vector2.zero;
            fadeScreen.color = new Color(0, 0, 0, 0);
            imgGO.SetActive(false);
            DontDestroyOnLoad(imgGO);
        }

        // procura TMP DayText
        GameObject dayTextGO = GameObject.Find("DayText");
        if (dayTextGO != null)
        {
            dayText = dayTextGO.GetComponent<TMP_Text>();
        }
        else
        {
            // cria um TMP (assume que TextMeshPro está importado no projeto)
            GameObject textGO = new GameObject("DayText");
            textGO.transform.SetParent(fadeScreen.transform, false);
            dayText = textGO.AddComponent<TMP_Text>();
            dayText.alignment = TextAlignmentOptions.Center;
            dayText.fontSize = 96;
            RectTransform rt = dayText.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.1f, 0.4f);
            rt.anchorMax = new Vector2(0.9f, 0.6f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            dayText.text = string.Format(dayTextFormat, 1);
            dayText.gameObject.SetActive(false);
        }
    }

    // Public API: mostra a tela e o texto DAY X. Se incrementDay==true, só mostra o dado fornecido (use GameDayManager antes).
    public void ShowDayTransition(int dayNumber)
    {
        StartCoroutine(DoTransition(dayNumber));
    }

    private IEnumerator DoTransition(int dayNumber)
    {
        // ativa fade screen
        fadeScreen.gameObject.SetActive(true);

        // fade-in
        float t = 0f;
        Color c = fadeScreen.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeScreen.color = c;
            yield return null;
        }
        c.a = 1f;
        fadeScreen.color = c;

        // mostra texto
        dayText.text = string.Format(dayTextFormat, dayNumber);
        dayText.gameObject.SetActive(true);

        yield return new WaitForSeconds(dayTextDuration);

        // opcional: fade-out (se quiser sumir); aqui eu faço fade-out para voltar ao jogo
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeScreen.color = c;
            yield return null;
        }
        c.a = 0f;
        fadeScreen.color = c;
        dayText.gameObject.SetActive(false);
        fadeScreen.gameObject.SetActive(false);
    }
}
