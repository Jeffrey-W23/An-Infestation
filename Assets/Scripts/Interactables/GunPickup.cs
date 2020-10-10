//--------------------------------------------------------------------------------------
// Purpose: The main logic of gun pickup items.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
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
    protected override void PickupItem()
    {
        // bool for if the item can be added
        bool bItemAdded = false;

        // Atempt to add item to inventory
        bItemAdded = m_oPlayerObject.GetWeapons().AddItem(new ItemStack(m_oItem, m_nItemCount));

        // remove the object from the world if a pick up is succesful
        if (bItemAdded)
            Object.Destroy(gameObject);
    }
}
