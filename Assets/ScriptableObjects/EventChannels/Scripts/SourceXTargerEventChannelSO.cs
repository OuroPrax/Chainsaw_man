using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/SourceTargetEventChannel")]
public class SourceXTargetEventChannelSO : ScriptableObject
{
    public UnityAction<Transform, Transform> OnEventRaised;

    public void RaiseEvent(Transform source, Transform target)
    {
        OnEventRaised?.Invoke(source, target);
    }
}
