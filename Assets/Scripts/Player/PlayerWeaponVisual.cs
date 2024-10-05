using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisual : MonoBehaviour
{
    private Player player;
    private Animator _animator;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupWeaponModels;

    [Header("Left Hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_Target;
    [SerializeField] private float leftHandIKWeightIncreaseAmount;
    private bool shouldIncreaseLeftHandWeight;

    [Header("Rig Reload")]
    [SerializeField] private float rigWeightIncreaseAmount;
    private bool shouldIncreaseRigWeight;
    private Rig rig;

    public void PlayFireAnimation() => _animator.SetTrigger("Fire");

    private void Start()
    {
        player = GetComponent<Player>();
        _animator = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    public WeaponModel GetCurrentWeaponModel()
    {
        WeaponModel weaponModel = null;

        WeaponType weaponType = player.playerWeapon.CurrentWeapon().weaponType;

        for(int i = 0; i <  weaponModels.Length; i++)
        {
            if(weaponModels[i].weaponType == weaponType)
            {
                weaponModel = weaponModels[i];
            }
        }

        return weaponModel;
    }

    public void PlayWeaponEquipAnimation()
    {
        EquipType equipType = GetCurrentWeaponModel().equipAnimationType;
        float equipmentSpeed = player.playerWeapon.CurrentWeapon().equipmentSpeed;

        ReduceRigWeight();
        DisableLeftHandIK();
        _animator.SetFloat("EquipType", ((float)equipType));
        _animator.SetTrigger("EquipWeapon");
        _animator.SetFloat("EquipSpeed", equipmentSpeed);
    }

    public void SwitchOnCurrentWeaponModel()
    {
        int animationLayerIndex = ((int)GetCurrentWeaponModel().holdType);

        SwitchOffWeaponModels();
        SwitchOffBackupWeaponModels();
        if (!player.playerWeapon.HasOnlyOneWeapon())
        {
            SwitchOnBackupWeaponModel();
        }

        SetLayerAnimator(animationLayerIndex);
        GetCurrentWeaponModel().gameObject.SetActive(true);
        SetLeftHandTargetTransform();
    }

    public void SwitchOffWeaponModels()
    {
        for(int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    public void SwitchOffBackupWeaponModels()
    {
        foreach(BackupWeaponModel weapon in backupWeaponModels)
        {
            weapon.gameObject.SetActive(false);
        }
    }

    public void SwitchOnBackupWeaponModel()
    {
        Weapon backupWeapon = player.playerWeapon.GetBackupWeapon();
        if (backupWeapon == null) return;

        foreach(BackupWeaponModel backupWeaponModal in backupWeaponModels)
        {
            if(backupWeaponModal.WeaponType == backupWeapon.weaponType)
            {
                backupWeaponModal.gameObject.SetActive(true);
            }
        }
    }

    public void PlayReloadAnimation()
    {
        float reloadSpeed = player.playerWeapon.CurrentWeapon().reloadSpeed;

        _animator.SetTrigger("Reload");
        _animator.SetFloat("ReloadSpeed", reloadSpeed);
        ReduceRigWeight();
    }

    #region Animation Rigging Setup
    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncreaseLeftHandWeight)
        {
            leftHandIK.weight += leftHandIKWeightIncreaseAmount * Time.deltaTime;
            if (leftHandIK.weight >= 1)
            {
                shouldIncreaseLeftHandWeight = false;
            }
        }
    }

    private void UpdateRigWeight()
    {
        if (shouldIncreaseRigWeight)
        {
            rig.weight += rigWeightIncreaseAmount * Time.deltaTime;
            if (rig.weight >= 1)
            {
                shouldIncreaseRigWeight = false;
            }
        }
    }

    private void SetLayerAnimator(int layerIndex)
    {
        for (int i = 1; i < _animator.layerCount; i++)
        {
            _animator.SetLayerWeight(i, 0);
        }

        _animator.SetLayerWeight(layerIndex, 1);
    }

    private void SetLeftHandTargetTransform()
    {
        Transform leftHandTransform = GetCurrentWeaponModel().holdPoint;
        leftHandIK_Target.localPosition = leftHandTransform.localPosition;
    }

    public void ReturnRigWeightToOne() => shouldIncreaseRigWeight = true;
    public void ReturnLeftHandIKWeightToOne() => shouldIncreaseLeftHandWeight = true;

    private void ReduceRigWeight()
    {
        rig.weight = 0.15f;
    }

    private void DisableLeftHandIK()
    {
        leftHandIK.weight = 0.15f;
    }

    #endregion
}

public enum EquipType { SideEquipAnimation, BackEquipAnimation};
