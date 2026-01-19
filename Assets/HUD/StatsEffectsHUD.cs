using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsEffectsHUD : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] FloatEventChannelSO SpecialRateChangedEventChannel;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip specialSoundActivate;

    bool _isSpecial = false;

    private void OnEnable() => SpecialRateChangedEventChannel.OnEventRaised += UpdateSpecial;
    private void OnDisable() => SpecialRateChangedEventChannel.OnEventRaised -= UpdateSpecial;


    void UpdateSpecial(float currentRate)
    {
        if(currentRate == 1) SpecialActivate();
        else SpecialDesactivate();
    }

    void SpecialActivate()
    {
        if (!_isSpecial)
        {
            _isSpecial = true;
            audioSource.PlayOneShot(specialSoundActivate);
        }
        animator.SetBool("Special", true);
    }

    void SpecialDesactivate()
    {
        _isSpecial = false;
        animator.SetBool("Special", false);
    }
}
