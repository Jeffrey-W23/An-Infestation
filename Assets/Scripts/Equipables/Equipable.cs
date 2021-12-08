//--------------------------------------------------------------------------------------
// Purpose: The main logic for the Equipable object.
//
// Description: This script is the base object for all Equipable items in the project.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using MLAPI;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Equipable object. Inheriting from NetworkBehaviour.
//--------------------------------------------------------------------------------------
public class Equipable : NetworkBehaviour
{
    // CUSTOM FOV SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Custom FOV Settings:")]

    // public bool for if there is a custom fov for this equipable item
    [LabelOverride("Has Custom FOV?")] [Tooltip("Does this equipable item have a custom Field Of View?")]
    public bool m_bHasCustomFOV = false;

    // public bool for is the fov can be toggled or not
    [LabelOverride("Does FOV Toggle?")] [Tooltip("Does this custom Field Of View toggle or enable when equipped?")]
    public bool m_bCanFOVToggle = false;

    //  public float for smoothing the lerp for fov
    [LabelOverride("FOV Transition Smoothing")] [Tooltip("Smoothing value for transitioning the fov from default settings to custom settings.")]
    public float m_fCustomFOVSmoothing = 4.0f;

    // public float for smoothing the lerp for the camera
    [LabelOverride("Camera Transition Smoothing")] [Tooltip("Smoothing value for transitioning the camera from default to custom FOV settings.")]
    public float m_fCustomCameraSmoothing = 4.0f;

    // public float for the distance to set when down sights
    [LabelOverride("Distance")] [Tooltip("The distance of the custom field of view.")]
    public float m_fCustomFOVDistance = 20.0f;

    // public float for the field of view when down sights
    [LabelOverride("Field Of View")] [Tooltip("The width of the custom field of view.")]
    public float m_fCustomFOVWidth = 30.0f;

    // public float for the zoom of the camera when down sights
    [LabelOverride("Zoom")] [Tooltip("The zoom of the camera for the custom field of view.")]
    public float m_fCustomFOVZoom = 10.0f;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // CURSOR SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Cursor Settings:")]

    // public bool for if there is a custom cursor for this equipable item
    [LabelOverride("Has Custom Cursor?")] [Tooltip("Is there a custom cursor for this equipable item?")]
    public bool m_bCustomCursor = false;

    // public texture2d for the guns crosshair
    [LabelOverride("Custom Cursor")] [Tooltip("The cursor to use for this equipable item.")]
    public Texture2D m_tCustomCursor;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // INVENTORY SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Inventory Settings:")]

    // public item value for assigning the item that represents this equipable in the inventory system
    [LabelOverride("Item In Inventory")] [Tooltip("The item that represents this equipable item in the inventory system.")]
    public Item m_oItemInInventory;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PROTECTED VALUES //
    //--------------------------------------------------------------------------------------
    // playber object for getting player script
    protected Player m_oPlayer;
    //--------------------------------------------------------------------------------------

    // PRIVATE NETWORKED VARS //
    //--------------------------------------------------------------------------------------
    // new private network varialbe as type bool for toggling the custom FOV over the server
    protected NetworkVariableBool mn_bCustomFOVToggle = new NetworkVariableBool(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, false);
    //--------------------------------------------------------------------------------------

    // GETTERS / SETTERS //
    //--------------------------------------------------------------------------------------
    // Setter of type Player for setting the parent player of the equipable
    public void SetPlayerScript(Player oPlayer) { m_oPlayer = oPlayer; }
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    protected void Awake()
    {
        // set the custom cursor of this equipable
        if (m_bCustomCursor)
            CustomCursor.m_oInstance.SetCustomCursor(m_tCustomCursor);
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    protected void Update()
    {
        // is player allowed to move
        if (m_oPlayer != null && !m_oPlayer.GetFrozenStatus())
        {
            // if the fov is off set down sights to false
            if (!m_oPlayer.GetPlayerVisionScript().GetToggleState())
                mn_bCustomFOVToggle.Value = false;

            // Enable the custom field of view
            if (m_bHasCustomFOV)
                EnableCustomFOV();
        }
    }

    //--------------------------------------------------------------------------------------
    // EnableCustomFOV: Enable the custom Field Of View.
    //--------------------------------------------------------------------------------------
    protected void EnableCustomFOV()
    {
        // if the custom FOV is togglable
        if (m_bCanFOVToggle)
        {
            // Only let the local player toggle the custom fov
            if (m_oPlayer.IsLocalPlayer)
            {
                // Toggle the custom fov toggle network variable on right click
                if (Input.GetMouseButtonDown(1) && mn_bCustomFOVToggle.Value)
                    mn_bCustomFOVToggle.Value = false;
                else if (Input.GetMouseButtonDown(1) && !mn_bCustomFOVToggle.Value)
                    mn_bCustomFOVToggle.Value = true;
            }
        }

        // else if the custom FOV is not togglable set the custom FOV to on
        else
            mn_bCustomFOVToggle.Value = true;

        // Activate the custom field of view
        if (mn_bCustomFOVToggle.Value)
        {
            // Pass adjustments to the FOV system
            m_oPlayer.GetPlayerVisionScript().AdjustFOV(m_fCustomFOVDistance, m_fCustomFOVWidth, m_fCustomFOVZoom, m_fCustomFOVSmoothing, m_fCustomCameraSmoothing);
            m_oPlayer.GetEnemyRendererScript().AdjustFOV(m_fCustomFOVDistance, m_fCustomFOVWidth, m_fCustomFOVZoom, m_fCustomFOVSmoothing, m_fCustomCameraSmoothing);
        }

        // Deactivate field of view, set everything back to default
        else
            m_oPlayer.SetFOVDefault();
    }
}