using UnityEngine;

[ExecuteAlways]
public class Shake : MonoBehaviour
{
    [Range(0, 1)]
    public float Amplitude;
    void Update()
    {
        transform.GetChild(0).transform.localPosition = (new Vector3(
            Mathf.Sin(Time.realtimeSinceStartup * 18.5f),
            Mathf.Sin(Time.realtimeSinceStartup * 15.5f),
            Mathf.Sin(Time.realtimeSinceStartup * 25.5f)) * 0.00625f
            + new Vector3(
            Mathf.Sin(Time.realtimeSinceStartup * 38.5f),
            Mathf.Sin(Time.realtimeSinceStartup * 35.5f),
            Mathf.Sin(Time.realtimeSinceStartup * 35.5f)) * 0.0025f) * Amplitude;
    }
}
