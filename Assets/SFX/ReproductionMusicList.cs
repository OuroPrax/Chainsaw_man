using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductionMusicList : MonoBehaviour
{
    [SerializeField] SimpleMusicCrossfade musicSource;
    [SerializeField] AudioClip[] audioClips;

    int _currentIndex = 0;

    void OnEnable()
    {
        musicSource.OnCompleted += CompletedMusic;
    }

    void CompletedMusic()
    {
        RandomPlayMusic();
    }

    void RandomPlayMusic()
    {
        int newIndex = GetRandomExcluding(0, audioClips.Length, _currentIndex);
        musicSource.ChangeAudioClip(audioClips[newIndex]);
        musicSource.Play();
        _currentIndex = newIndex;
    }

    void Update()
    {
        
    }

    int GetRandomExcluding(int minInclusive, int maxExclusive, int exclude)
    {
        int n = maxExclusive - minInclusive;

        if (exclude < minInclusive || exclude >= maxExclusive)
            return UnityEngine.Random.Range(minInclusive, maxExclusive);

        if (n <= 1)
            throw new System.ArgumentException("No hay valores disponibles.");

        int r = UnityEngine.Random.Range(0, n - 1);

        int result = minInclusive + r;
        if (result >= exclude) result += 1;
        return result;
    }
}
