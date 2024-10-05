using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerInputs controls;

    private Vector2 mouseInput;

    [Header("Aim Visual & Laser")]
    [SerializeField] private LineRenderer laser;

    [Header("Aim Info")]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;
    [SerializeField] private bool isAimingPrecisly;
    [SerializeField] private bool isLockingAtTarget;



    [Header("Camera Info")]
    [SerializeField] private Transform cameraTarget;
    [Range(0f, 1f)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(1f, 3f)]
    [SerializeField] private float maxCameraDistance = 4;
    [Range(1f, 5f)]
    [SerializeField] private float aimSensetivity = 5;

    [Space]

    private RaycastHit lastKnownMouseHit;

    private void Start()
    {
        player = GetComponent<Player>();

        AssignInputEvents();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            isAimingPrecisly = !isAimingPrecisly;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            isLockingAtTarget = !isLockingAtTarget;
        }

        UpdateLaser();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void UpdateLaser()
    {
        laser.enabled = player.playerWeapon.WeaponReady();

        if(laser.enabled == false )
        {
            return;
        }

        WeaponModel weaponModel = player.playerWeaponVisual.GetCurrentWeaponModel();

        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);

        Vector3 gunPoint = player.playerWeapon.GunPoint().position;
        Vector3 laserDirection = player.playerWeapon.BulletDirection();
        float gunDistance = 4f;
        float laserTipLength = .5f;

        Vector3 endpoint = gunPoint + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint, laserDirection, out RaycastHit hit, gunDistance))
        {
            endpoint = hit.point;
            laserTipLength = 0;
        }

        laser.SetPosition(0, gunPoint);
        laser.SetPosition(1, endpoint);
        laser.SetPosition(2, endpoint + laserDirection * laserTipLength);

    }

    private void UpdateCameraPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), aimSensetivity * Time.deltaTime);
    }

    public Transform Target()
    {
        Transform target = null;
        if(GetHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetHitInfo().transform;
        }
        return target;
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();
        if(target != null)
        {
            if (target.GetComponent<Renderer>() != null)
                aim.position = target.GetComponent<Renderer>().bounds.center;
            else
                aim.position = target.position;

            return;
        }

        aim.position = GetHitInfo().point;
        if (!isAimingPrecisly)
        {
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
        }
    }

    public RaycastHit GetHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hit;
            return hit;
        }

        return lastKnownMouseHit;
    }

    public bool CanAimPrecisly()
    {
        if (isAimingPrecisly)
        {
            return true;
        }
        return false;
    }

    public Transform GetAimTransform()
    {
        return aim;
    }

    private Vector3 DesiredCameraPosition()
    {
        float actualMaxCameraDistance = player.playerMovement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distanceToDesiredAimPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesiredAimPosition, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    private void AssignInputEvents()
    {
        controls = player.playerInputs;

        controls.Player.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Player.Aim.canceled += context => mouseInput = Vector2.zero;
    }
}
