using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : ComboController
{
    bool _canAttack = true;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && _canAttack) RegisterAttackInput();
    }

    public void SetCanAttack(bool value) => _canAttack = value;
}
