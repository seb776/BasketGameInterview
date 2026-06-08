using System.Collections;
using UnityEngine;

public class DisappearAfterDelay : MonoBehaviour
{
    public float DefaultDisappearDelay = 10.0f;
    public void TriggerDisappear()
    {
        StartCoroutine(_triggerDisappearCorout());
    }
    private IEnumerator _triggerDisappearCorout()
    {
        yield return new WaitForSeconds(DefaultDisappearDelay);
        Destroy(gameObject);
    }
}
