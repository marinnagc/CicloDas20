using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MedicineWarningUI : MonoBehaviour
{
    public static MedicineWarningUI Instance;

    [Header("Referências UI")]
    [SerializeField] private GameObject warningPanel; // O Canvas ou um painel pai
    [SerializeField] private Image redOverlay;
    [SerializeField] private TextMeshProUGUI warningText;

    [Header("Configurações")]
    [SerializeField] private int horarioAtivar = 16; // 16h
    [SerializeField] private float duracaoTela = 5f; // 5 segundos
    [SerializeField] private string mensagemAviso = "⚠️ REMÉDIO ATIVADO!\nDURAÇÃO: 2 HORAS ⚠️";

    [Header("Animação (opcional)")]
    [SerializeField] private bool usarFadeIn = true;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private bool usarPiscada = true;
    [SerializeField] private float piscadaVelocidade = 0.5f;

    private bool jaExibiu = false;
    private int ultimoHorarioChecado = -1;

    void Awake()
    {
        // Garante que só existe um MedicineWarningUI entre todas as cenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantém o objeto nas trocas de cena
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Esconde a UI no início
        if (warningPanel != null)
            warningPanel.SetActive(false);

        if (redOverlay != null)
            redOverlay.gameObject.SetActive(false);

        if (warningText != null)
        {
            warningText.text = mensagemAviso;
            warningText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (TimerController.Instance == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();

        // Detecta quando chega às 16h (ou horário configurado)
        if (horaAtual == horarioAtivar && !jaExibiu && ultimoHorarioChecado != horaAtual)
        {
            StartCoroutine(MostrarAviso());
            jaExibiu = true;
        }

        // Reset para o próximo dia (quando passar da meia-noite ou voltar para hora anterior)
        if (horaAtual < horarioAtivar && jaExibiu)
        {
            jaExibiu = false;
        }

        ultimoHorarioChecado = horaAtual;
    }

    IEnumerator MostrarAviso()
    {
        Debug.Log("[MedicineWarning] Exibindo aviso de remédio!");

        // Ativa a UI
        if (warningPanel != null)
            warningPanel.SetActive(true);

        if (redOverlay != null)
            redOverlay.gameObject.SetActive(true);

        if (warningText != null)
            warningText.gameObject.SetActive(true);

        // Fade in (opcional)
        if (usarFadeIn && redOverlay != null && warningText != null)
        {
            yield return StartCoroutine(FadeIn());
        }

        // Piscar texto (opcional)
        Coroutine piscarCoroutine = null;
        if (usarPiscada && warningText != null)
        {
            piscarCoroutine = StartCoroutine(PiscarTexto());
        }

        // Aguarda o tempo configurado
        yield return new WaitForSeconds(duracaoTela);

        // Para a piscada
        if (piscarCoroutine != null)
        {
            StopCoroutine(piscarCoroutine);
        }

        // Fade out (opcional)
        if (usarFadeIn && redOverlay != null && warningText != null)
        {
            yield return StartCoroutine(FadeOut());
        }

        // Esconde a UI
        if (warningPanel != null)
            warningPanel.SetActive(false);

        if (redOverlay != null)
            redOverlay.gameObject.SetActive(false);

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        Debug.Log("[MedicineWarning] Aviso ocultado!");
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color overlayColor = redOverlay.color;
        Color textColor = warningText.color;

        overlayColor.a = 0f;
        textColor.a = 0f;
        redOverlay.color = overlayColor;
        warningText.color = textColor;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);

            overlayColor.a = alpha * 0.6f; // 60% de opacidade no máximo
            textColor.a = alpha;

            redOverlay.color = overlayColor;
            warningText.color = textColor;

            yield return null;
        }

        // Garante opacidade final
        overlayColor.a = 0.6f;
        textColor.a = 1f;
        redOverlay.color = overlayColor;
        warningText.color = textColor;
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color overlayColor = redOverlay.color;
        Color textColor = warningText.color;

        float startAlphaOverlay = overlayColor.a;
        float startAlphaText = textColor.a;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);

            overlayColor.a = alpha * startAlphaOverlay;
            textColor.a = alpha * startAlphaText;

            redOverlay.color = overlayColor;
            warningText.color = textColor;

            yield return null;
        }

        // Garante opacidade final
        overlayColor.a = 0f;
        textColor.a = 0f;
        redOverlay.color = overlayColor;
        warningText.color = textColor;
    }

    IEnumerator PiscarTexto()
    {
        while (true)
        {
            // Fade out
            float elapsed = 0f;
            Color textColor = warningText.color;
            float startAlpha = textColor.a;

            while (elapsed < piscadaVelocidade / 2f)
            {
                elapsed += Time.deltaTime;
                textColor.a = Mathf.Lerp(startAlpha, 0.3f, elapsed / (piscadaVelocidade / 2f));
                warningText.color = textColor;
                yield return null;
            }

            // Fade in
            elapsed = 0f;
            while (elapsed < piscadaVelocidade / 2f)
            {
                elapsed += Time.deltaTime;
                textColor.a = Mathf.Lerp(0.3f, 1f, elapsed / (piscadaVelocidade / 2f));
                warningText.color = textColor;
                yield return null;
            }
        }
    }
}
