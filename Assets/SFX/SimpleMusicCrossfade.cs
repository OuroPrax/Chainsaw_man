using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class SimpleMusicCrossfade : MonoBehaviour
{
    [SerializeField] AudioSource audioSourceA; 
    [SerializeField] float fadeDuration = 2f;
    [SerializeField] bool unscaledTime = false;

    public event Action OnStarted;
    public event Action OnCompleted;

    private void Start()
    {
        if (audioSourceA.playOnAwake) Play();
    }

    public void Play()
    {
        StartCoroutine(PlayScheduledAndWait(audioSourceA, audioSourceA.clip));
    }

    public void ChangeAudioClip(AudioClip audioClip)
    {
        audioSourceA.clip = audioClip;
    }

    IEnumerator PlayScheduledAndWait(AudioSource source, AudioClip clip, double delaySeconds = 0.1)
    {
        double start = AudioSettings.dspTime + delaySeconds;
        source.clip = clip;
        source.PlayScheduled(start);
        double end = start + clip.length / source.pitch;
        while (AudioSettings.dspTime < end)
            yield return null;
        OnCompleted?.Invoke();
    }

    public void CrossfadeToB(SimpleMusicCrossfade nextAudio)
    {
        StopAllCoroutines();
        StartCoroutine(Crossfade(audioSourceA, nextAudio, fadeDuration));
    }

    IEnumerator Crossfade(AudioSource from, SimpleMusicCrossfade to, float duration)
    {
        if (to == null || from == null) yield break;
        if (!to.audioSourceA.isPlaying) to.Play();

        float timer = 0f;
        float startVolFrom = from.volume;
        float startVolTo = to.audioSourceA.volume;

        while (timer < duration)
        {
            timer += (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
            float t = timer / duration;
            from.volume = Mathf.Lerp(startVolFrom, 0f, t);
            to.audioSourceA.volume = Mathf.Lerp(startVolTo, 1f, t);
            yield return null;
        }

        from.volume = 0f;
        to.audioSourceA.volume = 1f;
        from.Stop(); // opcional
    }
}
