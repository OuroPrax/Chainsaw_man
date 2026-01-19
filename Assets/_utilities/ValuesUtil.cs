using UnityEngine;

/// <summary>
/// Clase estatica utilizada para facilitar la carga y guardado de valores
/// </summary>
public static class ValuesUtil
{
    public static void SaveAudioPercentage(GameAudioType audioType, float value)
    {
        string audioString = audioType.ToString();
        PlayerPrefs.SetFloat(audioString + "Audio", value);
    }
    public static float LoadAudioPercentage(GameAudioType audioType) => PlayerPrefs.GetFloat(audioType.ToString() + "Audio", 100);
    public static float GetVolumeForPencentage(float percentage) => Mathf.Log10(percentage / 100) * 25f;

    public static float LoadSensitivity() => PlayerPrefs.GetFloat("Sensitivity", 1f);
    public static void SaveSensitivity(float value) => PlayerPrefs.SetFloat("Sensitivity", value);
}
