using UnityEngine;

/// <summary>
/// Esta clase simplemente obtiene el puntaje final obtenido y lo muestra en la UI de Resultados
/// </summary>
public class ResultHandler: MonoBehaviour
{
    [SerializeField] ScoreToClassificationSO scoreToClassificationSO;
    [SerializeField] SharedInt currentScore;
    [SerializeField] SharedInt maxScore;
    [SerializeField] SharedFloat time;
    [SerializeField] int healthPoints;
    [SerializeField] float timerMultiplier;
    [Header("UI")]
    [SerializeField] UIResultPanel uIResultPanel;

    public void ShowResult()
    {        
        var healthRate = BattleServiceLocator.Instance.Get<PlayerHandler>().GetComponent<IHealth>().HealthRate;
        int currentResult = (int) ((currentScore.Value + healthPoints * healthRate) / (Mathf.Max(1f,time.Value) * timerMultiplier));
        int maxScorePrevious = maxScore.Value;
        bool isNewMaxScore = currentResult > maxScorePrevious;

        if (isNewMaxScore)
            maxScore.Value = currentResult;

        ClassificationSO classificationSO = scoreToClassificationSO.GetClassificationForScore(currentResult);

        uIResultPanel.gameObject.SetActive(true);
        uIResultPanel.SetResult(currentResult, maxScorePrevious, isNewMaxScore, classificationSO);
    }
}
