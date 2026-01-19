using System.Linq;
using UnityEngine;

/// <summary>
/// Sonido en base a si esta tocando el piso, posee un metodo para lanzar el sonido.
/// En el proyecto es utilizado por event trigger de animacion en cada pie del jugador
/// </summary>
public class FootstepHandler : MonoBehaviour
{
    [Header("Footstep Settings")]
    [SerializeField] SoundsArraySO stepSounds;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float raycastDistance = .1f;
    ISoundPoolHandler soundPoolHandler;
    private void Start() => soundPoolHandler = BattleServiceLocator.Instance.Get<ISoundPoolHandler>();
    public void PlayFootstep()
    {
        if(!Physics.Raycast(transform.position, Vector3.down, raycastDistance, groundLayer)) return;
        if (stepSounds.audioClips.Any())
            soundPoolHandler.PlaySound(stepSounds.audioClips[Random.Range(0, stepSounds.audioClips.Length)], Random.Range(.8f, 1f), Random.Range(.8f,1.2f), transform.position);
    }
}

