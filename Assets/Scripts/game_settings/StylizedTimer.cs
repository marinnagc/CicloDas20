using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StylizedTimer : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text clockText;                 // mostra HH:MM do jogo
    public TMP_Text periodLabel;               // "Manhã", "Tarde", "Noite", ...
    public Image dayProgressFill;              // Image com Fill (horizontal)
    public DayNightController dayNight; // referência ao controler

    [Header("Smoothing")]
    public float fillSmoothSpeed = 6f;

    void Start()
    {
        if (clockText == null) Debug.LogWarning("ClockText não atribuído.");
        if (dayNight == null) Debug.LogWarning("DayNightControllerFastDay não atribuído.");
    }

    void Update()
    {
        if (dayNight == null) return;

        // Atualiza texto HH:MM
        if (clockText != null)
            clockText.text = dayNight.GetCurrentTimeString();

        // Label (manhã/tarde/noite)
        if (periodLabel != null)
        {
            float nf = dayNight.normalizedTime;
            string label = DetermineLabel(nf);
            periodLabel.text = label;
        }

        // Barra de progresso (0..1)
        if (dayProgressFill != null)
        {
            float target = dayNight.normalizedTime;
            dayProgressFill.fillAmount = Mathf.Lerp(dayProgressFill.fillAmount, target, Time.deltaTime * fillSmoothSpeed);
        }
    }

    string DetermineLabel(float normalized)
    {
        // base simples: mapeia o normalizedTime para rótulos entre startHour e endHour
        // Aproveitamos GetCurrentHourFloat mas sem custo de precisão extra
        float hourFloat = dayNight.GetCurrentHourFloat();
        int hour = Mathf.FloorToInt(hourFloat) % 24;
        if (hour >= 6 && hour < 12) return "Manhã";
        if (hour >= 12 && hour < 18) return "Tarde";
        return "Noite";
    }
}
