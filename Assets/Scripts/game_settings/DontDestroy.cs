using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroy[] objs = FindObjectsOfType<DontDestroy>();

        if (objs.Length > 1)
        {
            // Já existe outro igual → destrói este antes de tudo
            Destroy(gameObject);
            return;
        }

        // Só o primeiro sobrevive entre cenas
        DontDestroyOnLoad(gameObject);
    }
}
