//--------------------------------------------------------------------------------------
// Purpose: Logic for the currently selected item stack in an inventory.
//
// Description: This is used for setting and getting the currently selected item stack in
// an inventory container, not to be confused with the hotbars currently selected which is
// just for item usage, where as this selected stack is for inventory management.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// SelectedStack Object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class SelectedStack : MonoBehaviour
{
    // CURRENT SELECTED //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Current Selected Settings")]

    // public image for displaying the item icon.
    [LabelOverride("Selected Icon")] [Tooltip("The child object used for displaying the currently selected item icon.")]
    public Image m_iIcon;

    // public text object for the displaying item count.
    [LabelOverride("Selected Count")] [Tooltip("The child object used for displaying the currently selected item count.")]
    public Text m_tCount;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private item stack for the current selected item stack.
    private ItemStack m_oCurrentStack = ItemStack.m_oEmpty;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // update the selected stack
        UpdateSelectedStack();

        // set the postion to follow the mosue
        transform.position = Input.mousePosition;
    }

    //--------------------------------------------------------------------------------------
    // SetSelectedStack: Set the currently selected item stack.
    //
    // Param:
    //      oStack: and Item Stack for what to set the selected item.
    //--------------------------------------------------------------------------------------
    public void SetSelectedStack(ItemStack oStack)
    {
        // set the current stack to the selected stack
        m_oCurrentStack = oStack;
    }

    //--------------------------------------------------------------------------------------
    // UpdateSelectedStack: Check and update the currently selected item stack.
    //--------------------------------------------------------------------------------------
    private void UpdateSelectedStack()
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
            // diabled the selected stack
            DisableSelectedStack();
        }
    }

    //--------------------------------------------------------------------------------------
    // DisableSelectedStack: Disable the currently selected item icons and count.
    //--------------------------------------------------------------------------------------
    private void DisableSelectedStack()
    {
        // set the icon to false
        m_iIcon.enabled = false;

        // set the count text to string empty
        m_tCount.text = string.Empty;
    }
}