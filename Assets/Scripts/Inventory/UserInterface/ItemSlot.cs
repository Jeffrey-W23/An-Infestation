//--------------------------------------------------------------------------------------
// Purpose: The main logic of an Item Slot in a inventory container. 
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// ItemSlot object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler // Get handlers for mouse ui methods.
{
    // SLOT //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Slot Settings:")]

    // public image for the slot icon
    [LabelOverride("Icon Object")] [Tooltip("The child object used for displaying the item icon.")]
    public Image m_iIcon;

    // public text for the item count
    [LabelOverride("Count Object")] [Tooltip("The child object used for displaying the item count.")]
    public Text m_tCount;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private id for the item slot Id
    private int m_nId;

    // private item stack for current stack
    private ItemStack m_oCurrentStack;

    // private container for current open container
    private Container m_oCurrentContainer;

    // private inventory manager instance
    private InventoryManager m_oInventoryManger;

    // private inventory for the item slot inventory
    private Inventory m_oInventory;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // SetSlot: Set up the item slot.
    //
    // Param:
    //      oInventory: An Inventory, The inventory to set the slot.
    //      nId: An int, The id for the slot.
    //      oContainer: An Container, the container to set.
    //--------------------------------------------------------------------------------------
    public void SetSlot(Inventory oInventory, int nId, Container oContainer)
    {
        // set the id and container
        m_nId = nId;
        m_oCurrentContainer = oContainer;

        // set the incompatible items
        m_oInventory = oInventory;

        // set the current stack to the stack in the inventory
        m_oCurrentStack = oInventory.GetStackInSlot(nId);

        // get the inventory manager instance
        m_oInventoryManger = InventoryManager.m_oInstance;

        // update the slot
        UpdateSlot();
    }

    //--------------------------------------------------------------------------------------
    // SetSlotContent: Set the stack of the item slot.
    //
    // Param:
    //      oStack: the item stack to set to the item slot.
    //--------------------------------------------------------------------------------------
    private bool SetSlotContent(ItemStack oStack)
    {
        // ensure the stack is not empty
        if (!oStack.IsStackEmpty())
        {
            // if the item is incompatible
            if (m_oInventory.CheckIfIncompatible(oStack.GetItem().m_eItemType, m_oInventory.GetIncompatibleItems()))
            {
                // return false, slot not set.
                return false;
            }
        }

        // set the current stack to passed in stack
        m_oCurrentStack.SetStack(oStack);

        // update the slot
        UpdateSlot();

        // return true for successful slot setting
        return true;
    }

    //--------------------------------------------------------------------------------------
    // SetTooltip: Set the tooltip of the item slot.
    //
    // Param:
    //      strTitle: A string, title of the tooltip.
    //--------------------------------------------------------------------------------------
    private void SetTooltip(string strTitle)
    {
        // activate passed in tooltip
        m_oInventoryManger.ActivateToolTip(strTitle);
    }

    //--------------------------------------------------------------------------------------
    // UpdateSlot: Check / update the item slot.
    //--------------------------------------------------------------------------------------
    public void UpdateSlot()
    {
        // check if the current stack is empty
        if (!m_oCurrentStack.IsStackEmpty())
        {
            // enabled the slot icon
            m_iIcon.enabled = true;
            m_iIcon.sprite = m_oCurrentStack.GetItem().m_sIcon;
            
            // make sure the slot icon is the same height and wisth of the item icon
            m_iIcon.GetComponent<RectTransform>().sizeDelta = new Vector2
                (m_oCurrentStack.GetItem().m_sIcon.rect.width, m_oCurrentStack.GetItem().m_sIcon.rect.height);

            // if the current stack item count is greater than 1
            if (m_oCurrentStack.GetItemCount() > 1)
            {
                // set the count text to the current stacks count
                m_tCount.text = m_oCurrentStack.GetItemCount().ToString();
            }

            // else if the current stack wasnt greater than 1
            else
            {
                // set the count text to string empty
                m_tCount.text = string.Empty;
            }
        }

        // else if the current stack is empty
        else
        {
            // set the icon to false
            m_iIcon.enabled = false;

            // set the count text to string empty
            m_tCount.text = string.Empty;
        }
    }

    //--------------------------------------------------------------------------------------
    // OnPointerDown: Called by the EventSystem when a PointerDown event occurs.
    //
    // Param:
    //      eventData: Current event data.
    //--------------------------------------------------------------------------------------
    public void OnPointerDown(PointerEventData eventData)
    {
        // get the currently selected stack
        ItemStack oCurrentSelectedStack = m_oInventoryManger.GetSelectedStack();

        // make a copy of the current stack
        ItemStack oCurrentStackCopy = m_oCurrentStack.CopyStack();

        // if the mouse is left click
        if (eventData.pointerId == -1)
        {
            // run the left click method
            OnLeftClick(oCurrentSelectedStack, oCurrentStackCopy);
        }

        // if the mouse is right click
        if (eventData.pointerId == -2)
        {
            // run the right click method
            OnRightClick(oCurrentSelectedStack, oCurrentStackCopy);
        }
    }

    //--------------------------------------------------------------------------------------
    // OnPointerEnter: Called by the EventSystem when the pointer enters the object 
    // associated with this EventTrigger.
    //
    // Param:
    //      eventData: Current event data.
    //--------------------------------------------------------------------------------------
    public void OnPointerEnter(PointerEventData eventData)
    {
        // get the currently selected item
        ItemStack oCurrentSelectedStack = m_oInventoryManger.GetSelectedStack();

        // if the current stack isnt empty and an item isnt selected
        if (!m_oCurrentStack.IsStackEmpty() && oCurrentSelectedStack.IsStackEmpty())
        {
            // activate the tooltip with the current stack information
            SetTooltip(m_oCurrentStack.GetItem().m_strTitle);
        }
    }

    //--------------------------------------------------------------------------------------
    // OnPointerExit: When the cursor exits the rect area of this selectable UI object.
    //
    // Param:
    //      eventData: Current event data.
    //--------------------------------------------------------------------------------------
    public void OnPointerExit(PointerEventData eventData)
    {
        // deactivated the tooltip
        SetTooltip(string.Empty);
    }

    //--------------------------------------------------------------------------------------
    // OnLeftClick: What the item slot will do on a left click.
    //
    // Param:
    //      oCurrentSelectedStack: an ItemStack of the currently slected stack.
    //      oCurrentStackCopy: and ItemStack of the copy of the item stack.
    //--------------------------------------------------------------------------------------
    private void OnLeftClick(ItemStack oCurrentSelectedStack, ItemStack oCurrentStackCopy)
    {
        // bool for if a slot is compatible or not
        bool bCompatibleSlot = false;

        // if the current stack is not empty and the selected stack is empty
        if (!m_oCurrentStack.IsStackEmpty() && oCurrentSelectedStack.IsStackEmpty())
        {
            // set the selected stack to the current stack copy
            m_oInventoryManger.SetSelectedStack(oCurrentStackCopy, m_oInventory);

            // Set the slot content to empty
            SetSlotContent(ItemStack.m_oEmpty);

            // deactivate the tooltip
            SetTooltip(string.Empty);
        }

        // if the current stack is empty and the selected stack is not empty
        if (m_oCurrentStack.IsStackEmpty() && !oCurrentSelectedStack.IsStackEmpty())
        {
            // Set the slot content to the current selected stack
            bCompatibleSlot = SetSlotContent(oCurrentSelectedStack);

            // if the slot is compatible
            if (bCompatibleSlot)
            {
                // set the current selected stack to empty
                m_oInventoryManger.SetSelectedStack(ItemStack.m_oEmpty, m_oInventory);

                // activate the tooltip
                SetTooltip(m_oCurrentStack.GetItem().m_strTitle);
            }
        }

        // if both current stack and selected stack are not empty
        if (!m_oCurrentStack.IsStackEmpty() && !oCurrentSelectedStack.IsStackEmpty())
        {
            // are the stacks equal
            if (ItemStack.AreItemsEqual(oCurrentStackCopy, oCurrentSelectedStack))
            {
                // Can you add to the stack copy
                if (oCurrentStackCopy.IsItemAddable(oCurrentSelectedStack.GetItemCount()))
                {
                    // increase the current stack copy count by the selected stack count
                    oCurrentStackCopy.IncreaseStack(oCurrentSelectedStack.GetItemCount());

                    // set the slot conent to the current stack copy
                    bCompatibleSlot = SetSlotContent(oCurrentStackCopy);

                    // if the slot is compatible
                    if (bCompatibleSlot)
                    {
                        // set the currently selected stack to empty
                        m_oInventoryManger.SetSelectedStack(ItemStack.m_oEmpty, m_oInventory);

                        // activate the tooltip
                        SetTooltip(m_oCurrentStack.GetItem().m_strTitle);
                    }
                }

                // else if the item is not addable
                else
                {
                    // new int var, get the difference between current copied stak and currently selected stack
                    int nDifference = (oCurrentStackCopy.GetItemCount() + oCurrentSelectedStack.GetItemCount()) - oCurrentStackCopy.GetItem().m_nMaxStackSize;

                    // set the current copy stack count to the max stack size of the stacks item
                    oCurrentStackCopy.SetItemCount(m_oCurrentStack.GetItem().m_nMaxStackSize);

                    // get a copy of the currently selected stack
                    ItemStack oCurrentSelectedStackCopy = oCurrentSelectedStack.CopyStack();

                    // set the count of the copied current selected stack to the differnce
                    oCurrentSelectedStackCopy.SetItemCount(nDifference);

                    // set the content of the stack to the current stack copy
                    bCompatibleSlot = SetSlotContent(oCurrentStackCopy);

                    // if the slot is compatible
                    if (bCompatibleSlot)
                    {
                        // set the currently selected stack to the current selected stack copy
                        m_oInventoryManger.SetSelectedStack(oCurrentSelectedStackCopy, m_oInventory);

                        // deactivate the tooltip
                        SetTooltip(string.Empty);
                    }
                }
            }

            // if the stacks arent equal
            else
            {
                // set the slot content to the currently selected stack
                bCompatibleSlot = SetSlotContent(oCurrentSelectedStack);

                // if the slot is compatible
                if (bCompatibleSlot)
                {
                    // set the currently selected stack to the current selected stack copy
                    m_oInventoryManger.SetSelectedStack(oCurrentStackCopy, m_oInventory);

                    // deactivate the tooltip
                    SetTooltip(string.Empty);
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // OnRightClick: What the item slot will do on a right click.
    //
    // Param:
    //      oCurrentSelectedStack: an ItemStack of the current stack.
    //      oCurrentStackCopy: and ItemStack of the copy of the item stack.
    //--------------------------------------------------------------------------------------
    private void OnRightClick(ItemStack oCurrentSelectedStack, ItemStack oCurrentStackCopy)
    {
        // bool for if a slot is compatible or not
        bool bCompatibleSlot = false;

        // if the current stack is not empty and the selected stack is empty
        if (!m_oCurrentStack.IsStackEmpty() && oCurrentSelectedStack.IsStackEmpty())
        {
            // split the current stack in half
            ItemStack oSplitStack = oCurrentStackCopy.SplitStack(oCurrentStackCopy.GetItemCount() / 2);

            // set the currently selected stack to the split stack.
            m_oInventoryManger.SetSelectedStack(oSplitStack, m_oInventory);

            // set the contentto the copy of the current stack.
            SetSlotContent(oCurrentStackCopy);

            // deactivate the tooltip
            SetTooltip(string.Empty);
        }

        // if the current stack is empty and the selected stack is not empty
        if (m_oCurrentStack.IsStackEmpty() && !oCurrentSelectedStack.IsStackEmpty())
        {
            // Set the slot content of a new item stack
            bCompatibleSlot = SetSlotContent(new ItemStack(oCurrentSelectedStack.GetItem(), 1));

            // if the slot is compatible
            if (bCompatibleSlot)
            {
                // get a copy of the currently selected stack
                ItemStack oCurrentSelectedStackCopy = oCurrentSelectedStack.CopyStack();

                // decrease the currently selected count by 1
                oCurrentSelectedStackCopy.DecreaseStack(1);

                // set the currently selected stack in the manager to this one 
                m_oInventoryManger.SetSelectedStack(oCurrentSelectedStackCopy, m_oInventory);

                // deactivate the tooltip
                SetTooltip(string.Empty);
            }
        }

        // if both current stack and selected stack are not empty
        if (!m_oCurrentStack.IsStackEmpty() && !oCurrentSelectedStack.IsStackEmpty())
        {
            // if the items stacks are equal
            if (ItemStack.AreItemsEqual(oCurrentStackCopy, oCurrentSelectedStack))
            {
                // if the current stack has room to add one item
                if (m_oCurrentStack.IsItemAddable(1))
                {
                    // increase the count of the current stack
                    oCurrentStackCopy.IncreaseStack(1);

                    // set the content of the current slot
                    bCompatibleSlot = SetSlotContent(oCurrentStackCopy);

                    // if the slot is compatible
                    if (bCompatibleSlot)
                    {
                        // get a copy of the currently selected stack
                        ItemStack oCurrentSelectedStackCopy = oCurrentSelectedStack.CopyStack();

                        // decrease the count
                        oCurrentSelectedStackCopy.DecreaseStack(1);

                        // set the currently selected stack in the manager to this new one 
                        m_oInventoryManger.SetSelectedStack(oCurrentSelectedStackCopy, m_oInventory);

                        // deactivate the tooltip
                        SetTooltip(string.Empty);
                    }
                }
            }
        }
    }
}