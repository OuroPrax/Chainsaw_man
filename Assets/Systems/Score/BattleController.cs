using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class BattleController: MonoBehaviour
{
    [SerializeField] BossController bossController;
    BattleEndCondictionMetChecker battleEndChecker;
    bool isPlaying;

    public void Retry() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void Exit() => Application.Quit();
    public void CursorLocked() => Cursor.lockState = CursorLockMode.Locked;
    public void CursorConfined() => Cursor.lockState = CursorLockMode.Confined;

    private void Start()
    {
        battleEndChecker = BattleServiceLocator.Instance.Get<BattleEndCondictionMetChecker>();
        battleEndChecker.OnEnded += EndBattle;
    }
    public void StartBattle()
    {
        if (isPlaying) return;

        bossController.Activate();
        BattleServiceLocator.Instance.Get<TimerHandler>().ResetTime();
        BattleServiceLocator.Instance.Get<TimerHandler>().StartTimer();
        BattleServiceLocator.Instance.Get<ScoreHandler>().ResetCurrentScore();
        isPlaying = true;
        battleEndChecker.StartCheck();
    }
    private void OnDestroy()
    {
        if(battleEndChecker != null)
        {
            battleEndChecker.OnEnded -= EndBattle;
            if (battleEndChecker.IsChecking)
                battleEndChecker.StopCheck();
        }
    }
    void EndBattle(BattleEndCondictionMetChecker checker)
    {
        isPlaying = false;
        BattleServiceLocator.Instance.Get<TimerHandler>().PauseTimer();
        if (checker.IsChecking)
            checker.StopCheck();

        BattleServiceLocator.Instance.Get<ResultHandler>().ShowResult();
    }
}