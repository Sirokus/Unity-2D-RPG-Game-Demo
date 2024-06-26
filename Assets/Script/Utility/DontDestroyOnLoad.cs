using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public static DontDestroyOnLoad ins;

    private void Awake()
    {
        if (ins != null)
        {
            Destroy(gameObject);
            return;
        }
        ins = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
