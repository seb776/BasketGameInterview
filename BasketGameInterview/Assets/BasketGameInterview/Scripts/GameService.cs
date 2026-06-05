using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum GameState
{
    Menu,
    InGame,
    EndScreen
}

public enum GameplayState
{
    WaitingToShoot,
    CalibratingShoot,
    HasShot,
}

public class GameService : MonoBehaviour
{
    public GameObject ShootBasketBallStartPosition;
    public GameObject BasketBallPrefab;
    public TextMeshPro ScoreText;
    public GameObject BasketRim;
    public BoxCollider BasketRimPositionArea;
    public EnteredRimDetector EnteredRimDetector;

    private GameObject _currentBasketBall;
    private GameState _currentGameState;
    private int _currentScore;
    public RectTransform LineShootBall;
    private void _triggerNewBall()
    {
        _currentBasketBall = GameObject.Instantiate(BasketBallPrefab);
        _currentBasketBall.transform.position = ShootBasketBallStartPosition.transform.position;
    }

    private void MoveBasketRim()
    {
        var newX = UnityEngine.Random.Range(BasketRimPositionArea.bounds.min.x, BasketRimPositionArea.bounds.max.x);
        var newZ = UnityEngine.Random.Range(BasketRimPositionArea.bounds.min.z, BasketRimPositionArea.bounds.max.z);
        BasketRim.transform.position = new Vector3(newX, 0.0f, newZ);
    }

    private void _shootBall(Vector2 direction)
    {
        var disappearAfterDelay = _currentBasketBall.GetComponent<DisappearAfterDelay>();

        disappearAfterDelay.TriggerDisappear();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnteredRimDetector.OnBallScored += EnteredRimDetector_OnBallScored;
        _currentGameState = GameState.Menu;
    }

    private void EnteredRimDetector_OnBallScored(float distance)
    {
        var scoresToDistance = new List<Tuple<float, int>>();
        var foundScore = scoresToDistance.Last(el => el.Item1 < distance);
        _currentScore += foundScore.Item2;
        // TODO trigger sound and anim
    }

    private void _startGame()
    {
        _currentGameState= GameState.InGame;
        _currentScore = 0;
        _triggerNewBall();
    }

    private void _handleMenuInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _currentGameState = GameState.InGame;
            _startGame();
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
        ScoreText.text = _currentScore.ToString("D4"); // Padded to be length 4
        if (_currentBasketBall != null)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(_currentBasketBall.transform.position);
            GameSingleton.Instance.UIService.DrawUILine(LineShootBall, screenPos, Input.mousePosition);
        }
        if (_currentGameState == GameState.Menu)
        {
            GameSingleton.Instance.UIService.ShowMenu();
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
