//--------------------------------------------------------------------------------------
// Purpose: The main logic for the player container.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// PlayerContainer object. Inheriting from Container.
//--------------------------------------------------------------------------------------
public class PlayerContainer : Container
{
    //--------------------------------------------------------------------------------------
    // Default Constructor.
    //
    // Param:
    //      oInventory: The inventory used for this container.
    //      oPlayerInventory: The inventory used for the player container.
    //      nSlots: the amount of slots in the container/inventory.
    //--------------------------------------------------------------------------------------
    public PlayerContainer(Inventory oInventory, Inventory oPlayerInventory, int nSlots) : base(oInventory, oPlayerInventory, nSlots)
    {
        // loop through each slot
        for (int i = 0; i < m_nSlots; i++)
        {
            // build the inventory slots
            AddSlot(oPlayerInventory, i, m_gPrefab.GetComponentInChildren<Transform>().Find("Inventory").transform);
        }

        // loop through each slot in the weapons inventory
        for (int i = 0; i < oInventory.GetInventory().Count; i++)
        {
            // build the inventory slots
            AddSlot(oInventory, i, m_gPrefab.GetComponentInChildren<Transform>().Find("Weapon Slots").transform);
        }
    }

    //--------------------------------------------------------------------------------------
    // GetPrefab: Get the container prefab for this container. 
    //
    // Return:
    //      GameObject: Returns the prefab of this container.
    //--------------------------------------------------------------------------------------
    public override GameObject GetPrefab()
    {
        // return the inventory managers instance container prefab
        return InventoryManager.m_gInstance.GetContainerPrefab("PlayerContainer");
    }
}