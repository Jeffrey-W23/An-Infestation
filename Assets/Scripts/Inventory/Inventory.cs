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
    ETYPE_GUN = 1 << 0,
    ETYPE_BULLET = 1 << 1 
}

//--------------------------------------------------------------------------------------
// Inventory object.
//--------------------------------------------------------------------------------------
public class Inventory
{
    // private List of item stack: the inventory array.
    private List<ItemStack> m_aoItems = new List<ItemStack>();

    // private enum item type for list of incompatible items in inventory
    [EnumFlags] // Used to allow multiple enums in one.
    public EItemType m_eIncompatibleItems;

    //--------------------------------------------------------------------------------------
    // Default Constructor.
    //
    // Param:
    //      nSize: An int for the size of the inventory system to create.
    //--------------------------------------------------------------------------------------
    public Inventory(int nSize, EItemType aeCompatibleItems)
    {
        // loop through the inventory size
        for (int i = 0; i < nSize; i++)
        {
            // add an item stack for every item
            m_aoItems.Add(new ItemStack(i));
        }

        // set the incompatible items of this inventory
        SetIncompatibleItems(aeCompatibleItems);
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
        if (oStack.GetItem().m_eItemType == m_eIncompatibleItems)
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
    //      eItems: The items to set to the incompatible items list.
    //--------------------------------------------------------------------------------------
    public void SetIncompatibleItems(EItemType eItems)
    {
        // set the incompatible items
        m_eIncompatibleItems = eItems;
    }

    //--------------------------------------------------------------------------------------
    // GetIncompatibleItems: Get the list of incompatible items
    //
    // Return:
    //      EItemType: return the list of incompatible items
    //--------------------------------------------------------------------------------------
    public EItemType GetIncompatibleItems()
    {
        // return incompatible items
        return m_eIncompatibleItems;
    }
}