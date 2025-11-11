using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Start()
    {
        DontDestroy[] objs = FindObjectsOfType<DontDestroy>();

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
