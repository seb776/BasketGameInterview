using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasketBallBounceSound : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var singleton = GameSingleton.Instance;
        if (singleton == null || singleton.SoundService == null)
            return;
        singleton.SoundService.PlayBounceBall();
    }
}
