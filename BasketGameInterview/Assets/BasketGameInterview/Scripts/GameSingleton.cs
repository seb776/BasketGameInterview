using UnityEngine;

public class GameSingleton : Singleton<GameSingleton>
{
    public GameService GameService;
    public UIService UIService;
    public SoundService SoundService;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
