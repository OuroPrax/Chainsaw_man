using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MovementPattern : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] Transform[] waypoints;
    [Space(10)]
    [SerializeField] float horizontalSpeed = 3f;
    [SerializeField] float verticalSpeed = 3f;
    [Space(10)]
    [SerializeField] float highY = 5f;
    [SerializeField] float lowY = 2f;
    [Space(10)]
    [SerializeField] float waitTime = 2f;
    Coroutine moveCoroutine;

    #region levitation
    [Header("Levitation")]
    [SerializeField] Transform oscillationTransform;
    [SerializeField] float levitationAmplitude = 0.2f;
    [SerializeField] float levitationFrequency = 2f;
    [SerializeField] float centeringDuration = 0.3f;
    Tween levitationTween;  
    void StartLevitation()
    {
        levitationTween = oscillationTransform
            .DOLocalMoveY(levitationAmplitude, 1f / levitationFrequency)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    IEnumerator StopLevitationAndCenter()
    {
        if (levitationTween != null && levitationTween.IsActive())
            levitationTween.Kill();

        yield return oscillationTransform
            .DOLocalMoveY(0f, centeringDuration)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();
    }
    #endregion

    #region look at player
    [Header("LookAtPlayer")]
    [SerializeField] float rotationSpeed = 180f; // grados por segundo
    [SerializeField] Transform transformToRotate;
    Coroutine lookAtCoroutine;
    IEnumerator LookAtLoop()
    {
        var target = FindAnyObjectByType<PlayerCombatController>(FindObjectsInactive.Include).transform;
        var waitTarget = new WaitUntil(() => target != null);
        while (true)
        {
            yield return waitTarget;

            Vector3 direction = target.position - transformToRotate.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f) continue;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Interpolación suave
            transformToRotate.rotation = Quaternion.RotateTowards(
                transformToRotate.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    #endregion


    private void OnDestroy()
    {
        StopMovement();
    }

    public void BeginRoute()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(RouteLoop());
        lookAtCoroutine = StartCoroutine(LookAtLoop());
    }
    public void StopMovement()
    {
        levitationTween?.Kill();
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        if(lookAtCoroutine != null)
            StopCoroutine(lookAtCoroutine);
    }

    IEnumerator RouteLoop()
    {
        if (waypoints.Length < 2) yield break;

        Vector3 newPos;
        Vector3 waypointPos = Vector3.zero;
        Vector3 previousPos;
        while (true)
        {
            previousPos = waypointPos;
            do
              { waypointPos = waypoints[Random.Range(0, waypoints.Length)].position;}
            while (waypointPos == previousPos);

            StartLevitation();
            // Subir a highY
            while (Mathf.Abs(transform.position.y - highY) > 0.01f)
            {
                newPos = new Vector3(transform.position.x, highY, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, newPos, verticalSpeed * Time.deltaTime);
                yield return null;
            }

            // Ir al waypoint horizontalmente
            while ((new Vector2(transform.position.x, transform.position.z) - new Vector2(waypointPos.x, waypointPos.z)).sqrMagnitude > 0.01f)
            {
                newPos = Vector3.MoveTowards(transform.position, waypointPos, horizontalSpeed * Time.deltaTime);
                transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
                yield return null;
            }

            // Bajar a lowY
            while (Mathf.Abs(transform.position.y - lowY) > 0.01f)
            {
                newPos = new Vector3(transform.position.x, lowY, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, newPos, verticalSpeed * Time.deltaTime);
                yield return null;
            }

            yield return StopLevitationAndCenter();

            yield return new WaitForSeconds(waitTime);

        }
    }
}
