using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponsCtrl : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();

        player.PlayerInputs.Player.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }
}
