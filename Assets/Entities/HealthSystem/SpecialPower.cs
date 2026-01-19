using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class SpecialPower: MonoBehaviour
{
    [SerializeField] PlayableDirector specialDirector;
    [SerializeField] FloatEventChannelSO SpecialRateChangedEventChannel;
    [SerializeField] IntEventChannelSO scoreGainedEventChannel;
    [SerializeField] float ratePerPoint;
    float rate;
    bool _canGain = true;

    private void OnEnable() => scoreGainedEventChannel.OnEventRaised += GainRate;
    private void OnDisable() => scoreGainedEventChannel.OnEventRaised -= GainRate;
    void GainRate(int scoreGained)
    {
        if (!_canGain) return;
        rate = Mathf.Clamp01(rate + scoreGained * ratePerPoint);
        SpecialRateChangedEventChannel.RaiseEvent(rate);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && rate >= 1f)
        {
            specialDirector.Play();
            //StartCoroutine(Dash());
            rate = 0f;
            SpecialRateChangedEventChannel.RaiseEvent(rate);
        }
    }
    IEnumerator Dash()
    {
        var rb = GetComponent<Rigidbody>();
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            yield return null;
            rb.velocity = transform.forward * 50f;
            rb.AddForce(5f * Time.deltaTime * transform.forward, ForceMode.Impulse);
        }
        rb.velocity = Vector3.zero;
    }

    public void ChangeCanGain(bool value) => _canGain = value;
}