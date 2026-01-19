using System.Collections;
using UnityEngine;

public class _test_Health : MonoBehaviour
{
    HealthHandler healthHandler;
    IEnumerator Start()
    {
        healthHandler = GetComponent<HealthHandler>();
        yield return new WaitForSeconds(5f);
        healthHandler.TakeDamage(50f, Vector3.zero);
    }
}