//--------------------------------------------------------------------------------------
// Purpose: The main logic of Equipable item pickups.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// EquipableItemPickup object. Inheriting from ItemPickup (Which is inheriting from interactable).
//--------------------------------------------------------------------------------------
public class EquipableItemPickup : ItemPickup
{
    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    new void Awake()
    {
        // Run the base awake
        base.Awake();
    }

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    new void Start()
    {
        // run the base awake
        base.Start();
    }

    //--------------------------------------------------------------------------------------
    // PickupItem: virtual function for picking up an item and adding to an inventory.
    //--------------------------------------------------------------------------------------
    protected override bool PickupItem(Player oPlayer)
    {
        // Atempt to add item to inventory.
        // remove the object from the world if a pick up is succesful
        if (oPlayer.GetEquipableItems().AddItem(new ItemStack(m_oItem, mn_nCurrentItemCount.Value)) && m_nbInteractableCollected != null)
        {
            // Set collected status to true
            m_nbInteractableCollected.Value = true;

            // return true for success
            return true;
        }

        // item wasn't added to inventory, return false
        return false;
    }
}