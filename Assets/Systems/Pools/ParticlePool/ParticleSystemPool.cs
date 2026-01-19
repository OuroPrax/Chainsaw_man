using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Clase que genera pool de sistemas de particulas por cada catalogo <see cref="ParticleEffectCatalog"/> 
/// buscando optimizar por medio de la reutilizacion. 
/// Por cada <see cref="ParticleEffectCatalog"/> se genera un pool independiente 
/// que reutiliza instancias de <see cref="ParticleSystem"/> generadas a partir de dicho catalogo. 
/// Al iniciar, precarga una instancia de cada prefab del catálogo para garantizar variedad sin duplicados. 
/// Cada instancia se auto-retorna al pool mediante <see cref="ParticleReturnToPool"/>
/// Uiliza el sistema de pool de Unity:<see cref="ObjectPool{T}"/>.
/// Se ejecuta temprano (<see cref="DefaultExecutionOrder"/> -100) para estar disponible antes de que otros sistemas lo invoquen.
/// </summary>
[DefaultExecutionOrder(-100)]
public class ParticleSystemPool : MonoBehaviour, IParticleSystemPool
{
    [SerializeField] ParticleEffectCatalog[] baseCatalog;
    Dictionary<ParticleEffectCatalog, ObjectPool<ParticleSystem>> pools = new();

    private void Awake()
    {
        foreach (var catalog in baseCatalog)
            TryAddCatalog(catalog);
    }

    #region pool generation
    ObjectPool<ParticleSystem> GenerateCatalogPool(ParticleEffectCatalog catalog)
    {
        // Crear el pool
        var pool = new ObjectPool<ParticleSystem>(
            () => CreateInstance(catalog),
            OnGet,
            OnRelease,
            OnDestroy_,
            false,
            0,
            100
        );
        // precargamos uno de cada PS para que de base no tenga repetidos
        foreach (var prefab in catalog.particlesGOs)
        {
            var ps = Instantiate(prefab).GetComponent<ParticleSystem>();
            var returner = ps.gameObject.AddComponent<ParticleReturnToPool>();
            returner.Initialize(() => pool.Release(ps));
            ps.gameObject.SetActive(false);
            pool.Release(ps);
        }
        return pool;
    }
    ParticleSystem CreateInstance(ParticleEffectCatalog catalog)
    {
        var ps = Instantiate(catalog.particlesGOs[Random.Range(0, catalog.particlesGOs.Length)]).GetComponent<ParticleSystem>();
        var returner = ps.gameObject.AddComponent<ParticleReturnToPool>();
        returner.Initialize(() => pools[catalog].Release(ps));
        ps.gameObject.SetActive(false);
        return ps;
    }
    void OnGet(ParticleSystem ps)
    {
        ps.gameObject.SetActive(true);
        ps.Play();
    }
    void OnRelease(ParticleSystem ps) => ps.gameObject.SetActive(false);
    void OnDestroy_(ParticleSystem ps) => Destroy(ps.gameObject);
    #endregion

    /// <summary>
    /// Funcion para saber si un determinado catalogo ya tiene pool generada
    /// </summary>
    public bool HasPoolForCatalog(ParticleEffectCatalog catalog) => pools.ContainsKey(catalog);

    /// <summary>
    /// Funcion para agregar un catalogo nuevo, se utiliza para ya tener generada la pool antes de ser usada, por las dudas
    /// </summary>
    /// <param name="catalog"></param>
    /// <returns>Si no existia devuelve true indicando que se agrego, sino devuelve false</returns>
    public bool TryAddCatalog(ParticleEffectCatalog catalog)
    {
        if (!HasPoolForCatalog(catalog))
        {
            pools[catalog] = GenerateCatalogPool(catalog);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Ejecuta un efecto de particulas del catalogo pasado, en una posicion y rotacion indicadas, si por algun motivo no existe
    /// la pool del catalogo, la genera
    /// </summary>
    /// <param name="catalog"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void Play(ParticleEffectCatalog catalog, Vector3 position, Quaternion rotation)
    {
        if (!pools.TryGetValue(catalog, out var pool))
        {
            pool = GenerateCatalogPool(catalog);
            pools[catalog] = pool;
        }

        var ps = pool.Get();
        ps.transform.SetPositionAndRotation(position, rotation);
    }
}