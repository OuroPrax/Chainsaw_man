using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.PlayOneShot(clip);
    }

}
