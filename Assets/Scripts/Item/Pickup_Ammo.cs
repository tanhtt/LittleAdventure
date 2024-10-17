using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoBoxType { bigBox, smallBox }

public class Pickup_Ammo : Interactable
{
    [SerializeField] private AmmoBoxType boxType;

    [System.Serializable]
    public struct AmmoData
    {
        public WeaponType weaponType;
        [Range(5f, 100f)] public int minAmount;
        [Range(5f, 100f)] public int maxAmount;
    }

    public List<AmmoData> smallBox;
    public List<AmmoData> bigBox;

    [SerializeField] private GameObject[] boxModel;

    private void Start()
    {
        UpdateBoxModel();
    }

    private void UpdateBoxModel()
    {
        for(int i = 0; i < boxModel.Length; i++)
        {
            boxModel[i].gameObject.SetActive(false);
            if(i == ((int)boxType))
            {
                boxModel[i].gameObject.SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = smallBox;

        if(this.boxType == AmmoBoxType.bigBox)
        {
            currentAmmoList = bigBox;
        }

        foreach (AmmoData ammoData in currentAmmoList)
        {
            Weapon weapon = playerWeaponsCtrl.WeaponInSlots(ammoData.weaponType);
            AddBulletToWeapon(weapon, GetRandomBulletAmount(ammoData));
        }

        ObjectPool.instance.ReturnObject(gameObject);
    }

    private void AddBulletToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null) return;
        weapon.totalReserveAmmo += amount;
    }

    private int GetRandomBulletAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.maxAmount, ammoData.minAmount);
        float max = Mathf.Max(ammoData.maxAmount, ammoData.minAmount);

        return Mathf.RoundToInt(Random.Range(min, max));
    }

}
