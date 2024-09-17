using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private Transform[] weapons;

    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform rifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform sniper;

    [Header("Ref Target")]
    [SerializeField] private Transform leftHandTargetTransform;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        ActiveWeapon(pistol);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActiveWeapon(pistol);
            SetLayerAnimator(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActiveWeapon(revolver);
            SetLayerAnimator(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActiveWeapon(rifle);
            SetLayerAnimator(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActiveWeapon(shotgun);
            SetLayerAnimator(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ActiveWeapon(sniper);
            SetLayerAnimator(3);
        }
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
        leftHandTargetTransform.localPosition = leftHandTransform.localPosition;
    }

    private void SetLayerAnimator(int layerIndex)
    {
        for(int i = 1; i < _animator.layerCount; i++)
        {
            _animator.SetLayerWeight(i, 0);
        }

        _animator.SetLayerWeight(layerIndex, 1);
    }
}
