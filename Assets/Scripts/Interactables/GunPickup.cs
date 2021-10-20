//--------------------------------------------------------------------------------------
// Purpose: The main logic of gun pickup items.
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
// GunPickup object. Inheriting from ItemPickup (Which is inheriting from interactable).
//--------------------------------------------------------------------------------------
public class GunPickup : ItemPickup
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
    protected override void PickupItem(Player oPlayer)
    {
        // Atempt to add item to inventory.
        // remove the object from the world if a pick up is succesful
        if (oPlayer.GetWeapons().AddItem(new ItemStack(m_oItem, m_nItemCount)) && m_nbInteractableCollected != null)
            m_nbInteractableCollected.Value = true;
    }
}