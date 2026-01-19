using UnityEngine;

[CreateAssetMenu(menuName = "Shared Values/Int")]
public class SharedInt : SharedValue<int> 
{
    [System.Serializable]
    public class ReadOnly
    {
        [SerializeField] SharedInt sharedInt;
        public int Value => sharedInt != null ? sharedInt.Value : 0;
    }
}
