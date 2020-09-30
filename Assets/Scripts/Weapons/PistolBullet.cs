//--------------------------------------------------------------------------------------
// Purpose: The main logic for Pistol bullet object.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// PistolBullet object. Inheriting from Bullet.
//--------------------------------------------------------------------------------------
public class PistolBullet : Bullet
{
    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    new void Awake()
    {
        // Run the base awake
        base.Awake();
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    new void FixedUpdate()
    {
        // Run the base update
        base.FixedUpdate();
    }
}