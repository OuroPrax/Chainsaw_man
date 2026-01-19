using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Poder del jugador, cuando pierde vida la vida que tenia antes de perder se registra <see cref="healthSnapshot"/> y empieza una corrutina para empezar a 
/// reducir <see cref="healthSnapshot"/> hasta llegar a la vida actual del jugador, si el jugador mata a un enemigo puede recuperar hasta el
/// el valor actual de <see cref="healthSnapshot"/>
/// Si el jugador recibe otro golpe mientras se reduce <see cref="healthSnapshot"/> se vuelve actualizar el valor y vuelve a 
/// empezar reducir vida
/// </summary>
public class RecoverHealthPower : MonoBehaviour
{
    [SerializeField] float healthToRecoverPerKill = 3;
    [SerializeField] SourceXTargetEventChannelSO playerKilledEnemyEventChannel;
    public event Action<float> OnRecoverChanged;
    public float RateValue => healthSnapshot / healthHandler.MaxHealth;

    [SerializeField] HealthHandler healthHandler;
    [SerializeField] float waitTimeToLose = 3f;
    [SerializeField] float healthLostPerSecond = 5f;
    float healthSnapshot;
    Coroutine reduceRecoberCoroutine;

    #region unity lifecycle
    private void OnEnable()
    {
        healthHandler.OnHealthChanged += HandleHealthChanged;
        playerKilledEnemyEventChannel.OnEventRaised += ResolveEnemyKilled;
    }
    private void OnDisable()
    {
        healthHandler.OnHealthChanged -= HandleHealthChanged;
        playerKilledEnemyEventChannel.OnEventRaised -= ResolveEnemyKilled;
    }
    void ResolveEnemyKilled(Transform _, Transform enemy) => RecoverHealth(healthToRecoverPerKill);
    void RecoverHealth(float amount)
    {
        amount = Mathf.Min(healthSnapshot - healthHandler.CurrentHealth, amount);
        if (amount <= 0f) return;
        healthHandler.ChangeHealth(amount);
    }

    void HandleHealthChanged(float previous, float current)
    {
        if (current < previous) // si perdio vida, registramos la vida previa y empezamos una corrutina para llevarla a la actual
        {
            healthSnapshot = previous;
            OnRecoverChanged?.Invoke(healthSnapshot);
            if (reduceRecoberCoroutine != null) StopCoroutine(reduceRecoberCoroutine);
            reduceRecoberCoroutine = StartCoroutine(ReduceRecoverAmount());
        }
        else if (current > healthSnapshot) //si la vida actual es mayor a la que tenemos registrada
        {
            if (reduceRecoberCoroutine != null)
            {
                StopCoroutine(reduceRecoberCoroutine);
                reduceRecoberCoroutine = null;
            };
        }
    }
    #endregion

    IEnumerator ReduceRecoverAmount()
    {
        yield return new WaitForSeconds(waitTimeToLose); // esperamos un tiempo antes de perder la vida registrada

        while (healthHandler.CurrentHealth < healthSnapshot) // mientras la vida registrada sea mayor a la actual, la vamos reduciendo
        {
            healthSnapshot -= healthLostPerSecond * Time.deltaTime;
            OnRecoverChanged?.Invoke(healthSnapshot);
            yield return null;
        }
        healthSnapshot = healthHandler.CurrentHealth;
        OnRecoverChanged?.Invoke(healthSnapshot);
        reduceRecoberCoroutine = null;
    }
}
