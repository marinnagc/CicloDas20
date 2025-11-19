using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : MonoBehaviour
{
    // Definido pela porta ANTES de chamar LoadScene
    public static string nextSpawnPoint;

    void Awake()
    {
        // Garante que vamos reagir quando a cena terminar de carregar (caso Player seja DontDestroyOnLoad)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Tenta posicionar no Start (caso o Player exista na cena carregada)
        TryPlacePlayer();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ap�s cena carregada, tenta posicionar (bom para player persistente)
        TryPlacePlayer();
    }

    void TryPlacePlayer()
    {
        if (string.IsNullOrEmpty(nextSpawnPoint)) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach (var sp in spawnPoints)
        {
            if (sp.spawnID == nextSpawnPoint)
            {
                player.transform.position = sp.transform.position;
                // opcional: zerar velocidade do rigidbody se houver
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }

                // limpamos para n�o reaplicar acidentalmente
                nextSpawnPoint = null;
                return;
            }
        }

        // se n�o encontrou o spawn, opcionalmente logar
        Debug.LogWarning($"PlayerSpawnManager: spawn '{nextSpawnPoint}' n�o encontrado na cena {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        nextSpawnPoint = null;
    }
}
