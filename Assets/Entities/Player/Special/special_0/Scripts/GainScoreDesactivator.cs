using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainScoreDesactivator : MonoBehaviour
{
    [SerializeField] SpecialPower specialPower;

    private void OnEnable() => specialPower.ChangeCanGain(false);
    private void OnDisable() => specialPower.ChangeCanGain(true);
}
