using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Localizador de servicios para el contexto de batalla.
/// Permite registrar y acceder a instancias de servicios mediante su tipo, tanto globales como especificas para 
/// un determinado objeto.
/// Es una forma sencilla y desacoplada de resolver manejo de dependencias.
/// Se inicializa temprano gracias a el atributo <see cref="DefaultExecutionOrder"/> con prioridad -300.
/// Tutorial detallado del tema (esta en ingles y es nivel intermedio): https://www.youtube.com/watch?v=D4r5EyYQvwY 
/// </summary>
[DefaultExecutionOrder(-300)]
public class BattleServiceLocator : MonoBehaviour, IServiceLocator
{
    public static BattleServiceLocator Instance { get; private set; }

    readonly Dictionary<Type, object> services = new();
    readonly Dictionary<object, Dictionary<Type, object>> objectServices = new();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Evita duplicados
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Opcional: persiste entre escenas
    }
    /// <summary>
    /// Registra un servicio T
    /// </summary>
    public void Register<T>(T service) where T : class => services[typeof(T)] = service;
    /// <summary>
    /// Se obtiene un servicio del tipo de T
    /// </summary>
    public T Get<T>() where T : class => services.TryGetValue(typeof(T), out var service) ? (T)service : null;

    /// <summary>
    /// Registra un servicio T para un determinado objeto
    /// </summary>
    public void RegisterFor<T>(object obj, T service) where T : class
    {
        if (!objectServices.ContainsKey(obj))
            objectServices[obj] = new Dictionary<Type, object>();

        objectServices[obj][typeof(T)] = service;
    }
    /// <summary>
    /// Se obtiene un servicio T para un determinado objeto
    /// </summary>
    public T GetFor<T>(object obj) where T : class
        => (objectServices.TryGetValue(obj, out var services) && services.TryGetValue(typeof(T), out var service))
            ? (T)service
            : Get<T>();
}
