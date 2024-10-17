using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInputs playerInputs {  get; private set; }
    public PlayerAim playerAim { get; private set; }
    public PlayerMovement playerMovement { get; private set; }
    public PlayerWeaponsCtrl playerWeapon {  get; private set; }
    public PlayerWeaponVisual playerWeaponVisual { get; private set; }
    public PlayerInteraction playerInteraction { get; private set; }

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        playerAim = GetComponent<PlayerAim>();
        playerMovement = GetComponent<PlayerMovement>();
        playerWeapon = GetComponent<PlayerWeaponsCtrl>();
        playerWeaponVisual = GetComponent<PlayerWeaponVisual>();
        playerInteraction = GetComponent<PlayerInteraction>();
    }

    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }
}
