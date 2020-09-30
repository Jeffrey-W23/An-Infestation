//--------------------------------------------------------------------------------------
// Purpose: The main logic for Pistol gun object.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Pistol object. Inheriting from BaseGun.
//--------------------------------------------------------------------------------------
public class Pistol : Gun
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
    new void Update()
    {
        // Run the base update
        base.Update();
    }
}