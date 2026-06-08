using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public TextMeshPro TimerText;
    public GameObject BasketRim;
    public BoxCollider BasketRimPositionArea;
    public EnteredRimDetector EnteredRimDetector;
    public Text EndUIScore;

    private GameObject _currentBasketBall;
    private GameState _currentGameState;
    private GameplayState _currentGameplayState;
    private int _currentScore;
    public RectTransform LineShootBall;
    public float ShootForceMultiplier = 0.05f;
    public float ShootVerticalBoost = 1.0f;
    public float ShootBallDelay = 1.5f;
    private float _shootBallCooldown;

    // Player has ShootStableDelay seconds of steady aim. After that, the longer
    // they hold, the more the ball shakes (via Shake component on the ball) and
    // the more randomness is added to the released shot direction.
    public float ShootStableDelay = 1.0f;
    public float ShootInstabilityAmplitudePerSecond = 0.5f;   // how fast Shake.Amplitude ramps up per "overage" second
    public float ShootInstabilityForceRandomness = 60.0f;     // random offset added to shoot direction per "overage" second
    private float _calibrationStartTime;
    private Shake _currentBallShake;

    private UIService _uiSvc => GameSingleton.Instance.UIService;
    private void _triggerNewBall()
    {
        _currentBasketBall = GameObject.Instantiate(BasketBallPrefab);
        _currentBasketBall.transform.position = ShootBasketBallStartPosition.transform.position;
        var rb = _currentBasketBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        _currentBallShake = _currentBasketBall.GetComponent<Shake>();
        if (_currentBallShake != null)
        {
            _currentBallShake.Amplitude = 0.0f;
        }
        _currentGameplayState = GameplayState.WaitingToShoot;
    }

    private void _clearCurrentBall()
    {
        if (_currentBasketBall != null)
        {
            Destroy(_currentBasketBall);
            _currentBasketBall = null;
        }
        _currentBallShake = null;
    }

    private void MoveBasketRim()
    {
        var newX = UnityEngine.Random.Range(BasketRimPositionArea.bounds.min.x, BasketRimPositionArea.bounds.max.x);
        var newZ = UnityEngine.Random.Range(BasketRimPositionArea.bounds.min.z, BasketRimPositionArea.bounds.max.z);
        BasketRim.transform.position = new Vector3(newX, 0.0f, newZ);
    }

    private void _shootBall(Vector2 direction)
    {
        var rb = _currentBasketBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            // Convert screen-space drag direction into a world-space force.
            // Horizontal screen drag -> sideways (camera right)
            // Vertical screen drag   -> forward (camera forward) + a bit of up
            Camera cam = Camera.main;
            Vector3 worldDir =
                cam.transform.right * direction.x +
                cam.transform.forward * direction.y +
                Vector3.up * direction.y * ShootVerticalBoost;

            rb.AddForce(worldDir * ShootForceMultiplier, ForceMode.Impulse);
        }

        var disappearAfterDelay = _currentBasketBall.GetComponent<DisappearAfterDelay>();
        disappearAfterDelay.TriggerDisappear();
        _currentGameplayState = GameplayState.HasShot;
        _shootBallCooldown = ShootBallDelay;
    }

    private void _handleGameplayInputs()
    {
        if (_currentGameplayState == GameplayState.HasShot)
        {
            _shootBallCooldown -= Time.deltaTime;
            if (_shootBallCooldown <= 0.0f)
            {
                _triggerNewBall();
            }
            return;
        }

        if (_currentBasketBall == null)
            return;

        if (_currentGameplayState == GameplayState.WaitingToShoot)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _currentGameplayState = GameplayState.CalibratingShoot;
                _calibrationStartTime = Time.time;
            }
        }
        else if (_currentGameplayState == GameplayState.CalibratingShoot)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 ballScreenPos = Camera.main.WorldToScreenPoint(_currentBasketBall.transform.position);
                Vector2 mouseScreenPos = Input.mousePosition;
                // Slingshot logic: shoot opposite to where the user dragged
                Vector2 dragDir = ballScreenPos - mouseScreenPos;
                // Add randomness proportional to how long player held past ShootStableDelay
                float overage = _calibrationOverage();
                dragDir += UnityEngine.Random.insideUnitCircle * overage * ShootInstabilityForceRandomness;
                _shootBall(dragDir);
            }
        }
    }

    private float _calibrationOverage()
    {
        return Mathf.Max(0.0f, Time.time - _calibrationStartTime - ShootStableDelay);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnteredRimDetector.OnBallScored += EnteredRimDetector_OnBallScored;
        _currentGameState = GameState.Menu;
    }

    // Distance thresholds (from camera) -> points awarded.
    // Must be ordered ascending by distance; the LAST entry whose threshold
    // is below the shot distance wins, so further shots score more.
    private static readonly List<Tuple<float, int>> _scoresToDistance = new List<Tuple<float, int>>
    {
        Tuple.Create(0.0f, 10),
        Tuple.Create(2.0f, 20),
        Tuple.Create(3.0f, 30),
        Tuple.Create(4.0f, 40),
        Tuple.Create(5.0f, 50),
    };

    private void EnteredRimDetector_OnBallScored(float distance)
    {
        var matching = _scoresToDistance.Where(el => el.Item1 < distance);
        if (!matching.Any())
            return;
        var foundScore = matching.Last();
        _currentScore += foundScore.Item2;
        GameSingleton.Instance.SoundService.PlayScoreBall();
        MoveBasketRim();
        // TODO trigger anim
    }

    private void _startGame()
    {
        _currentGameState= GameState.InGame;
        _currentScore = 0;
        _triggerNewBall();
        _time = 60.0f;
    }
    private float _time;

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
            _startGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentGameState == GameState.Menu)
        {
            _handleMenuInputs();
        }
        else if (_currentGameState == GameState.EndScreen)
        {
            _handleEndScreenInputs();
        }
        else if (_currentGameState == GameState.InGame)
        {
            _handleGameplayInputs();
        }
        ScoreText.text = _currentScore.ToString("D4"); // Padded to be length 4
        TimerText.text = TimeSpan.FromSeconds(_time).ToString(@"m\:ss");
        if (_currentBasketBall != null && _currentGameplayState == GameplayState.CalibratingShoot)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(_currentBasketBall.transform.position);
            GameSingleton.Instance.UIService.DrawUILine(LineShootBall, screenPos, Input.mousePosition);
            LineShootBall.gameObject.SetActive(true);
            if (_currentBallShake != null)
            {
                _currentBallShake.Amplitude = Mathf.Clamp01(_calibrationOverage() * ShootInstabilityAmplitudePerSecond);
            }
        }
        else
        {
            if (LineShootBall != null)
            {
                LineShootBall.gameObject.SetActive(false);
            }
            if (_currentBallShake != null)
            {
                _currentBallShake.Amplitude = 0.0f;
            }
        }
        if (_currentGameState == GameState.InGame)
        {
            if (_time < 0.0f)
            {
                EndUIScore.text = ScoreText.text;
                _currentGameState = GameState.EndScreen;
                _clearCurrentBall();
            }
            _time -= Time.deltaTime;
        }

        if (_currentGameState == GameState.InGame)
        {
            _uiSvc.ShowGameUI();
        }
        if (_currentGameState != GameState.InGame)
        {
            _uiSvc.HideGameUI();
        }
        if (_currentGameState == GameState.Menu)
        {
            _uiSvc.ShowMenuUI();
        }
        if (_currentGameState != GameState.Menu)
        {
            _uiSvc.HideMenuUI();
        }
        if (_currentGameState == GameState.EndScreen)
        {
            _uiSvc.ShowEndUI();
        }
        if (_currentGameState != GameState.EndScreen)
        {
            _uiSvc.HideEndUI();
        }

    }
}
