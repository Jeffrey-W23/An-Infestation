//--------------------------------------------------------------------------------------
// Purpose: Sets an object for the camera to follow.
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

//--------------------------------------------------------------------------------------
// CameraFollow object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class CameraFollow : MonoBehaviour
{
    // PUBLIC VALUES //
    //--------------------------------------------------------------------------------------
    // Public Gameobject for the target the camera is to follow.
    [LabelOverride("Target")] [Tooltip("The object you want the camera to follow.")]
    public GameObject m_gTarget;
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private vector3 for the offset for the camera
    private Vector3 m_v3Offset;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // set the offset value.
        m_v3Offset = transform.position - m_gTarget.transform.position;
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // Calc the new x and y position of the camera
        float fNewXPosition = m_gTarget.transform.position.x - m_v3Offset.x;
        float fNewYPosition = m_gTarget.transform.position.y - m_v3Offset.y;

        // Update the postion of the camera.
        transform.position = new Vector3(fNewXPosition, fNewYPosition, transform.position.z);
    }
}