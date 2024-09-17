using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisual : MonoBehaviour
{
    private Animator _animator;
    private bool isGrabbingWeapon;

    #region Gun ref
    [SerializeField] private Transform[] weapons;

    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform rifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform sniper;

    #endregion

    [Header("Left Hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_Target;
    [SerializeField] private float leftHandIKWeightIncreaseAmount;
    private bool shouldIncreaseLeftHandWeight;

    [Header("Rig Reload")]
    [SerializeField] private float rigWeightIncreaseAmount;
    private bool shouldIncreaseRigWeight;
    private Rig rig;


    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        ActiveWeapon(pistol);
    }

    private void Update()
    {
        CheckWeaponSwitch();

        if(Input.GetKeyDown(KeyCode.R))
        {
            _animator.SetTrigger("Reload");
            ReduceRigWeight();
        }


        if (shouldIncreaseRigWeight)
        {
            rig.weight += rigWeightIncreaseAmount * Time.deltaTime;
            if(rig.weight >= 1)
            {
                shouldIncreaseRigWeight = false;
            }
        }

        if (shouldIncreaseLeftHandWeight)
        {
            leftHandIK.weight += leftHandIKWeightIncreaseAmount * Time.deltaTime;
            if(leftHandIK.weight >= 1)
            {
                shouldIncreaseLeftHandWeight = false;
            }
        }
    }

    public void ReturnRigWeightToOne() => shouldIncreaseRigWeight = true;
    public void ReturnLeftHandIKWeightToOne() => shouldIncreaseLeftHandWeight = true;

    public void SetBusyGrabbingWeapon(bool isBusy)
    {
        isGrabbingWeapon = isBusy;
        _animator.SetBool("BusyGrabingWeapon", isGrabbingWeapon);
    }


    private void PlayWeaponGrabAnimation(GrabType grabType)
    {
        ReduceRigWeight();
        DisableLeftHandIK();
        _animator.SetFloat("WeaponGrabType", ((float)grabType));
        _animator.SetTrigger("WeaponGrab");
        SetBusyGrabbingWeapon(true);
    }

    private void ReduceRigWeight()
    {
        rig.weight = 0.15f;
    }

    private void DisableLeftHandIK()
    {
        leftHandIK.weight = 0.15f;
    }

    private void ActiveWeapon(Transform weapon)
    {
        DeactiveWeapons();
        weapon.gameObject.SetActive(true);
        SetLeftHandTargetTransform(weapon);
    }

    private void DeactiveWeapons()
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(false);
        }
    }

    private void SetLeftHandTargetTransform(Transform weapon)
    {
        Transform leftHandTransform = weapon.GetComponentInChildren<LeftHandTarget>().transform;
        leftHandIK_Target.localPosition = leftHandTransform.localPosition;
    }

    private void SetLayerAnimator(int layerIndex)
    {
        for(int i = 1; i < _animator.layerCount; i++)
        {
            _animator.SetLayerWeight(i, 0);
        }

        _animator.SetLayerWeight(layerIndex, 1);
    }


    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActiveWeapon(pistol);
            SetLayerAnimator(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActiveWeapon(revolver);
            SetLayerAnimator(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActiveWeapon(rifle);
            SetLayerAnimator(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActiveWeapon(shotgun);
            SetLayerAnimator(2);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ActiveWeapon(sniper);
            SetLayerAnimator(3);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
    }
}

public enum GrabType { SideGrab, BackGrab};
