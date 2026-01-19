using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverParticles : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ParticleSystem particleSystem;

    public void OnPointerEnter(PointerEventData eventData)
    {
        particleSystem.gameObject.SetActive(true);
        particleSystem.loop = true;
    }

    public void OnPointerExit(PointerEventData eventData) => particleSystem.loop = false;


}
