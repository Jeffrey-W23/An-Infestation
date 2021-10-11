//--------------------------------------------------------------------------------------
// Purpose: Bend the arm toward the cursor.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

//--------------------------------------------------------------------------------------
// Arm object. Inheriting from NetworkBehaviour.
//--------------------------------------------------------------------------------------
public class Arm : NetworkBehaviour
{
    // WEAPON SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Hand Settings:")]

    // public gameobject for the in hand item spawn location
    [LabelOverride("In-Hand Item Spawn")] [Tooltip("The spawn location of the In-Hand item.")]
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

    // private bool for freezing the arm
    private bool m_bFreezeArm = false;

    // private itemstack for the current hands item
    private ItemStack m_oInHandItemStack;

    // private gameobject for the item in player hand
    private GameObject m_gInHand;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    void Awake()
    {
        // set the in hand stack to empty.
        m_oInHandItemStack = ItemStack.m_oEmpty;
    }

    //--------------------------------------------------------------------------------------
    // LateUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    void LateUpdate()
    {
        // Check if current player object is the local player
        if (IsLocalPlayer)
        {
            // if arm is not frozen
            if (!m_bFreezeArm)
            {
                // Get mouse inside camera
                Vector3 v3Pos = transform.parent.Find("PlayerCamera").GetComponent<Camera>().WorldToScreenPoint(transform.position);

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

    //--------------------------------------------------------------------------------------
    // GetInHandItemStack: Get the current in hand item stack.
    //
    // Return:
    //      ItemStack: The item stack in hand
    //--------------------------------------------------------------------------------------
    public ItemStack GetInHandItemStack()
    {
        // return the in hand item stack
        return m_oInHandItemStack;
    }

    //--------------------------------------------------------------------------------------
    // SetInHandItemStack: Set the in hand item stack.
    //
    // Param:
    //      oItem: The item to set to the in hand item stack.
    //--------------------------------------------------------------------------------------
    public void SetInHandItemStack(ItemStack oItem)
    {
        // if the item is not empty
        if (oItem != ItemStack.m_oEmpty)
        {
            // set in the hand item stack to the passed in item
            m_oInHandItemStack = oItem;

            // set the in hand item to the hand
            SetInHandObject(m_oInHandItemStack.GetItem().m_gSceneObject);
        }

        // else if the item is empty
        else
        {
            // set in the hand item stack to the passed in item
            m_oInHandItemStack = oItem;

            // set the in hand item to null
            SetInHandObject(null);
        }
    }

    //--------------------------------------------------------------------------------------
    // SetInHand: Set the current in hand object.
    //
    // Param:
    //      gObject: The object to set to the hand.
    //--------------------------------------------------------------------------------------
    private void SetInHandObject(GameObject gObject)
    {
        // destroy the object currently in hand
        Object.Destroy(m_gInHand);

        // if the passed in object is not null
        if (gObject != null)
        {
            // set the in hand to the passed in object and instantiate
            m_gInHand = gObject;
            m_gInHand = Instantiate(m_gInHand);

            // Set the transform of the in hand item
            m_gInHand.transform.parent = transform;
            m_gInHand.transform.position = m_gInHandSpawn.transform.position;
            m_gInHand.transform.rotation = transform.rotation;
        }
    }
}