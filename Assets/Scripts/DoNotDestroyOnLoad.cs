using UnityEngine;

public class DoNotDestroyOnLoad : MonoBehaviour
{

    public static DoNotDestroyOnLoad Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
    }
}