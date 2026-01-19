using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Maneja las distintas partes que pueden recibir daño en un personaje, utilizado en enemigos tipo zombie.
/// Redirige el dano a la vida central del enemigo.
/// Al morir, las partes que reciben dano se escalan a cero, generando la sensacion de desmembramiento.
/// </summary>
public class SimpleLimbsDamageDetector : MonoBehaviour
{
    [SerializeField] HealthHandler healthHandler;
    [SerializeField] RagdollToggle ragdollToggle;
    [SerializeField] ParticleEffectCatalog bloodCatalog;
    IParticleSystemPool particleSystemPool;
    struct LimbInfo
    {
        public Transform limbTransform;
        public Vector3 originalLocalPos;
        public Quaternion originalLocalRot;

        public LimbInfo(Transform limbTransform, Vector3 originalLocalPos, Quaternion originalLocalRot)
        {
            this.limbTransform = limbTransform;
            this.originalLocalPos = originalLocalPos;
            this.originalLocalRot = originalLocalRot;
        }
    }
    List<LimbInfo> initialLimbStates;
    private void Awake()
    {
        particleSystemPool = BattleServiceLocator.Instance.Get<IParticleSystemPool>();
        initialLimbStates = ragdollToggle.GetBodyParts.Select(bp => new LimbInfo(bp.transform, bp.transform.localPosition, bp.transform.localRotation)).ToList();
    }

    private void OnEnable()
    {
        SubcribeDetectDamanges();
        ragdollToggle.DesactiveRagdoll();
    }
    void SubcribeDetectDamanges()
    {
        foreach (var bodyPart in ragdollToggle.GetBodyParts)
        {
            if (!bodyPart.TryGetComponent(out DamageAdviser damageAdviser))
                damageAdviser = bodyPart.AddComponent<DamageAdviser>();

            damageAdviser.OnDamageDetected += ResolveDamageDetected;
        }
    }
    private void OnDisable() => UnsubcribeDetectDamanges();
    void UnsubcribeDetectDamanges()
    {
        foreach (var bodyPart in ragdollToggle.GetBodyParts)
        {
            if (bodyPart.TryGetComponent(out DamageAdviser damageAdviser))
                damageAdviser.OnDamageDetected -= ResolveDamageDetected;
        }
    }
    void ResolveDamageDetected(float amount, Vector3 position, DamageAdviser damageAdviser)
    {
        healthHandler.TakeDamage(amount, position);
        particleSystemPool?.Play(bloodCatalog, position, damageAdviser.transform.rotation);

        if (healthHandler.CurrentHealth > 0) return;

        if (TryGetComponent(out Collider col))
            col.enabled = false;

        if (damageAdviser.transform.localScale == Vector3.zero) return;

        damageAdviser.transform.localScale = Vector3.zero;
    }

    public void ResetParts()
    {
        foreach (var limb in initialLimbStates)
        {
            limb.limbTransform.localScale = Vector3.one;
            limb.limbTransform.SetLocalPositionAndRotation(limb.originalLocalPos, limb.originalLocalRot);
        }
    }
}
