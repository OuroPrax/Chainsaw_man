using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsawSFX : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] ComboController comboController;
    [SerializeField] List<AudioBodyInfo> ChainsawComboSound;

    void OnEnable()
    {
        comboController.OnComboStarted += StartChainsawComboSound;
        comboController.OnComboEnded += DisableSlash;
    }

    void OnDisable()
    {
        comboController.OnComboStarted -= StartChainsawComboSound;
        comboController.OnComboEnded -= DisableSlash;        
    }

    void StartChainsawComboSound(int step)
    {
        int currentStep = step - 1;

        Debug.Log($"SlashesCombo.Count: {ChainsawComboSound.Count - 1} | currentStep: {currentStep} - {ChainsawComboSound.Count - 1 < currentStep}");

        if (ChainsawComboSound.Count - 1 < currentStep) return;

        ChainsawComboSound[currentStep].audioSource.PlayOneShot(ChainsawComboSound[currentStep].clip);
    }

    void DisableSlash()
    {

    }
}


[System.Serializable]
public class AudioBodyInfo
{
    public AudioClip clip;
    public AudioSource audioSource;
}
