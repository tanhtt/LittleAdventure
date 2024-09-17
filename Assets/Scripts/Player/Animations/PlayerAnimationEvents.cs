using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisual weaponVisualController;

    private void Start()
    {
        weaponVisualController = GetComponentInParent<PlayerWeaponVisual>();
    }

    public void ReloadIsOver()
    {
        weaponVisualController.ReturnRigWeightToOne();
    }

    public void ReturnRig()
    {
        weaponVisualController.ReturnRigWeightToOne();
        weaponVisualController.ReturnLeftHandIKWeightToOne();
    }

    public void GrabWeaponIsOver()
    {
        weaponVisualController.SetBusyGrabbingWeapon(false);
    }
}
