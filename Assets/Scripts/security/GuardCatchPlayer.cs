using System.Collections;
using UnityEngine;

public class GuardCatchPlayer : MonoBehaviour
{
    [Header("Detecção")]
    [SerializeField] private string playerTag = "Player";

    private static bool isTransitioning = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(playerTag)) return;
        StartTransition();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        StartTransition();
    }

    private void StartTransition()
    {
        if (isTransitioning) return;

        // Chama o DayManager para reiniciar o dia com a mensagem personalizada
        if (DayManager.Instance != null)
        {
            DayManager.Instance.IniciarTransicaoDiaPorCaptura();
            isTransitioning = true;
        }
        else
        {
            Debug.LogWarning("[GuardCatchPlayer] DayManager não encontrado!");
        }
    }

    // Reseta o flag quando voltar para a cena
    private void OnEnable()
    {
        isTransitioning = false;
    }
}