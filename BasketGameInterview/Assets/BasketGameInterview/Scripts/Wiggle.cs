using UnityEngine;

[ExecuteAlways]
public class Wiggle : MonoBehaviour
{
    private const float WIGGLE_DURATION = 0.6f;
    private const float MOVE_AMOUNT = 8f;
    private const float TILT_AMOUNT = 12f;
    private const float TILT_FREQUENCY = 4.5f;  // ~2-3 swings over the duration
    private const float TILT_DAMPING = 5f;     // higher = faster decay

    public float TimeBetweenWiggles = 3f;

    private float _timer;
    private float _wiggleTime;
    private bool _isWiggling;

    private Vector3 _startPos;
    private Quaternion _startRot;

    void Start()
    {
        _startPos = transform.localPosition;
        _startRot = transform.localRotation;
        StartWiggle();
    }

    void Update()
    {
        if (_isWiggling)
        {
            _wiggleTime += Time.deltaTime;
            float t = _wiggleTime / WIGGLE_DURATION;

            // Sine wave that starts and ends at 0 (one full arc)
            float wave = Mathf.Sin(t * Mathf.PI);

            // Damped oscillation: decaying envelope * sine gives 2-3 swings fading out
            float decay = Mathf.Exp(-TILT_DAMPING * t);
            float tilt = Mathf.Sin(t * Mathf.PI * TILT_FREQUENCY) * TILT_AMOUNT * decay;

            transform.localPosition = _startPos + Vector3.up * (wave * MOVE_AMOUNT);
            transform.localRotation = _startRot * Quaternion.Euler(0f, 0f, tilt);

            if (_wiggleTime >= WIGGLE_DURATION)
            {
                transform.localPosition = _startPos;
                transform.localRotation = _startRot;
                _isWiggling = false;
                _timer = 0f;
            }
        }
        else
        {
            _timer += Time.deltaTime;
            if (_timer >= TimeBetweenWiggles)
                StartWiggle();
        }
    }

    void StartWiggle()
    {
        _wiggleTime = 0f;
        _isWiggling = true;
    }
}