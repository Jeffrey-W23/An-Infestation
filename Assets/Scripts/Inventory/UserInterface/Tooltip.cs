//--------------------------------------------------------------------------------------
// Purpose: Display a tooltip for a hovered or selected item.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// Tooltip Object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class Tooltip : MonoBehaviour
{
    // TOOLTIP //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Tooltip Settings")]

    // public text object for showing the tooltip.
    [LabelOverride("Text Object")] [Tooltip("The text object used to show the tooltip.")]
    public Text m_tText;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private image for the tooltip back image.
    private Image m_iImage;

    // private bool for checking if the tooltip can activate.
    private bool m_bHovering;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Initialization.
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // get the image component
        m_iImage = GetComponent<Image>();

        // diabaled the image
        m_iImage.enabled = false;
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // if the mouse is hovering over an item
        if (m_bHovering)
        {
            // set the tooltip to follow the mouse
            transform.position = Input.mousePosition;
        }
    }

    //--------------------------------------------------------------------------------------
    // SetTooltip: Set the data needed to display the tooltip / activate the tooltip.
    //
    // Param:
    //      strTitle: The title of the item needing a tooltip.
    //--------------------------------------------------------------------------------------
    public void SetTooltip(string strTitle)
    {
        // if the string length is more than 0
        if (strTitle.Length > 0)
        {
            // actiavate the tooltip
            m_bHovering = true;

            // set the text of the tooltip 
            m_tText.text = strTitle;

            // enabled the image
            m_iImage.enabled = true;
        }

        // else if the length is less than 0
        else
        {
            // deactivate the tooltip
            m_bHovering = false;
            m_tText.text = string.Empty;
            m_iImage.enabled = false;
        }
    }
}