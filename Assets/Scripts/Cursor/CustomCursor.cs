//--------------------------------------------------------------------------------------
// Purpose: Sets the cursor of the scene.
//
// Description: Script is used for easily setting the cursor of the current scene.
// Script takes in a public texture and will set the hotpoint to center.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// CustomCursor object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class CustomCursor : MonoBehaviour
{
    // CURSORS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Cursors:")]

    // Public texture for the cursor visuals.
    [LabelOverride("Cursor")] [Tooltip("The cursor object to replace the default unity cursor.")]
    public Texture2D m_tCursor;

    // Public texture for the crosshair cursor visuals.
    [LabelOverride("Crosshair")] [Tooltip("The crosshair cursor object to replace the default unity cursor.")]
    public Texture2D m_tCrosshair;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PUBLIC HIDDEN //
    //--------------------------------------------------------------------------------------
    // new singleton for setting the cursor throughout project
    [HideInInspector]
    public static CustomCursor m_oInstance;
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private texture2d for the current cursor
    private Texture2D m_tCurrentCursor;

    // private texture2d for the previous cursor
    private Texture2D m_tPreviousCursor;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // set instance
        m_oInstance = this;

        // Set the cursor to the default
        SetCustomCursor(m_tCursor);

        // set the previous and current cursor
        m_tCurrentCursor = m_tCursor;
        m_tPreviousCursor = m_tCursor;
    }

    //--------------------------------------------------------------------------------------
    // SetCustomCursor: Set the current cursor of the scene
    // 
    // Param:
    //      tCursor: Texture2D to set the cursor.
    //--------------------------------------------------------------------------------------
    public void SetCustomCursor(Texture2D tCursor)
    {
        // Set the previous cursor to current cursor before it is changed.
        m_tPreviousCursor = GetCurrentCursor();

        // Set the mouse click point.
        Vector2 v2CursorHotspot = new Vector2(tCursor.width / 2, tCursor.height / 2);

        // Set the cursor values.
        Cursor.SetCursor(tCursor, v2CursorHotspot, CursorMode.ForceSoftware);

        // set the current cursor
        m_tCurrentCursor = tCursor;
    }

    //--------------------------------------------------------------------------------------
    // SetDefaultCursor: Sets the current cursor of the scene to the public default.
    //--------------------------------------------------------------------------------------
    public void SetDefaultCursor()
    {
        // Set the cursor to the desfault
        SetCustomCursor(m_tCursor);
    }

    //--------------------------------------------------------------------------------------
    // SetDefaultCursor: Sets the current cursor of the scene to the public default.
    //--------------------------------------------------------------------------------------
    public void SetCrosshair()
    {
        // Set the cursor to the desfault
        SetCustomCursor(m_tCrosshair);
    }

    //--------------------------------------------------------------------------------------
    // SetPrevious: Set the cursor to the previously used cursor.
    //--------------------------------------------------------------------------------------
    public void SetPreviousCursor()
    {
        // set the custom cursor to previous cursor
        SetCustomCursor(m_tPreviousCursor);
    }

    //--------------------------------------------------------------------------------------
    // GetCurrentCursor: Get the currently selected cursor.
    //
    // Return:
    //      Texture2D: returns the current cursor as a texture2D
    //--------------------------------------------------------------------------------------
    public Texture2D GetCurrentCursor()
    {
        // return the current cursor
        return m_tCurrentCursor;
    }

    //--------------------------------------------------------------------------------------
    // GetPreviousCursor: Get the previously selected cursor.
    //
    // Return:
    //      Texture2D: returns the previous cursor as a texture2D
    //--------------------------------------------------------------------------------------
    public Texture2D GetPreviousCursor()
    {
        // return the current cursor
        return m_tPreviousCursor;
    }
}