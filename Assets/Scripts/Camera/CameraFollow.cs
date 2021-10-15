//--------------------------------------------------------------------------------------
// Purpose: Sets an object for a camera to follow.
//
// Description: Script takes in a target game object for the camera to follow. Main
// purpose for following the player around the screen.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

//--------------------------------------------------------------------------------------
// CameraFollow object. Inheriting from NetworkBehaviour.
//--------------------------------------------------------------------------------------
public class CameraFollow : NetworkBehaviour
{
    // PUBLIC VALUES //
    //--------------------------------------------------------------------------------------
    // Public Gameobject for the target the camera is to follow.
    [LabelOverride("Target")] [Tooltip("The target object for the camera to follow.")]
    public GameObject m_gTarget;
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private vector3 for the offset for the camera
    private Vector3 m_v3Offset;

    // private quaternion for setting the cameras inital rotation
    private Quaternion m_qInitRotation;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        // Check if current player object is the local player
        if (!IsLocalPlayer)
        {
            // turn off any camera object that is not the local player
            gameObject.SetActive(false);
        }

        else
        {
            // Set the initial rotation of this camera
            m_qInitRotation = transform.rotation;

            // set the offset value.
            m_v3Offset = transform.position - m_gTarget.transform.position;
        }
    }

    //--------------------------------------------------------------------------------------
    // LateUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void LateUpdate()
    {
        // Check if current player object is the local player
        if (IsLocalPlayer)
        {
            // ensure the camera does not rotate with the parent object.
            transform.rotation = m_qInitRotation;

            // Calc the new x and y position of the camera
            float fNewXPosition = m_gTarget.transform.position.x - m_v3Offset.x;
            float fNewYPosition = m_gTarget.transform.position.y - m_v3Offset.y;

            // Update the postion of the camera.
            transform.position = new Vector3(fNewXPosition, fNewYPosition, transform.position.z);
        }
    }
}