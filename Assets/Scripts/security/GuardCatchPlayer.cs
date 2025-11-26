using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GuardCatchPlayer : MonoBehaviour
{
    [Header("Detecção")]
    [SerializeField] private string playerTag = "Player";

    private static bool isTransitioning = false;

    private void Awake()
    {
        // Garante que o collider seja trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        Debug.Log("[GuardCatchPlayer] Awake em " + gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[GuardCatchPlayer] OnTriggerEnter2D com " + other.name);

        if (!other.CompareTag(playerTag)) return;

        TryCatchPlayer();
    }

    private void TryCatchPlayer()
    {
        // Já está em transição? Não faz nada.
        if (isTransitioning) return;

        // Fora do horário → não pega
        if (!IsHorarioDeRonda())
        {
            Debug.Log("[GuardCatchPlayer] Player encostou FORA do horário de ronda. Ignorando.");
            return;
        }

        if (DayManager.Instance == null)
        {
            Debug.LogWarning("[GuardCatchPlayer] DayManager não encontrado!");
            return;
        }

        // Log de captura
        if (TimerController.Instance != null)
        {
            float hora = TimerController.Instance.GetHoraAtual();
            Debug.Log("[GuardCatchPlayer] CAPTUROU o player às " + hora.ToString("0.00") + "h. Iniciando transição de dia por captura.");
        }

        DayManager.Instance.IniciarTransicaoDiaPorCaptura();
        isTransitioning = true;
    }

    bool IsHorarioDeRonda()
    {
        if (TimerController.Instance == null)
        {
            Debug.LogWarning("[GuardCatchPlayer] TimerController.Instance == null ao checar horário de ronda.");
            return false;
        }

        int hora = TimerController.Instance.GetHoraInteira();

        // Segurança ativo das 20:00 até 21:59
        return hora >= 20 && hora < 22;
    }

    private void OnEnable()
    {
        isTransitioning = false;
    }
}
