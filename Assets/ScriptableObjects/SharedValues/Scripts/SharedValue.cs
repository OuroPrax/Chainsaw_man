using System;
using UnityEngine;

/// <summary>
/// Clase base genérica para valores compartidos en forma de ScriptableObject.
/// Permite almacenar un valor del tipo a implementar que puede ser observado por otros componentes.
/// Al cambiar el valor, se dispara el evento <see cref="OnValueChanged"/> para notificar a los observadores.
/// Ideal para desacoplar lógica entre sistemas y facilitar la depuración visual desde el editor.
/// </summary>
public abstract class SharedValue<T> : ScriptableObject
{
    [SerializeField] protected T value;

    public event Action<T> OnValueChanged;

    /// <summary>
    /// Propiedad pública para acceder y modificar el valor.
    /// Al asignar un nuevo valor distinto, se dispara <see cref= "OnValueChanged"/>
    /// </summary>
    public virtual T Value
    {
        get => value;
        set
        {
            if (!Equals(this.value, value))
            {
                this.value = value;
                OnValueChanged?.Invoke(this.value);
            }
        }
    }
}
