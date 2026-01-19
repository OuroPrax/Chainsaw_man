using UnityEngine;

/// <summary>
/// Controlador de puntaje que actualiza el puntuacion actual y guarda el maximo alcanzado si es que cambia.
/// Escucha eventos de enemigos derrotados y suma el puntaje correspondiente.
/// Posee un metodo para resetear la puntuacion actual del jugador
/// </summary>
public class ScoreHandler : MonoBehaviour
{
    [SerializeField] IntEventChannelSO scoreGainedEventChannel;
    [SerializeField] SharedInt currentScore;
    [SerializeField] SharedInt maxScore;
    [SerializeField] SourceXTargetEventChannelSO playerKilledEnemyEventChannel;
    [SerializeField] bool Debug;

    private void Start()
    {
        if(Debug)
            ResetCurrentScore();
        maxScore.Value = PlayerPrefs.GetInt("MaxScore", 0);
    }

    private void OnEnable()
    {
        playerKilledEnemyEventChannel.OnEventRaised += ResolveEnemyKilled;
        maxScore.OnValueChanged += SaveNewMaxScore;
    }
    private void OnDisable()
    {
        playerKilledEnemyEventChannel.OnEventRaised -= ResolveEnemyKilled;
        maxScore.OnValueChanged -= SaveNewMaxScore;
    }
    void ResolveEnemyKilled(Transform player, Transform enemy)
    {
        if (!enemy.TryGetComponent(out ScoreValue scoreValue)) return;

        var scoreToAdd = scoreValue.Score;
        scoreGainedEventChannel.RaiseEvent(scoreToAdd);
        currentScore.Value += scoreToAdd;
    }
    void SaveNewMaxScore(int score)
    {
        PlayerPrefs.SetInt("MaxScore", score);
        PlayerPrefs.Save();
    }

    public void ResetCurrentScore() => currentScore.Value = 0;
}
