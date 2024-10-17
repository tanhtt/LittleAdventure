using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>();

    private Interactable closetInteractable;

    private void Start()
    {
        Player player = GetComponent<Player>();
        player.playerInputs.Player.Interaction.performed += context => InteractWithCloset();
    }

    private void InteractWithCloset()
    {
        closetInteractable?.Interaction();
        interactables.Remove(closetInteractable);

        UpdateClosetInteractable();
    }

    public void UpdateClosetInteractable()
    {
        closetInteractable?.HighlightMaterial(false);

        closetInteractable = null;

        float closetDistance = float.MaxValue;

        foreach(Interactable interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);

            if(distance < closetDistance)
            {
                closetDistance = distance;
                closetInteractable = interactable;
            }
        }

        closetInteractable?.HighlightMaterial(true);
    }

    public List<Interactable> Interactables() => interactables;
}
