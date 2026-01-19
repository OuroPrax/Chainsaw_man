using UnityEngine;

/// <summary>
/// Script simple que pausa el juego al habilitarse y lo reanuda cuando no queda ninguna instancia activa.
/// </summary>
public class EnablePauser : MonoBehaviour
{
    static int count = 0;
    private void OnEnable()
    {
        Time.timeScale = 0f;
        count++;
    }
    private void OnDisable()
    {
        count = Mathf.Max(0, count - 1);
        if (count == 0)
            Time.timeScale = 1f;
    }
}
