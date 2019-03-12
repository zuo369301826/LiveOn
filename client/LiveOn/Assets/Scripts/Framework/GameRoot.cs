using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance { get; set; }

    void Start()
    {
        Instance = this;

        DontDestroyOnLoad(this);

        GameManager.instance.Init();
    }

    void OnDestroy()
    {
        GameManager.instance.Term();

        Instance = null;
    }

    void Update()
    {
        GameManager.instance.Tick();
    }

    void LateUpdate()
    {
        GameManager.instance.LateTick();
    }
}
