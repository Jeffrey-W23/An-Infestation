//--------------------------------------------------------------------------------------
// Purpose: The main logic of Equipable item pickups.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// EquipableItemPickup object. Inheriting from ItemPickup (Which is inheriting from interactable).
//--------------------------------------------------------------------------------------
public class EquipableItemPickup : ItemPickup
{
    // PROTECTED NETWORKED VARS //
    //--------------------------------------------------------------------------------------
    // new protected network variable ulong for keeping track of the scene object of this item's network ID
    protected NetworkVariableULong mn_ulSceneObjectNetworkID = new NetworkVariableULong(0);
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    protected new void Awake()
    {
        // Run the base awake
        base.Awake();
    }

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    protected new void Start()
    {
        // run the base awake
        base.Start();

        // Spawn scene object for the item if this instance is the host
        if (NetworkManager.Singleton.IsHost)
            SpawnSceneObjectServerRpc();

        // else if a connecting client set the scene object spawned above to inactive
        else
            NetworkSpawnManager.SpawnedObjects[mn_ulSceneObjectNetworkID.Value].gameObject.SetActive(false);
    }

    //--------------------------------------------------------------------------------------
    // OnEnable: Function that will call when this gameObject is enabled.
    //--------------------------------------------------------------------------------------
    protected new void OnEnable()
    {
        // Run base OnEnabled from Item Pickup class
        base.OnEnable();
    }

    //--------------------------------------------------------------------------------------
    // OnDestroy: Function that will call on this gameObjects destruction.
    //--------------------------------------------------------------------------------------
    private void OnDestroy()
    {

    }

    //--------------------------------------------------------------------------------------
    // PickupItem: virtual function for picking up an item and adding to an inventory.
    //--------------------------------------------------------------------------------------
    protected override bool PickupItem(Player oPlayer)
    {
        // Atempt to add item to inventory.
        // remove the object from the world if a pick up is succesful
        if (oPlayer.GetEquipableItems().AddItem(new ItemStack(m_oItem, mn_nCurrentItemCount.Value, mn_ulSceneObjectNetworkID.Value)) && m_nbInteractableCollected != null)
        {
            // Set collected status to true
            m_nbInteractableCollected.Value = true;

            // Assign the scene object for this item to the player
            oPlayer.GetArm().InitalizeEquipableItem(mn_ulSceneObjectNetworkID.Value);

            // return true for success
            return true;
        }

        // item wasn't added to inventory, return false
        return false;
    }

    //--------------------------------------------------------------------------------------
    // SpawnSceneObjectServerRpc: Server function for Spawning the scene object used when 
    // picking up this item.
    //--------------------------------------------------------------------------------------
    [ServerRpc]
    private void SpawnSceneObjectServerRpc()
    {
        // Send debug message to console
        Debug.Log("ServerRpc for Spawn Scene Object Called!");

        // Spawn object on the server intended to be used as the item when equipped
        NetworkObject noSceneObject = Instantiate(m_oItem.m_gSceneObject.GetComponent<NetworkObject>());
        noSceneObject.transform.position = transform.position;
        noSceneObject.Spawn();

        // Set the object id for thew scene object
        mn_ulSceneObjectNetworkID.Value = noSceneObject.NetworkObjectId;

        // Call client rpc for ensuring spawned object is prepared for clients
        SpawnSceneObjectClientRpc(noSceneObject.NetworkObjectId);
    }

    //--------------------------------------------------------------------------------------
    // SpawnSceneObjectClientRpc: Client function for preparing the spawned scene object.
    //
    // Param:
    //      ulObjectNetworkID: ulong value for ID of the spawned network object.
    //--------------------------------------------------------------------------------------
    [ClientRpc]
    private void SpawnSceneObjectClientRpc(ulong ulObjectNetworkID)
    {
        // Send debug message to console
        Debug.Log("ClientRpc for Spawn Scene Object Called!");

        // Set the object to inactive ready for use
        NetworkSpawnManager.SpawnedObjects[ulObjectNetworkID].gameObject.SetActive(false);
    }
}