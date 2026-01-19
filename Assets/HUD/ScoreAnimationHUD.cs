using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreAnimationHUD : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] IntEventChannelSO scoreChangedEventChannel;

    private void OnEnable() => scoreChangedEventChannel.OnEventRaised += UpdateScore;
    private void OnDisable() => scoreChangedEventChannel.OnEventRaised -= UpdateScore;

    void UpdateScore(int amount) => animator.SetTrigger("Gain");
}
