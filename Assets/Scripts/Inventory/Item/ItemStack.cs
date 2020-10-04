//--------------------------------------------------------------------------------------
// Purpose: The main logic for an item stack
//
// Description: This script is mostly used for getting and setting the item stack which
// is used to create a stack of items for an inventory. Every item will be an item stack,
// even with nothing in the stack.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// ItemStack object.
//--------------------------------------------------------------------------------------
public class ItemStack
{
    // public static itemstack for an empty item stack
    public static ItemStack m_oEmpty = new ItemStack();

    // public item for the item of this item stack
    public Item m_oItem;

    // public int for the count of this item stack
    public int m_nItemCount;

    // public int for the slot if of this item stack
    public int m_nSlotId;

    //--------------------------------------------------------------------------------------
    // Default Constructor.
    //--------------------------------------------------------------------------------------
    public ItemStack()
    {
        // set default values
        m_oItem = null;
        m_nItemCount = 0;
        m_nSlotId = -1;
    }

    //--------------------------------------------------------------------------------------
    // Constructor.
    //
    // Param:
    //      nSlotId: An int for passing in a slot id.
    //--------------------------------------------------------------------------------------
    public ItemStack(int nSlotId)
    {
        // set default values
        m_oItem = null;
        m_nItemCount = 0;
        m_nSlotId = nSlotId;
    }

    //--------------------------------------------------------------------------------------
    // Constructor.
    //
    // Param:
    //      oItem: An Item for passing in an item object.
    //      nItemCount: An int for passing in the item count.
    //--------------------------------------------------------------------------------------
    public ItemStack(Item oItem, int nItemCount)
    {
        // set default values
        m_oItem = oItem;
        m_nItemCount = nItemCount;
        m_nSlotId = -1;
    }

    //--------------------------------------------------------------------------------------
    // Constructor.
    //
    // Param:
    //      oItem: An Item of passing in an item object.
    //      nItemCount: An int for passing in the item count.
    //      nSlotId: An int for passing in a slot id.
    //--------------------------------------------------------------------------------------
    public ItemStack(Item oItem, int nItemCount, int nSlotId)
    {
        // set default values
        m_oItem = oItem;
        m_nItemCount = nItemCount;
        m_nSlotId = nSlotId;
    }

    //--------------------------------------------------------------------------------------
    // GetItem: Get the item of which this stack is made up of.
    //
    // Return:
    //      Item: item in the stack
    //--------------------------------------------------------------------------------------
    public Item GetItem()
    {
        // return the item
        return m_oItem;
    }

    //--------------------------------------------------------------------------------------
    // GetItemCount: Get the item count of the item stack.
    //
    // Return:
    //      int: the number of items in the stack.
    //--------------------------------------------------------------------------------------
    public int GetItemCount()
    {
        // return the item count
        return m_nItemCount;
    }

    //--------------------------------------------------------------------------------------
    // SetItemCount: Set the item count of the item stack.
    //
    // Param:
    //      nAmount: An int for the amount to set the item count.
    //--------------------------------------------------------------------------------------
    public void SetItemCount(int nAmount)
    {
        // set the count of the item
        m_nItemCount = nAmount;
    }

    //--------------------------------------------------------------------------------------
    // SetStack: Set up an item stack by assigning an item and a item count.
    //
    // Param:
    //      oStack: An ItemStack for what to set the current itemstack.
    //--------------------------------------------------------------------------------------
    public void SetStack(ItemStack oStack)
    {
        // set the item to the passed in stacks item
        m_oItem = oStack.GetItem();

        // set the count to the passed in stacks count
        m_nItemCount = oStack.GetItemCount();
    }

    //--------------------------------------------------------------------------------------
    // SplitStack: Split the item stack in two.
    //
    // Param:
    //      nAmount: An int for the amount to split the item stack.
    //
    // Return:
    //      ItemStack: Return the new item stack that was created with the split.
    //--------------------------------------------------------------------------------------
    public ItemStack SplitStack(int nAmount)
    {
        // get the min between amount and count
        int i = Mathf.Min(nAmount, m_nItemCount);

        // make a copy of the current stack
        ItemStack oStackCopy = CopyStack();

        // set the copied stack count to result of min and decrease current stack count
        oStackCopy.SetItemCount(i);
        DecreaseStack(i);

        // return copied stack
        return oStackCopy;
    }

    //--------------------------------------------------------------------------------------
    // CopyStack: Copy the current item stack.
    //
    // Return:
    //      ItemStack: returns the copy made of the current stack.
    //--------------------------------------------------------------------------------------
    public ItemStack CopyStack()
    {
        //return a copy of the stack
        return new ItemStack(m_oItem, m_nItemCount, m_nSlotId);
    }

    //--------------------------------------------------------------------------------------
    // IsStackEmpty: Check if the item stack is currently empty.
    //
    // Return:
    //      bool: true of false for if the stack is empty.
    //--------------------------------------------------------------------------------------
    public bool IsStackEmpty()
    {
        // return if the stack is empty or not
        return m_nItemCount < 1;
    }

    //--------------------------------------------------------------------------------------
    // IsItemEqual: Compare the item in two item stacks to check if they are the same.
    //
    // Param:
    //      oStack: An ItemStack which is to be compared.
    //
    // Return:
    //      bool: true or false if these item stacks are the same.
    //--------------------------------------------------------------------------------------
    public bool IsItemEqual(ItemStack oStack)
    {
        // Check if an item is equal
        return !oStack.IsStackEmpty() && m_oItem == oStack.GetItem();
    }

    //--------------------------------------------------------------------------------------
    // AreItemsEqual: Comapre the items in two item stacks to check if they are the same.
    // unlike IsItemEqual, this function is used to check comparisions more closely.
    //
    // Param:
    //      oStack1: An ItemStack for first object to compare.
    //      oStack2: An ItemStack for the second object to compare.
    //
    // Return:
    //      bool: true or false if these items stacks are equal.
    //--------------------------------------------------------------------------------------
    public static bool AreItemsEqual(ItemStack oStack1, ItemStack oStack2)
    {
        // check if items are equal in different stacks.
        return oStack1 == oStack2 ? true : (!oStack1.IsStackEmpty() && !oStack2.IsStackEmpty()
            ? oStack1.IsItemEqual(oStack2) : false);
    }

    //--------------------------------------------------------------------------------------
    // IsItemAddable: Check if an item can be added to an ItemStack, if there is space.
    //
    // Param:
    //      nAmount: An int for the amount of items wishing to be added.
    //
    // Return:
    //      bool: true or false if an add is acceptable.
    //--------------------------------------------------------------------------------------
    public bool IsItemAddable(int nAmount)
    {
        // check if the stack has room for another item
        return (m_nItemCount + nAmount) <= m_oItem.m_nMaxStackSize;
    }

    //--------------------------------------------------------------------------------------
    // IncreaseStack: Increase the item count of the item stack.
    //
    // Param:
    //      nAmount: An int for the amount to increase the item stack.
    //--------------------------------------------------------------------------------------
    public void IncreaseStack(int nAmount)
    {
        // increase the count of the stack
        m_nItemCount += nAmount;
    }

    //--------------------------------------------------------------------------------------
    // DecreaseStack: Decrease the item count of he item stack.
    //
    // Param:
    //      nAmount: An int for the amount to decrease the item stack.
    //--------------------------------------------------------------------------------------
    public void DecreaseStack(int nAmount)
    {
        // drecrease the count of the stack
        m_nItemCount -= nAmount;
    }
}