using UnityEngine;

public class SoundFromTimer : MonoBehaviour
{
    [Header("Referências")]
    public TimerController timer;     // seu relógio
    public AudioSource sfxSource;     // AudioSource que toca os efeitos

    [Header("Clipes de áudio")]
    public AudioClip alarmClip;       // som das 16h
    public AudioClip curfewClip;      // som das 20h

    [Header("Horários (24h)")]
    public float alarmHour = 16f;     // aviso do remédio
    public float curfewHour = 20f;    // toque de recolher
    public float resetHour = 6f;      // hora em que o dia "recomeça"

    private bool alarmPlayed = false;
    private bool curfewPlayed = false;

    void Update()
    {
        if (timer == null || sfxSource == null)
            return;

        float horaAtual = timer.GetHoraAtual();   // ex: 16.5 = 16:30

        // --- ALARME DAS 16h ---
        if (!alarmPlayed &&
            alarmClip != null &&
            horaAtual >= alarmHour &&
            horaAtual < alarmHour + 0.05f)       // janelinha pequena
        {
            sfxSource.PlayOneShot(alarmClip);
            alarmPlayed = true;
        }

        // --- CURFEW DAS 20h ---
        if (!curfewPlayed &&
            curfewClip != null &&
            horaAtual >= curfewHour &&
            horaAtual < curfewHour + 0.05f)
        {
            sfxSource.PlayOneShot(curfewClip);
            curfewPlayed = true;
        }

        // --- reset para o próximo dia ---
        if (horaAtual < resetHour)
        {
            alarmPlayed = false;
            curfewPlayed = false;
        }
    }
}
