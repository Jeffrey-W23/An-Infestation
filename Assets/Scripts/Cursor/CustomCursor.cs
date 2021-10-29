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

    // STANDARD GETTERS / SETTERS //
    //--------------------------------------------------------------------------------------
    // Setter for setting the default cursor
    public void SetDefaultCursor() { SetCustomCursor(m_tCursor); }

    // Setter for setting the default crosshair
    public void SetCrosshair() { SetCustomCursor(m_tCrosshair); }

    // Setter for setting the previous cursor
    public void SetPreviousCursor() { SetCustomCursor(m_tPreviousCursor); }

    // Getter of type Texture2D for Current Cursor value
    public Texture2D GetCurrentCursor() { return m_tCurrentCursor; }

    // Getter of type Texture2D for the Previous Cursor value
    public Texture2D GetPreviousCursor() { return m_tPreviousCursor; }
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
}