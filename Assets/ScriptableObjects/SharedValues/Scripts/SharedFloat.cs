using UnityEngine;

[CreateAssetMenu(menuName = "Shared Values/Float")]
public class SharedFloat : SharedValue<float>
{
    [System.Serializable]
    public class ReadOnly
    {
        [SerializeField] SharedFloat sharedFloat;
        public float Value => sharedFloat != null ? sharedFloat.Value : 0f;
    }
}
