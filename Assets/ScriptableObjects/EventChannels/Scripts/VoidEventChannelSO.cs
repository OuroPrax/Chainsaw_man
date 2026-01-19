using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Clase para canal de evento en forma de ScriptableObject.
/// Permite de forma rapida conectar clases que emiten o escuchan el mismo evento, por inspector.
/// Posee un metodo <see cref="RaiseEvent"/> para emitir el evento.
/// </summary>
[CreateAssetMenu(menuName = "Events/VoidEventChannel")]
public class VoidEventChannelSO : ScriptableObject
{
    public UnityAction OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
