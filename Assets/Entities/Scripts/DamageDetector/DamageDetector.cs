using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageDetector : MonoBehaviour
{
    [SerializeField] string tagDetector = "Damange";
    [SerializeField] string tagBodyPartDestructible = "Destructible";
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
            if (!bodyPart.TryGetComponent(out DamageDetectorInBodyPart detectorInBodyPart))
                detectorInBodyPart = bodyPart.AddComponent<DamageDetectorInBodyPart>();

            detectorInBodyPart.DetectDamange += OnDamageDetected;
            detectorInBodyPart.tagDetector = tagDetector;
        }
    }
    private void OnDisable() => UnsubcribeDetectDamanges();
    void UnsubcribeDetectDamanges()
    {
        foreach (var bodyPart in ragdollToggle.GetBodyParts)
        {
            if (bodyPart.TryGetComponent(out DamageDetectorInBodyPart detectorInBodyPart))
                detectorInBodyPart.DetectDamange -= OnDamageDetected;
        }
    }
    void OnDamageDetected(DamageDetectorInBodyPart bodyPart, float amount, Vector3 position)
    {
        if (amount <= 0f || !bodyPart.CompareTag(tagBodyPartDestructible)) return;
        particleSystemPool?.Play(bloodCatalog, position, bodyPart.transform.rotation);

        if(!healthHandler.NoHealth)
            healthHandler.ChangeHealth(-amount);

        if (healthHandler.NoHealth)
            ReducePart(bodyPart);
    }

    void ReducePart(DamageDetectorInBodyPart bodyPart)
    {
        if (bodyPart.transform.localScale == Vector3.zero) return;

        bodyPart.transform.localScale = Vector3.zero;
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
