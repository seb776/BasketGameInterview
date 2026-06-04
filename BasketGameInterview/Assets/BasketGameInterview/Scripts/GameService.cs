using UnityEngine;

public enum GameState
{
    Menu,
    InGame,
    EndScreen
}

public class GameService : MonoBehaviour
{
    public GameObject ShootBasketBallStartPosition;
    public GameObject BasketBallPrefab;

    private GameObject _currentBasketBall;

    private GameState _currentGameState;

    private void _triggerNewBall()
    {
        _currentBasketBall = GameObject.Instantiate(BasketBallPrefab);
        _currentBasketBall.transform.position = ShootBasketBallStartPosition.transform.position;
    }

    private void _shootBall(Vector2 direction)
    {
        var disappearAfterDelay = _currentBasketBall.GetComponent<DisappearAfterDelay>();

        disappearAfterDelay.TriggerDisappear();
    }

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
