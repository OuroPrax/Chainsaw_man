using UnityEngine;

public interface IParticleSystemPool
{
    public bool TryAddCatalog(ParticleEffectCatalog catalog);
    public void Play(ParticleEffectCatalog catalog, Vector3 position, Quaternion rotation);
}
