using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisual weaponVisualController;
    private PlayerWeaponsCtrl weaponController;

    private void Start()
    {
        weaponVisualController = GetComponentInParent<PlayerWeaponVisual>();
        weaponController = GetComponentInParent<PlayerWeaponsCtrl>();
    }

    public void ReloadIsOver()
    {
        weaponVisualController.ReturnRigWeightToOne();
        weaponController.CurrentWeapon().RefillBullets();
        weaponController.SetWeaponReady(true);
    }

    public void ReturnRig()
    {
        weaponVisualController.ReturnRigWeightToOne();
        weaponVisualController.ReturnLeftHandIKWeightToOne();
    }

    public void EquipingWeaponIsOver()
    {
        weaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel() => weaponVisualController.SwitchOnCurrentWeaponModel();
}
