using UnityEngine;

public class GameDayManager : MonoBehaviour
{
    public static GameDayManager Instance { get; private set; }
    private const string PREF_KEY = "GameDayNumber";

    public int DayNumber { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        DayNumber = PlayerPrefs.GetInt(PREF_KEY, 1); // default dia 1
    }

    public void IncrementDay()
    {
        DayNumber++;
        PlayerPrefs.SetInt(PREF_KEY, DayNumber);
        PlayerPrefs.Save();
    }

    public void SetDay(int day)
    {
        DayNumber = day;
        PlayerPrefs.SetInt(PREF_KEY, DayNumber);
        PlayerPrefs.Save();
    }
}
