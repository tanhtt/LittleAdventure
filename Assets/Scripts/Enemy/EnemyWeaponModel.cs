using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponModel : MonoBehaviour
{
    public Enemy_MeleeWeaponType weaponType;
    public AnimatorOverrideController overrideController;

    public EnemyMeleeWeaponData weaponData;

    [SerializeField] private GameObject[] trailEffects;

    public void EnableTrailEffect(bool active)
    {
        foreach(GameObject effect in trailEffects)
        {
            effect.SetActive(active);
        }
    }
}
