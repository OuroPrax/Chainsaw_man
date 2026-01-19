using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

/// <summary>
/// Clase que posee un pool de audiosources, es utilizada por toda clase que requiera reproducir un sonido
/// (en este proyecto para un sonido SFX) 
/// </summary>
public class SoundPoolHandler : MonoBehaviour, ISoundPoolHandler
{
    [SerializeField] int defaultCapacity = 10;
    [SerializeField] int maxSize = 30;
    ObjectPool<AudioSource> pool;

    #region pool
    private void Awake()
    {
        pool = new(
            CreatePooledItem,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            true,
            defaultCapacity,
            maxSize
        );
    }
    AudioSource CreatePooledItem()
    {
        var go = new GameObject("PooledAudioSource");
        go.transform.parent = transform;
        var source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.gameObject.SetActive(false);
        return source;
    }
    void OnTakeFromPool(AudioSource source) => source.gameObject.SetActive(true);
    void OnReturnedToPool(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.transform.localPosition = Vector3.zero;
        source.gameObject.SetActive(false);
    }
    void OnDestroyPoolObject(AudioSource source) => Destroy(source.gameObject);
    void ReturnToPool(AudioSource source) => pool.Release(source);
    #endregion

    /// <summary>
    /// Se llama a este metodo cuando se quiere reproducir un sonido.
    /// Se obtiene un audiosource de la pool para reproducirlo, cuando el sonido se completa el audiosource vuelve a la pool.
    /// En base al pitch y el largo del sonido se obtiene cuando debe regresar a la pool
    /// </summary>
    /// <param name="clip">Sonido a reproducir</param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <param name="position">si no se setea una posicion se considera sonido 2D</param>
    public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f, Vector3? position = null)
    {
        if (clip == null) return;

        var source = pool.Get();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        if(position == null) // si no hay posicion lo reproducimos como sonido 2D
        {
            source.spatialBlend = 0f;
        }
        else// si hay posicion, lo posicionamos y lo reproducimos como sonido 3D
        {
            source.spatialBlend = 1f;
            source.transform.position = position ?? transform.position;
        }
        source.Play();

        if (pitch <= 0f)
            Debug.LogWarning($"Pitch invalido ({pitch}), usando valor cero o negativo.");

        float duration = clip.length / Mathf.Max(.1f,pitch); // sacamos la duracion en base al largo del clip dividido el pitch
        StartCoroutine(ReleaseAfterDelay(source, duration));
    }
    /// <summary>
    /// Corrutina que espera un tiempo para volver a la pool un audiosource
    /// </summary>
    IEnumerator ReleaseAfterDelay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(source);
    }
}
