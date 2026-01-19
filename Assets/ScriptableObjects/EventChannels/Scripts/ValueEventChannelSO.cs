using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Clase abstracta generica para canal de evento que lanza un valor, en forma de ScriptableObject.
/// Permite de forma rapida conectar clases que emiten o escuchan el mismo evento, por inspector.
/// Posee un metodo <see cref="RaiseEvent"/> para emitir el evento.
/// </summary>
public abstract class ValueEventChannelSO<T> : ScriptableObject
{
    public UnityAction<T> OnEventRaised;

    public void RaiseEvent(T value)
    {
        OnEventRaised?.Invoke(value);
    }
}