using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _test_slash : MonoBehaviour
{
    [SerializeField] ComboController comboController;
    [SerializeField] List<SlashComboInfo> SlashesCombo;

    void OnEnable()
    {
        comboController.OnComboHit += EnableSlash;
        comboController.OnComboEnded += DisableSlash;
    }

    void Update()
    {
        
    }


    private SlashComboInfo currentSlashComboInfo;

    void EnableSlash(int step)
    {
        int currentStep = step - 1;
        if (SlashesCombo.Count - 1 < currentStep) return;

        currentSlashComboInfo = SlashesCombo[currentStep];

        foreach (SlashInfo slash in SlashesCombo[currentStep].SlashesInfo)
        {
            if (!slash.activate) continue;
            slash.slash.SetActive(true);
            slash.slash.transform.localPosition = slash.position;
            slash.slash.transform.localRotation = slash.rotation;
        }
    }

    void DisableSlash()
    {
        foreach (SlashInfo slash in currentSlashComboInfo.SlashesInfo)
        {
            slash.slash.SetActive(false);
        }
    }
}

[System.Serializable]
public class SlashComboInfo
{
    public List<SlashInfo> SlashesInfo;
}

[System.Serializable]
public class SlashInfo
{
    public bool activate;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject slash;
}