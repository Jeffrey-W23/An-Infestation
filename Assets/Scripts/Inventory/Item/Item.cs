//--------------------------------------------------------------------------------------
// Purpose: The main script for setting up an item for the use with inventory systems.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Enum EItemType. Used for setting the type of item.
//--------------------------------------------------------------------------------------
public enum EItemType
{
    ETYPE_GUN,
    ETYPE_BULLET
}

//--------------------------------------------------------------------------------------
// Item object. Inheriting from ScriptableObject.
//--------------------------------------------------------------------------------------
[CreateAssetMenu(fileName = "New Item")] // Create a menu item for quickly creating items
public class Item : ScriptableObject
{
    // ITEM //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Item Settings:")]

    // public string for the item title
    [LabelOverride("Item Title")] [Tooltip("The title of the item.")]
    public string m_strTitle;

    // public sprite for the item icon
    [LabelOverride("Item Icon")] [Tooltip("The icon of the item, for displaying in inventory, etc.")]
    public Sprite m_sIcon;

    // public int for the max stack size
    [LabelOverride("Max Stack Size")] [Tooltip("The max that an item can be stacked in an inventory.")]
    public int m_nMaxStackSize = 1;

    // public enum for item type.
    [LabelOverride("Item Type")] [Tooltip("What type of item? A weapon, ammo, health item?")]
    public EItemType m_eItemType;
    //--------------------------------------------------------------------------------------
}