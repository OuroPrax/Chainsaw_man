using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToForward : MonoBehaviour
{
    [SerializeField] float distanceToAdvace = 20;
    [SerializeField] Transform characterVisual;


    private void OnEnable()
    {
        characterVisual.localPosition = new Vector3(0, 0, distanceToAdvace);
    }

    private void OnDisable()
    {
        characterVisual.parent.transform.position = characterVisual.position;  
        characterVisual.localPosition = Vector3.zero;
    }
}
