using System;
using UnityEngine;

/// <summary>
/// interfaz que debe implementar cualquier clase que pueda ser retenida
/// </summary>
public interface IRestrainable
{
    event Action<bool> OnRestrainedChanged;

    Transform Root { get; }

    /// <summary> Indica si el enemigo está actualmente restringido. </summary>
    bool IsRestrained { get; }

    /// <summary> Intenta aplicar la restricción al enemigo. </summary>
    bool TryRestrain();

    /// <summary> Libera al enemigo sin aplicar fuerza. </summary>
    void Release();

    /// <summary> Lanza al enemigo en una dirección específica. </summary>
    void Launch(Vector3 force);

}
