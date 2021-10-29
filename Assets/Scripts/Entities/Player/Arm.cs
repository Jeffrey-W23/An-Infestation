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
using MLAPI.Messaging;
using MLAPI.Spawning;
using MLAPI.NetworkVariable;

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

    // private inventory manager for the inventory manager instance
    private InventoryManager m_oInventoryManger;

    // private itemstack for the current hands item
    private ItemStack m_oInHandItemStack;

    // private gameobject for the item in player hand
    private GameObject m_gInHand;
    //--------------------------------------------------------------------------------------







    //
    private NetworkVariableInt mn_nItemIndex = new NetworkVariableInt(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, 0);

    //
    private NetworkVariableBool mn_bIsHoldingItem = new NetworkVariableBool(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, false);






    // GETTERS / SETTERS //
    //--------------------------------------------------------------------------------------
    // Getter of type bool for Freeze Arm value
    public bool GetFreezeArm() { return m_bFreezeArm; }

    // Getter of type ItemStack for In Hand Item Stack value
    public ItemStack GetInHandItemStack() { return m_oInHandItemStack; }

    // Setter of type bool for Freeze Arm value
    public void SetFreezeArm(bool bFreeze) { m_bFreezeArm = bFreeze; }
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        // get the inventory manager instance
        m_oInventoryManger = InventoryManager.m_oInstance;
    }

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // set the in hand stack to empty.
        m_oInHandItemStack = ItemStack.m_oEmpty;
    }

    //--------------------------------------------------------------------------------------
    // LateUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void LateUpdate()
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
    // OnEnable: Function that will call when this gameObject is enabled.
    //--------------------------------------------------------------------------------------
    private void OnEnable()
    {
        // subscribe to value change event for body color // CHANGE
        mn_bIsHoldingItem.OnValueChanged += OnHoldingItemChange;
    }

    //--------------------------------------------------------------------------------------
    // OnDestroy: Function that will call on this gameObjects destruction.
    //--------------------------------------------------------------------------------------
    private void OnDestroy()
    {
        // Unsubscribe from body colors on change event // CHANGE
        mn_bIsHoldingItem.OnValueChanged -= OnHoldingItemChange;
    }

    //--------------------------------------------------------------------------------------
    // OnHoldingItemChange: Event function for on Holding Item bool change.
    //
    // Params:
    //      bOldState: The previous bool state before the change event triggered.
    //      bNewState: The new bool state that triggered the event change.
    //--------------------------------------------------------------------------------------
    private void OnHoldingItemChange(bool bOldState, bool bNewState)
    {
        //
        if (!bNewState)
        {
            //
            //if (m_gInHand != null)
                //Destroy(m_gInHand);

            //
            UnequipItemServerRpc();
        }
    }






    //--------------------------------------------------------------------------------------
    // SetInHandItemStack: Set the in hand item stack.
    //
    // Param:
    //      oItem: The item to set to the in hand item stack.
    //--------------------------------------------------------------------------------------
    public void SetInHandItemStack(ItemStack oItem)
    {
        //
        m_oInHandItemStack = oItem;

        //
        foreach (var i in m_oInventoryManger.GetItemDatabase())
        {
            //
            if (i.Value == oItem.GetItem())
                mn_nItemIndex.Value = i.Key;
        }

        //
        if (!oItem.IsStackEmpty() && oItem.GetItem().m_gSceneObject != null)
        {
            //
            mn_bIsHoldingItem.Value = true;

            //
            EquipItemServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        //
        else
        {
            //
            mn_bIsHoldingItem.Value = false;
        }
    }

    //--------------------------------------------------------------------------------------
    // EquippedItemServerRpc:
    //--------------------------------------------------------------------------------------
    [ServerRpc]
    private void EquipItemServerRpc(ulong ulClientID)
    {
        //
        GameObject gItemObject = null;

        //
        foreach (var i in m_oInventoryManger.GetItemDatabase())
        {
            //
            if (i.Key == mn_nItemIndex.Value)
                gItemObject = i.Value.m_gSceneObject;
        }

        //
        GameObject oEquippingItem = Instantiate(gItemObject);
        oEquippingItem.GetComponent<NetworkObject>().SpawnWithOwnership(ulClientID);

        //
        ulong ulObjectNetworkID = oEquippingItem.GetComponent<NetworkObject>().NetworkObjectId;

        //
        EquipItemClientRpc(ulObjectNetworkID);
    }

    //--------------------------------------------------------------------------------------
    // EquippedItemClientRpc:
    //
    // Param:
    //      ulObjectNetworkID: 
    //--------------------------------------------------------------------------------------
    [ClientRpc]
    private void EquipItemClientRpc(ulong ulObjectNetworkID)
    {
        //
        NetworkObject noObject = NetworkSpawnManager.SpawnedObjects[ulObjectNetworkID];

        //
        m_gInHand = noObject.gameObject;

        // Set the transform of the in hand item
        m_gInHand.transform.parent = transform;
        m_gInHand.transform.position = m_gInHandSpawn.transform.position;
        m_gInHand.transform.rotation = transform.rotation;

        //
        m_gInHand.GetComponent<SpriteRenderer>().enabled = true;
    }

    //--------------------------------------------------------------------------------------
    // UnequipItemServerRpc:
    //--------------------------------------------------------------------------------------
    [ServerRpc(RequireOwnership = false)]
    private void UnequipItemServerRpc()
    {
        //
        UnequipItemClientRpc();
    }

    //--------------------------------------------------------------------------------------
    // UnequipItemClientRpc:
    //--------------------------------------------------------------------------------------
    [ClientRpc]
    private void UnequipItemClientRpc()
    {
        if (m_gInHand != null)
            Destroy(m_gInHand);
    }
}