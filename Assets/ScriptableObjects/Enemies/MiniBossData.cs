using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/MiniBoss")]
public class MiniBossData : EnemySO
{
    public LimbData[] limbsDatas;
}
[Serializable]
public struct LimbData
{
    public Part part;
    public int health;

    public enum Part { Head, LeftArm, RightArm, LeftLeg, RightLeg } 
}


