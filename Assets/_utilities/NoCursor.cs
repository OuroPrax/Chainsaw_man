using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoCursor : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
