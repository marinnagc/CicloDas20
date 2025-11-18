using UnityEngine;

public class PersistentPostFX : MonoBehaviour

{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
