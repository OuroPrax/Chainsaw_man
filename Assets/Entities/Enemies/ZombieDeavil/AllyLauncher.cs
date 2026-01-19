using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyLauncher : MonoBehaviour
{
    public bool IsFull => elevatedZombies.Count >= maxAtSameTime;

    Transform target;
    [Header("Detection")]
    [SerializeField] float searchRadius = 10f;
    [SerializeField] LayerMask zombieMask;
    [SerializeField] int maxAtSameTime;

    [Header("Timing")]
    [SerializeField] float launchCooldown = 8f;
    [SerializeField] float holdDuration = 2f;
    [SerializeField] float pickupHeight = 3f;
    [SerializeField] float elevateSpeed = 3f;

    [Header("Transforms & Force")]
    [SerializeField] Transform pickTransform;
    [SerializeField] float launchForce = 15f;

    Coroutine launchRoutine;
    WaitWhile waitWhileIsFull;
    readonly List<IRestrainable> elevatedZombies = new();

    private void Awake() => waitWhileIsFull = new WaitWhile(() => IsFull);

    public void BeginLaunching(Transform target)
    {
        this.target = target;
        launchRoutine ??= StartCoroutine(LaunchLoop());
    }
    public void StopLaunching()
    {
        if (launchRoutine != null)
        {
            StopCoroutine(launchRoutine);
            launchRoutine = null;
        }

        elevatedZombies.Clear();
    }

    IEnumerator LaunchLoop()
    {
        var waitCooldown = new WaitForSeconds(launchCooldown);

        while (true)
        {
            yield return waitWhileIsFull;
            yield return waitCooldown;
            TryPickAndLaunch();
        }
    }
    void TryPickAndLaunch()
    {
        // 1) Detect nearby zombies
        var colliders = Physics.OverlapSphere(transform.position, searchRadius, zombieMask);
        if (colliders.Length == 0) return;
        Debug.Log("1)Detecto");

        IRestrainable restrainable = null;
        // 2) Recorremos todos y nos quedamos con el más cercano que cumpla las condiciones
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out restrainable))
                break;
        }

        // 3) Si no encontramos ninguno válido, salimos
        if (restrainable == null) return;

        // 4) Activamos ragdoll, parent y arrancamos la corrutina
        restrainable.TryRestrain();
        elevatedZombies.Add(restrainable);
        StartCoroutine(ElevateHoldAndLaunch(restrainable));
    }
    IEnumerator ElevateHoldAndLaunch(IRestrainable restrainable)
    {
        var targetRoot = restrainable.Root;
        targetRoot.SetParent(pickTransform);

        Vector3 start = targetRoot.localPosition;
        Vector3 offsetXZ = start;
        offsetXZ.y = 0f;
        Vector3 end;
        float currentDistance = offsetXZ.magnitude;
        float minXZDistance = 2f;
        if (currentDistance < minXZDistance)
        {
            Vector3 adjustedXZ = offsetXZ.normalized * minXZDistance;
            end = new Vector3(adjustedXZ.x, pickupHeight, adjustedXZ.z);
        }
        else
            end = new Vector3(start.x, pickupHeight, start.z);
        Debug.Log("End " + end);

        yield return targetRoot.DOLocalMove(end, elevateSpeed).SetSpeedBased(true).WaitForCompletion();
        targetRoot.localPosition = end;
     
        Debug.Log("Zombie local" + targetRoot.localPosition);
        // Mantener en alto
        var elapsed = 0f;
        while (elapsed < holdDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Zombie local despues de esperar " + targetRoot.localPosition);

        // Soltar y lanzar
        targetRoot.SetParent(null);

        Debug.Log("Lanzamiento: Espera currentTarget");
        yield return new WaitUntil(() => target);

        var dir = (target.position - targetRoot.position).normalized;
        restrainable.Launch(dir * launchForce);

        Debug.Log("Lanzamiento: Lanzado");
        // Quitar de la lista de elevados
        elevatedZombies.Remove(restrainable);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
