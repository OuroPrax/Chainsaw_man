using System.Collections;
using UnityEngine;

public class ChainsawSoundController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip idleLoopClip;
    [SerializeField] float idlePitch = 1f;
    [SerializeField] float idleVolume = 1f;
    [SerializeField] float lerpSpeed = 5f;
    Coroutine lerpPitchCoroutine;

    private void Start() => SetupSource(audioSource);
    void SetupSource(AudioSource source)
    {
        source.clip = idleLoopClip;
        source.loop = true;
        source.pitch = idlePitch;
    }
    public void PlaySound(AudioClip audioClip, float volume = 1f, float pitch = 1f)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        if (lerpPitchCoroutine != null) StopCoroutine(lerpPitchCoroutine);
        StartCoroutine(LerpPitch(volume, pitch));
    }
    public void StopSound()
    {
        audioSource.clip = idleLoopClip;
        audioSource.Play();
        if (lerpPitchCoroutine != null) StopCoroutine(lerpPitchCoroutine);
        StartCoroutine(LerpPitch(idleVolume, idlePitch));
    }
    IEnumerator LerpPitch(float targetVolume, float targetPitch)
    {
        float startVolume = audioSource.volume;
        float startPitch = audioSource.pitch;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * lerpSpeed;
            float lerpT = Mathf.Clamp01(t);
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, lerpT);
            audioSource.pitch = Mathf.Lerp(startPitch, targetPitch, lerpT);
            yield return null;
        }

        audioSource.volume = targetVolume;
        audioSource.pitch = targetPitch;
    }
}