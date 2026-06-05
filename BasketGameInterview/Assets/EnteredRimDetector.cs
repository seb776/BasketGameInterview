using System;
using UnityEngine;

public class EnteredRimDetector : MonoBehaviour
{
    public event Action<float> OnBallScored;
    private void OnCollisionEnter(Collision collision)
    {
        // if ball
        // then check if ball center actually inside the collider (to discard just touching cases)
        if (OnBallScored != null)
        {
            OnBallScored(Vector3.Distance(Camera.main.transform.position, this.gameObject.transform.position));
        }
    }
}
