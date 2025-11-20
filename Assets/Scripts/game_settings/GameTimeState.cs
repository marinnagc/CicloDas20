using UnityEngine;

/// Guarda o dia atual e a hora em que o dia deve começar.
/// É estático, então continua valendo mesmo trocando de cena.
public static class GameTimeState
{
    // Dia atual (se quiser usar depois)
    public static int CurrentDay = 1;

    // Hora em que o relógio do jogo começa
    public static float StartHour = 6f;   // dia 1 começa às 6

    public static void SetDay2()
    {
        CurrentDay = 2;
        StartHour = 8f;   // dia 2 começa às 8
    }

    public static void ResetToDay1()
    {
        CurrentDay = 1;
        StartHour = 6f;
    }
}
