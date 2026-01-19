using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Scriptable Object utilizado para generar tablas de clasificacion en base al puntaje del jugador.
/// Utiliza OnValidate para reordenar de forma descendiente los puntajes a clasificacion facilitando el calculo
/// Posee un metodo <see cref="GetClassificationForScore"/> para obtener la clasificacion en base al puntaje
/// </summary>
[CreateAssetMenu(menuName = "Classifications/ScoreToClassification")]
public class ScoreToClassificationSO : ScriptableObject
{
    [SerializeField] ClassificationRequiredScore[] scoresToClassifications;
    [SerializeField] ClassificationSO lowestClassification;
    [Serializable]
    public class ClassificationRequiredScore
    {
        public int requiredScore;
        public ClassificationSO classification;
    }

    public ClassificationSO GetClassificationForScore(int score)
    {
        foreach (var item in scoresToClassifications)
        {
            if (item.requiredScore <= score)
                return item.classification;
        }
        return lowestClassification;
    }

    private void OnValidate() 
        => scoresToClassifications
            = scoresToClassifications
             .OrderByDescending(x => x.requiredScore)
             .ToArray();
}
