using TMPro;
using UnityEngine;

public class UIResultPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI maxScoreText;
    [SerializeField] TextMeshProUGUI newMaxScoreText;
    [SerializeField] TextMeshProUGUI letterText;

    public void SetResult(int score, int maxScore, bool isNewMaxScore, ClassificationSO classificationSO)
    {
        scoreText.text = score.ToString();
        maxScoreText.text = maxScore.ToString();
        newMaxScoreText.gameObject.SetActive(isNewMaxScore);
        letterText.text = classificationSO.Name.ToString();
    }
}
