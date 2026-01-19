using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimatorUpdateMode : MonoBehaviour
{
    [SerializeField] AnimatorUpdateMode mode = AnimatorUpdateMode.Normal;
    [SerializeField] Animator animator;

    public void ChangeUpdateMode()
    {
        animator.updateMode = mode;
    }
}
