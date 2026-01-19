using UnityEngine;

public interface ISoundPoolHandler
{
    void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f, Vector3? position = null);
}
