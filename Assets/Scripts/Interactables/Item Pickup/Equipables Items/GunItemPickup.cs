//--------------------------------------------------------------------------------------
// Purpose: The main logic of gun pickup items.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// EquipableItemPickup object. Inheriting from ItemPickup (Which is inheriting from interactable).
//--------------------------------------------------------------------------------------
public class GunItemPickup : EquipableItemPickup
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
    //
    // Params:
    //      oPlayer: Player gameobject interacting with the interactable object
    //--------------------------------------------------------------------------------------
    protected override bool PickupItem(Player oPlayer)
    {
        // Run base PickupItem function, check if it was successful
        if (base.PickupItem(oPlayer))
        {
            // return true for successful item added to inventory
            return true;
        }

        // return false if picking up item fails
        return false;
    }
}