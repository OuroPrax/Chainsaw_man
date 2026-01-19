using UnityEngine;

/// <summary>
/// Scriptable Object que contiene los datos necesarios de una clasificacion de Rango
/// </summary>
[CreateAssetMenu(menuName = "Classifications/Classification")]
public class ClassificationSO : ScriptableObject
{
    public string Name;
    public string description;
    public Sprite Icon;
}