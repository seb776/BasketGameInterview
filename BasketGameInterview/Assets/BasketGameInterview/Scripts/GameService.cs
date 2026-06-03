using UnityEngine;

public enum GameState
{
    Menu,
    InGame,
    EndScreen
}

public class GameService : MonoBehaviour
{
    public GameObject BasketBall;



    private GameState _currentGameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentGameState = GameState.Menu;    
    }

    private void _handleMenuInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _currentGameState = GameState.InGame;
        }
    }

    private void _handleEndScreenInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _currentGameState = GameState.InGame;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentGameState == GameState.Menu)
        {
            _handleMenuInputs();
        }
        if (_currentGameState == GameState.EndScreen)
        {
            _handleEndScreenInputs();
        }

        if (_currentGameState != GameState.Menu)
        {
            GameSingleton.Instance.UIService.HideMenu();
        }
        if (_currentGameState == GameState.EndScreen)
        {
            GameSingleton.Instance.UIService.ShowEndScreen();
        }
        if (_currentGameState != GameState.EndScreen)
        {
            GameSingleton.Instance.UIService.HideEndScreen();
        }

    }
}
