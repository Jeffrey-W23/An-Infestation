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
    // PUBLIC VALUES //
    //--------------------------------------------------------------------------------------
    // public list of enums for incompatible items in an inventory
    public List<EItemType> m_aeIncompatibleItems = new List<EItemType>();
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private List of item stack: the inventory array.
    private List<ItemStack> m_aoItems = new List<ItemStack>();

    // private gameobject for the owner of this inventory
    private GameObject m_gPlayerObject;
    //--------------------------------------------------------------------------------------

    // DELEGATES //
    //--------------------------------------------------------------------------------------
    // Create a new Delegate for handling the what happens when the inventory is updated.
    public delegate void ItemUpdatedEventHandler(int nIndex);

    // ItemStack Added event callback 
    public event ItemUpdatedEventHandler OnItemStackAddedCallback;

    // ItemStack Updated event callback
    public event ItemUpdatedEventHandler OnItemStackUpdatedCallback;

    // ItemStack Removed event callback
    public event ItemUpdatedEventHandler OnItemStackRemovedCallback;
    //--------------------------------------------------------------------------------------

    // GETTERS / SETTERS //
    //--------------------------------------------------------------------------------------
    // Getter of type GameObject for getting the player gameobject.
    public GameObject GetPlayerObject() { return m_gPlayerObject; }

    // Getter of type ItemStack for the getting an inventory item by index
    public ItemStack GetStackInSlot(int nIndex) { return m_aoItems[nIndex]; }

    // Getter of type ItemStack List for getting the inventory array
    public List<ItemStack> GetArray() { return m_aoItems; }

    // Getter of type EItemType List for getting the incompatible items
    public List<EItemType> GetIncompatibleItems() { return m_aeIncompatibleItems; }

    // Setter of type EItemType List for setting the incompatible items
    public void SetIncompatibleItems(List<EItemType> aeItems) { m_aeIncompatibleItems = aeItems; }
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Default Constructor.
    //
    // Param:
    //      nSize: An int for the size of the inventory system to create.
    //      aeIncompatibleItems: An array of ItemType enums for incompatible items.
    //      gPlayer: A gameobject for the player object who owns this inventory.
    //--------------------------------------------------------------------------------------
    public Inventory(int nSize, List<EItemType> aeIncompatibleItems, GameObject gPlayer)
    {
        // loop through the inventory size
        for (int i = 0; i < nSize; i++)
        {
            // add an item stack for every item
            m_aoItems.Add(new ItemStack(i));
        }

        // set the incompatible and compatible items of this inventory
        SetIncompatibleItems(aeIncompatibleItems);

        // Set the player object of this inventory
        m_gPlayerObject = gPlayer;
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
        for (int i = 0; i < m_aoItems.Count; i++)
        {
            // is the stack empty
            if (m_aoItems[i].IsStackEmpty())
            {
                // set the stack to passed in stack
                m_aoItems[i].SetStack(oStack);

                // Call Item Stack Added Callback and pass 
                // in item that is being added to the inventory
                if (OnItemStackAddedCallback != null)
                    OnItemStackAddedCallback(i);

                // return true, item added
                return true;
            }

            // is the stack equal to passed in stack
            if (ItemStack.AreItemsEqual(oStack, m_aoItems[i]))
            {
                // is it possible to add items to this stack
                if (m_aoItems[i].IsItemAddable(oStack.GetItemCount()))
                {
                    // increase the stack count
                    m_aoItems[i].IncreaseStack(oStack.GetItemCount());

                    // Call Item Stack Updated Callback and pass 
                    // in item that is being updated in the inventory
                    if (OnItemStackUpdatedCallback != null)
                        OnItemStackUpdatedCallback(i);

                    // return true, item added
                    return true;
                }

                // else if the item is not addable
                else
                {
                    // new int var, get the difference between passed in stack and current stack
                    int nDifference = (m_aoItems[i].GetItemCount() + oStack.GetItemCount()) - m_aoItems[i].GetItem().m_nMaxStackSize;

                    // set the count of the stack to the max stack size of the stacks item
                    m_aoItems[i].SetItemCount(m_aoItems[i].GetItem().m_nMaxStackSize);

                    // set the count of the passed in stack to the stack differnce
                    oStack.SetItemCount(nDifference);

                    // Call Item Stack Updated Callback and pass 
                    // in item that is being updated in the inventory
                    if (OnItemStackUpdatedCallback != null)
                        OnItemStackUpdatedCallback(i);
                }
            }
        }

        // return false, item not added.
        return false;
    }

    //--------------------------------------------------------------------------------------
    // AddItemAtPosition: Add an item to the inventory system at a set position.
    //
    // Param:
    //      oStack: The item stack to add to the inventory system.
    //      nIndex: The position in the inventory to add the item.
    //
    // Return:
    //      bool: return the status of the item adding.
    //--------------------------------------------------------------------------------------
    public bool AddItemAtPosition(ItemStack oStack, int nIndex)
    {
        // check if the item attempting to add to the inventory is incompatible
        if (CheckIfIncompatible(oStack.GetItem().m_eItemType, m_aeIncompatibleItems))
        {
            // return false, item not added.
            return false;
        }

        // is the stack empty
        if (m_aoItems[nIndex].IsStackEmpty())
        {
            // set the stack to passed in stack
            m_aoItems[nIndex].SetStack(oStack);

            // Call Item Stack Added Callback and pass 
            // in item that is being added to the inventory
            if (OnItemStackAddedCallback != null)
                OnItemStackAddedCallback(nIndex);

            // return true, item added
            return true;
        }

        // is the stack equal to passed in stack
        if (ItemStack.AreItemsEqual(oStack, m_aoItems[nIndex]))
        {
            // is it possible to add items to this stack
            if (m_aoItems[nIndex].IsItemAddable(oStack.GetItemCount()))
            {
                // increase the stack count
                m_aoItems[nIndex].IncreaseStack(oStack.GetItemCount());

                // Call Item Stack Updated Callback and pass 
                // in item that is being updated in the inventory
                if (OnItemStackUpdatedCallback != null)
                    OnItemStackUpdatedCallback(nIndex);

                // return true, item added
                return true;
            }

            // else if the item is not addable
            else
            {
                // new int var, get the difference between passed in stack and current stack
                int nDifference = (m_aoItems[nIndex].GetItemCount() + oStack.GetItemCount()) - m_aoItems[nIndex].GetItem().m_nMaxStackSize;

                // set the count of the stack to the max stack size of the stacks item
                m_aoItems[nIndex].SetItemCount(m_aoItems[nIndex].GetItem().m_nMaxStackSize);

                // set the count of the passed in stack to the stack differnce
                oStack.SetItemCount(nDifference);

                // Call Item Stack Updated Callback and pass 
                // in item that is being updated in the inventory
                if (OnItemStackUpdatedCallback != null)
                    OnItemStackUpdatedCallback(nIndex);
            }
        }

        // return false, item not added.
        return false;
    }

    //--------------------------------------------------------------------------------------
    // RemoveItemAtPosition: Remove an item in the inventory system from a set position.
    //
    // Params:
    //      nIndex: The position in the inventory to remove the item.
    //--------------------------------------------------------------------------------------
    public void RemoveItemAtPosition(int nIndex)
    {
        // Call Item Stack Removed Callback and pass 
        // in item that is being remove from inventory
        if (OnItemStackRemovedCallback != null)
            OnItemStackRemovedCallback(nIndex);

        // Set the stack at the current index to empty
        m_aoItems[nIndex].SetStack(ItemStack.m_oEmpty);
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