using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInputs playerInputs {  get; private set; }
    public PlayerAim playerAim { get; private set; }
    public PlayerMovement playerMovement { get; private set; }
    public PlayerWeaponsCtrl playerWeapon {  get; private set; }

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        playerAim = GetComponent<PlayerAim>();
        playerMovement = GetComponent<PlayerMovement>();
        playerWeapon = GetComponent<PlayerWeaponsCtrl>();
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
