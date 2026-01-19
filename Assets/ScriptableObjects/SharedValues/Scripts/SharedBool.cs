using UnityEngine;

[CreateAssetMenu(menuName = "Shared Values/Bool")]
public class SharedBool : SharedValue<bool> 
{
    [System.Serializable]
    public class ReadOnly
    {
        [SerializeField] SharedBool sharedBool;
        public bool Value => sharedBool != null && sharedBool.Value;
    }
}
