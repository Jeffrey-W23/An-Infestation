//--------------------------------------------------------------------------------------
// Purpose: Bend the arm toward the cursor.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Arm object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class Arm : MonoBehaviour
{
    // WEAPON SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Hand Settings:")]







    // TEMP // TEMP // // TEMP // TEMP //
    // public gameobject for Weapon prefab.
    //[LabelOverride("Weapon Prefab")] [Tooltip("The weapon equiped to the player at start.")]
    //public GameObject m_gWeaponPrefab;
    // TEMP // TEMP // // TEMP // TEMP //





    public GameObject m_gInHand;
    public GameObject m_gInHandSpawn;






    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // ARM SETTING //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Arm Setting:")]

    // public int for arm bend value
    [LabelOverride("Arm Bend")] [Tooltip("The amount of bend on the arm towards the cursor.")]
    public int m_nArmBend = 120;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // float for the distance between mouse and object.
    private float m_fDistanceBetween;

    // The Pistol weapon.
    private GameObject m_gPistol;

    // private bool for freezing the arm
    private bool m_bFreezeArm = false;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    void Awake()
    {
        // Set the parenting of pistol prefab.
        //m_gPistol = Instantiate(m_gWeaponPrefab);
        //m_gPistol.transform.parent = transform;
    }

    //--------------------------------------------------------------------------------------
    // LateUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    void LateUpdate()
    {
        // if arm is not frozen
        if (!m_bFreezeArm)
        {
            // Get mouse inside camera
            Vector3 v3Pos = Camera.main.WorldToScreenPoint(transform.position);

            // update the distance.
            m_fDistanceBetween = Vector3.Distance(v3Pos, Input.mousePosition);

            // Check the distance between the mouse and arm.
            // if far enough away turn the mouse towards mouse.
            // else stop arm rotation.
            if (m_fDistanceBetween > m_nArmBend)
            {
                // Get the  mouse direction.
                Vector3 v3Dir = Input.mousePosition - v3Pos;

                // Work out the angle.
                float fAngle = Mathf.Atan2(v3Dir.y, v3Dir.x) * Mathf.Rad2Deg;

                // Update the rotation.
                transform.rotation = Quaternion.AngleAxis(fAngle, Vector3.forward);
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // GetFreezeArm: Get the current freeze status of the arm. 
    //
    // Return:
    //      bool: the current freeze staus of the arm.
    //--------------------------------------------------------------------------------------
    public bool GetFreezeArm()
    {
        // get the arm freeze bool
        return m_bFreezeArm;
    }

    //--------------------------------------------------------------------------------------
    // SetFreezeArm: Set the freeze status of the arm object. Used for ensuring the
    // arm stays still, good for open menus or possibly cut scenes, etc.
    //
    // Param:
    //      bFreeze: bool for setting the freeze status.
    //--------------------------------------------------------------------------------------
    public void SetFreezeArm(bool bFreeze)
    {
        // set the arm freeze bool
        m_bFreezeArm = bFreeze;
    }










    public GameObject GetInHand()
    {
        return m_gInHand;
    }

    public void SetInHand(GameObject gObject)
    {
        Object.Destroy(m_gInHand);

        if (gObject != null)
        {
            m_gInHand = gObject;
            m_gInHand = Instantiate(m_gInHand);

            m_gInHand.transform.parent = transform;
            m_gInHand.transform.position = m_gInHandSpawn.transform.position;
            m_gInHand.transform.rotation = transform.rotation;
        }
    }
}