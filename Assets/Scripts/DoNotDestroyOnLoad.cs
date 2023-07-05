using UnityEngine;

public class DoNotDestroyOnLoad : MonoBehaviour
{
    private static bool isBGMPlaying = false;

    private void Awake()
    {
        if (!isBGMPlaying)
        {
            DontDestroyOnLoad(gameObject);
            isBGMPlaying = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}