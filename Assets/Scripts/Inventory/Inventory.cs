//--------------------------------------------------------------------------------------
// Purpose: The main logic of the Inventory system.
//
// Description: A simple script for initializing an inventory system and allow adding 
// items to it.
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
    ETYPE_NONE = 1 << 0,
    ETYPE_ALL = 1 << 1,
    ETYPE_GUN = 1 << 2,
    ETYPE_BULLET = 1 << 3,
    ETYPE_TEST = 1 << 4
}

//--------------------------------------------------------------------------------------
// Inventory object.
//--------------------------------------------------------------------------------------
public class Inventory
{
    // private List of item stack: the inventory array.
    private List<ItemStack> m_aoItems = new List<ItemStack>();

    // private list of enums for incompatible items in an inventory
    public List<EItemType> m_aeIncompatibleItems = new List<EItemType>();

    //--------------------------------------------------------------------------------------
    // Default Constructor.
    //
    // Param:
    //      nSize: An int for the size of the inventory system to create.
    //--------------------------------------------------------------------------------------
    public Inventory(int nSize, List<EItemType> aeIncompatibleItems)
    {
        // loop through the inventory size
        for (int i = 0; i < nSize; i++)
        {
            // add an item stack for every item
            m_aoItems.Add(new ItemStack(i));
        }

        // set the incompatible and compatible items of this inventory
        SetIncompatibleItems(aeIncompatibleItems);
    }

    //--------------------------------------------------------------------------------------
    // AddItem: Add an item to the inventory system.
    //
    // Param:
    //      oStack: The item stack to add to the inventory system.
    //
    // Return:
    //      bool: return the status of the item adding.
    //--------------------------------------------------------------------------------------
    public bool AddItem(ItemStack oStack)
    {
        // check if the item attempting to add to the inventory is incompatible
        if (CheckIfIncompatible(oStack.GetItem().m_eItemType, m_aeIncompatibleItems))
        {
            // return false, item not added.
            return false;
        }

        // loop through each item stack in the inventory
        foreach (ItemStack i in m_aoItems)
        {
            // is the stack empty
            if (i.IsStackEmpty())
            {
                // set the stack to passed in stack
                i.SetStack(oStack);

                // return true, item added
                return true;
            }

            // is the stack equal to passed in stack
            if (ItemStack.AreItemsEqual(oStack, i))
            {
                // is it possible to add items to this stack
                if (i.IsItemAddable(oStack.GetItemCount()))
                {
                    // increase the stack count
                    i.IncreaseStack(oStack.GetItemCount());

                    // return true, item added
                    return true;
                }

                // else if the item is not addable
                else
                {
                    // new int var, get the difference between passed in stack and current stack
                    int nDifference = (i.GetItemCount() + oStack.GetItemCount()) - i.GetItem().m_nMaxStackSize;

                    // set the count of the stack to the max stack size of the stacks item
                    i.SetItemCount(i.GetItem().m_nMaxStackSize);

                    // set the count of the passed in stack to the stack differnce
                    oStack.SetItemCount(nDifference);
                }
            }
        }

        // return false, item not added.
        return false;
    }

    //--------------------------------------------------------------------------------------
    // GetStackInSlot: Get the stack in the requested inventory slot.
    //
    // Param:
    //      nIndex: An int for which slot to select.
    //
    // Return:
    //      ItemStack: Return the item stack from that index in the inventory
    //--------------------------------------------------------------------------------------
    public ItemStack GetStackInSlot(int nIndex)
    {
        // return stack from index
        return m_aoItems[nIndex];
    }

    //--------------------------------------------------------------------------------------
    // GetInventory: Get the inventory list.
    //
    // Return:
    //      List<ItemStack>: return the inventory list.
    //--------------------------------------------------------------------------------------
    public List<ItemStack> GetInventory()
    {
        // return the inventory
        return m_aoItems;
    }

    //--------------------------------------------------------------------------------------
    // SetIncompatibleItems: Set the incompatible item list.
    //
    // Param:
    //      aeItems: The items to set to the incompatible items list.
    //--------------------------------------------------------------------------------------
    public void SetIncompatibleItems(List<EItemType> aeItems)
    {
        // set the incompatible items
        m_aeIncompatibleItems = aeItems;
    }

    //--------------------------------------------------------------------------------------
    // GetIncompatibleItems: Get the list of incompatible items
    //
    // Return:
    //      List<EItemType>: return the list of incompatible items
    //--------------------------------------------------------------------------------------
    public List<EItemType> GetIncompatibleItems()
    {
        // return incompatible items
        return m_aeIncompatibleItems;
    }

    //--------------------------------------------------------------------------------------
    // CheckIfIncompatible: Check if an item is incompatible with an inventory.
    //
    // Param:
    //      eCheck: The Enum value representing the item to check.
    //      aeIncompatibleList: The incompatible items list to check the item against.
    //
    // Return:
    //      bool: Return if the item is compatible with the inventory or not.
    //--------------------------------------------------------------------------------------
    public bool CheckIfIncompatible(EItemType eCheck, List<EItemType> aeIncompatibleList)
    {
        // loop through the incompatible items list
        for (int i = 0; i < aeIncompatibleList.Count; i++)
        {
            // if the incompatible items list has a ETYPE_ALL return true
            if (aeIncompatibleList[i] == EItemType.ETYPE_ALL)
                return true;

            // if the eCheck value matches and incompatible item and that value is not ETYPE_NONE return true
            if (eCheck == aeIncompatibleList[i] && aeIncompatibleList[i] != EItemType.ETYPE_NONE)
                return true;
        }

        // else return false
        return false;
    }
}