using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected PlayerWeaponsCtrl playerWeaponsCtrl;
    protected MeshRenderer mesh;
    protected Material defaultMaterial;
    [SerializeField] private Material highlightMaterial;

    private void Start()
    {
        if(mesh == null)
            mesh = GetComponentInChildren<MeshRenderer>();

        defaultMaterial = mesh.sharedMaterial;
    }

    protected void UpdateMeshAndMaterial(MeshRenderer mesh)
    {
        this.mesh = mesh;
        this.defaultMaterial = this.mesh.sharedMaterial;
    }

    public void HighlightMaterial(bool active)
    {
        if (active)
        {
            mesh.material = highlightMaterial;
        }
        else
        {
            mesh.material = defaultMaterial;
        }
    }

    public virtual void Interaction()
    {
        Debug.Log("Interact with: "+ this.gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null) return;

        playerInteraction.Interactables().Add(this);
        playerInteraction.UpdateClosetInteractable();

        playerWeaponsCtrl = other.GetComponent<PlayerWeaponsCtrl>();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            
        if (playerInteraction == null) return;

        playerInteraction.Interactables().Remove(this);
        playerInteraction.UpdateClosetInteractable();
    }
}
