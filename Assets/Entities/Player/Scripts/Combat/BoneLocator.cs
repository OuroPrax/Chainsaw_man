using UnityEngine;

public class BoneLocator : MonoBehaviour
{
    [SerializeField] Transform neck;

    public Transform Neck => neck;
}
