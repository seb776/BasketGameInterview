using System.Collections;
using UnityEngine;

public class SoundService : MonoBehaviour
{
    public AudioSource MusicSource;
    public AudioClip ThrowBall;
    public AudioClip BounceBall;
    public AudioClip ScoreBall;

    public void MuteMusic()
    {
        MusicSource.mute = true;
    }

    public void UnmuteMusic()
    {
        MusicSource.mute = false;
    }
    public void PlayThrowBall()
    {
        _playSound(ThrowBall);
    }
    public void PlayBounceBall()
    {
        _playSound(BounceBall);
    }
    public void PlayScoreBall()
    {
        _playSound(ScoreBall);
    }
    private void _playSound(AudioClip clip)
    {
        if (MusicSource.mute)
            return;
        GameObject go = new GameObject("SFX_" + clip.name);
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        // Adding delay to avoid too close destroy
        StartCoroutine(DestroyAfter(go, clip.length + 1.0f));
    }

    private IEnumerator DestroyAfter(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(go);
    }
}
