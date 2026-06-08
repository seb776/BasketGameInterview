using System;
using UnityEngine;

public class EnteredRimDetector : MonoBehaviour
{
    public event Action<float> OnBallScored;

    private void OnTriggerEnter(Collider other)
    {
        // Filter: only count actual basketballs (marker component on the ball prefab)
        var ball = other.GetComponentInParent<BasketBallBounceSound>();
        if (ball == null)
            return;

        if (OnBallScored != null)
        {
            OnBallScored(Vector3.Distance(Camera.main.transform.position, this.gameObject.transform.position));
        }
    }
}
