using System;
using UnityEngine;


/// <summary>
/// Logica de parte con vida utilizada en el miniboss, posee la logica necesaria para recibir dano de avisadores <see cref="damageAdvaisers"/> 
/// y cuando muere se escala a cero.
/// Posee metodo para resetear su estado <see cref="ResetLimb"/> y para inicialisarze en base datos <see cref="Init(LimbData)"/>.
/// Posee un campo del tipo enum <see cref="LimbData.Part"/> para indicar el tipo de parte que es.
/// </summary>
public class Limb : MonoBehaviour
{
    [SerializeField] ParticleEffectCatalog particleEffectCatalog;
    public event Action<Limb> OnDead;
    public event Action<Limb, float/*previous*/, float/*current*/> OnHealthChanged;
    public bool IsAlive => currentHealth > 0;
    public LimbData.Part Part;

    [SerializeField] DamageAdviser[] damageAdvaisers;

    float currentHealth, maxHealth;
    Vector3 originalLocalPos;
    Quaternion originalLocalRot;
    private void Awake()
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        foreach (var item in damageAdvaisers)
        {
            item.OnDamageDetected += TakeDamage;
        }
    }
    private void OnDestroy()
    {
        foreach (var item in damageAdvaisers)
        {
            item.OnDamageDetected -= TakeDamage;
        }
    }
    public void Init(LimbData limbData)
    {
        maxHealth = limbData.health;
        currentHealth = maxHealth;
    }
    public void ResetLimb()
    {
        currentHealth = maxHealth;
        transform.localScale = Vector3.one;
        transform.SetLocalPositionAndRotation(originalLocalPos, originalLocalRot);
    }
    public void TakeDamage(float amount, Vector3 position, DamageAdviser adviser)
    {
        if(amount<=0) return;

        BattleServiceLocator.Instance.Get<IParticleSystemPool>().Play(particleEffectCatalog, position, adviser.transform.rotation);

        var previous = currentHealth;
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        OnHealthChanged?.Invoke(this, previous, currentHealth);
        if (currentHealth <= 0)
        {
            transform.localScale = Vector3.zero;
            OnDead?.Invoke(this);
        }
    }
}
