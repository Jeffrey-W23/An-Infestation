//--------------------------------------------------------------------------------------
// Purpose: The main logic of pick up items.
//
// Description: Set up an item for pickup in the game world by the player. Set its count,
// what it will look like in the world and what will happen on interaction.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// ItemPickup object. Inheriting from Interactable.
//--------------------------------------------------------------------------------------
public class ItemPickup : Interactable
{
    // ITEM SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Item Settings:")]

    // public item for the kind of item the pick up is
    [LabelOverride("Item")] [Tooltip("Item this item pickup will represent.")]
    public Item m_oItem;

    // public int for how manu items will be picked up
    [LabelOverride("Item Count")] [Tooltip("How many items are in this item pick up?")]
    public int m_nItemCount = 1;

    // public bool for if a custom sprite is to be used
    [LabelOverride("Use a Custom Sprite?")] [Tooltip("Will this item pickup use a custom item or use the same icon as the inventory item?")]
    public bool m_bCustomSprite = true;

    // public sprite for the custom sprite
    [LabelOverride("Custom Sprite")] [Tooltip("The sprite to use for the custom sprite.")]
    public Sprite m_sCustomSprite;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PROTECTED VALUES //
    //--------------------------------------------------------------------------------------
    // protected sprite renderer for the pickup item object
    protected SpriteRenderer m_srSpriteRenderer;

    // protected inventory manager object for getting the inventory manager intstance
    protected InventoryManager m_oInventoryManger;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    protected new void Awake()
    {
        // Run the base awake
        base.Awake();

        // Get the sprite renderer
        m_srSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    protected void Start()
    {
        // get the inventory instance
        m_oInventoryManger = InventoryManager.m_oInstance;

        // if the pickup item is going to use a custom sprite
        if (m_bCustomSprite)
        {
            // set the sprite to the custom sprite
            m_srSpriteRenderer.sprite = m_sCustomSprite;
        }

        // else if the sprite is staying the same as item icon
        else
        {
            // set the sprite of the pick up item to the inventory iten icon
            m_srSpriteRenderer.sprite = m_oItem.m_sIcon;
        }
    }

    //--------------------------------------------------------------------------------------
    // PickupItem: virtual function for picking up an item and adding to an inventory.
    //--------------------------------------------------------------------------------------
    protected virtual void PickupItem()
    {
        // bool for if the item can be added
        bool bItemAdded = false;

        // Atempt to add item to inventory
        bItemAdded = m_oPlayerObject.GetInventory().AddItem(new ItemStack(m_oItem, m_nItemCount));

        // remove the object from the world if a pick up is succesful
        if (bItemAdded)
            Object.Destroy(gameObject);

    }

    //--------------------------------------------------------------------------------------
    // InteractedWith: override function from base class for what Interactable objects do 
    // once they have been interacted with.
    //--------------------------------------------------------------------------------------
    protected override void InteractedWith()
    {
        // Run the base interactedWith function.
        base.InteractedWith();

        //
        PickupItem();

        // enabled interaction again
        m_bInteracted = false;
        m_bInteractable = false;
    }
}