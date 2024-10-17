using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Weapon : Interactable
{
    [SerializeField] private Weapon_Data weaponData;
    [SerializeField] private Weapon weapon;
    [SerializeField] private List<BackupWeaponModel> backupWeaponModels;

    private bool oldWeapon;

    private void Start()
    {
        if(oldWeapon == false)
        {
            weapon = new Weapon(weaponData);
        }
        UpdateGameObject();
    }

    [ContextMenu("Update Item Model")]
    private void UpdateGameObject()
    {
        this.gameObject.name = "Pickup_Weapon - " + weaponData.weaponType.ToString();
        UpdateItemModel();
    }

    private void UpdateItemModel()
    {
        foreach(BackupWeaponModel model in backupWeaponModels)
        {
            model.gameObject.SetActive(false);

            if(model.WeaponType == weaponData.weaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>());
            }
        }
    }

    public void UpdatePickupWeapon(Weapon weapon, Transform transform)
    {
        oldWeapon = true;

        this.weapon = weapon;
        this.weaponData = weapon.weaponData;

        this.transform.position = transform.position + new Vector3(0, 0.7f, 0);
    }

    public override void Interaction()
    {
        playerWeaponsCtrl.PickupWeapon(weapon);
        ObjectPool.instance.ReturnObject(gameObject);
    }
}
