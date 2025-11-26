using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance { get; private set; }

    // -1, 0 ou 1
    public int Horizontal { get; private set; }
    public int Vertical { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // evita duplicados
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);   // fica vivo em todas as scenes
    }

    public void AddHorizontal(int value)
    {
        Horizontal += value;
        Horizontal = Mathf.Clamp(Horizontal, -1, 1);
    }

    public void AddVertical(int value)
    {
        Vertical += value;
        Vertical = Mathf.Clamp(Vertical, -1, 1);
    }

    public void ResetAxes()
    {
        Horizontal = 0;
        Vertical = 0;
    }
}
