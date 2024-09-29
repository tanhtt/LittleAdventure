using System;

public enum WeaponType
{
    Pistol,
    Revolver,
    Riffle,
    Shotgun,
    Sniper,
}

[System.Serializable]
public class Weapon
{
    public WeaponType weaponType;
    public int ammo;
    public int maxAmmo;

    public bool CanShoot()
    {
        return this.HaveEnoughBullet();
    }

    private bool HaveEnoughBullet()
    {
        if (ammo > 0)
        {
            ammo--;
            return true;
        }
        return false;
    }
}
