using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Bootstrapper para el contexto de batalla.
/// Se encarga de inicializar servicios esenciales y configurar parámetros globales.
/// Registra instancias en el <see cref="BattleServiceLocator"/> para permitir acceso desacoplado a sistemas como <see cref="IParticleSystemPool"/> y <see cref="ISoundPoolHandler"/>.
/// Tambien esta aplicando configuraciones de usuario como sensibilidad y volumen de audio, dado que el juego es pequeno
/// y esta todo en una sola escena.
/// Nos aseguramos una ejecucion temprana (<see cref="DefaultExecutionOrder"/> -200)

[DefaultExecutionOrder(-200)]
public class BattleBootstrapper : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SharedFloat sensitivity;
    [SerializeField] AudioMixer audioMixer;
    [Header("References")]
    [SerializeField] BossController bossController;
    [SerializeField] SharedInt activeEnemies;
    [SerializeField] PlayerHandler playerHandler;
    [SerializeField] ParticleSystemPool particlePool;
    [SerializeField] SoundPoolHandler soundPoolHandler;
    [SerializeField] ResultHandler resultHandler;
    [SerializeField] TimerHandler timerHandler;
    [SerializeField] ScoreHandler scoreHandler;
    BattleEndCondictionMetChecker battleEndChecker;

    private void Awake()
    {
        BattleServiceLocator.Instance.Register<IParticleSystemPool>(particlePool);
        BattleServiceLocator.Instance.Register<ISoundPoolHandler>(soundPoolHandler);
        BattleServiceLocator.Instance.Register(timerHandler);
        BattleServiceLocator.Instance.Register(playerHandler);
        BattleServiceLocator.Instance.Register(resultHandler);
        BattleServiceLocator.Instance.Register(scoreHandler);
        battleEndChecker = new BattleEndCondictionMetChecker(activeEnemies, bossController.GetComponent<IHealth>());
        BattleServiceLocator.Instance.Register(battleEndChecker);
        SetSensitivity();
        SetAudioMixer();
    }
    void SetSensitivity() => sensitivity.Value = ValuesUtil.LoadSensitivity();
    void SetAudioMixer()
    {
        //Por cada elemento del enum de tipos de audio, seteamos el audio
        foreach (GameAudioType audioType in Enum.GetValues(typeof(GameAudioType)))
            audioMixer.SetFloat(audioType.ToString(), ValuesUtil.GetVolumeForPencentage(ValuesUtil.LoadAudioPercentage(audioType)));
    }
}
