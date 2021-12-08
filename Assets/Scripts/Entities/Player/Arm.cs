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
    private bool m_bFrozenStatus = false;

    // private bool for debug menu toggle
    private bool m_bDebugMode = false;

    // private inventory manager for the inventory manager instance
    private InventoryManager m_oInventoryManger;

    // private itemstack for the current hands item
    private ItemStack m_oEquippedItemStack;

    // private gameobject for the item in player hand
    private GameObject m_gEquippedItem;
    //--------------------------------------------------------------------------------------

    // PRIVATE NETWORKED VARS //
    //--------------------------------------------------------------------------------------
    // new private network bool for keeping track of if an item is currently equipped.
    private NetworkVariableBool mn_bIsItemEquipped = new NetworkVariableBool(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, false);

    // new private network variable ulong for keeping track of the current equipped items network ID
    private NetworkVariableULong mn_ulEquippedItemNetworkID = new NetworkVariableULong(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 0);
    //--------------------------------------------------------------------------------------

    // GETTERS / SETTERS //
    //--------------------------------------------------------------------------------------
    // Getter of type bool for Frozen Arm status value
    public bool GetFrozenStatus() { return m_bFrozenStatus; }

    // Getter of type ItemStack for In Equipped Item Stack value
    public ItemStack GetEquippedItemStack() { return m_oEquippedItemStack; }

    // Setter of type bool for Frozen Arm status value
    public void SetFrozenStatus(bool bStatus) { m_bFrozenStatus = bStatus; }

    // Setter of type bool for setting the debug mode status
    public void SetDebugMode(bool bStatus) { m_bDebugMode = bStatus; }
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        // get the inventory manager instance
        m_oInventoryManger = InventoryManager.m_oInstance;

        // Check if it is the local player connecting
        if (mn_ulEquippedItemNetworkID.Value != 0)
        {
            // Ensure the equipped item is set as the same one as the server before unequipping, 
            // important for when players join the game session late.
            m_gEquippedItem = NetworkSpawnManager.SpawnedObjects[mn_ulEquippedItemNetworkID.Value].gameObject;

            // Check if the item is currently equipped, if it isnt make sure it is set to inactive.
            if (!mn_bIsItemEquipped.Value)
                m_gEquippedItem.SetActive(false);
        }
    }

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // set the equipped item stack to empty.
        m_oEquippedItemStack = ItemStack.m_oEmpty;
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
            if (!m_bFrozenStatus)
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
    // EquipItem: Function for equipping and unequipping items for the player on the game 
    // server using server and client functions.
    //
    // Param:
    //      oItem: ItemStack value for the item to be equipped.
    //--------------------------------------------------------------------------------------
    public void EquipItem(ItemStack oItem)
    {
        // Send debug message to console
        if (m_bDebugMode)
            Debug.Log("EquipItem function call started!");

        // new int value for the item ID of the equipped item
        int nItemID = -1;

        // Loop through the inventory item database
        foreach (var i in m_oInventoryManger.GetItemDatabase())
        {
            // Set the item ID based on the passed in item object
            if (i.Value == oItem.GetItem())
                nItemID = i.Key;
        }

        // If the item is valid for equipping
        if (!oItem.IsStackEmpty() && oItem.GetItem().m_gSceneObject != null)
        {
            // If the status of the equipped item is true,
            // Run server rpc to unequip item on each of the clients
            if (mn_bIsItemEquipped.Value)
                UnequipItemServerRpc();
            
            // Set the equipped item stack to the passed in item
            m_oEquippedItemStack = oItem;

            // Run server rpc to equip item on each of the clients
            // passing in players current client ID and itemID
            EquipItemServerRpc(NetworkManager.Singleton.LocalClientId, nItemID);

            // Send debug message to console
            if (m_bDebugMode)
                Debug.Log("EquipItem function call finished with Equip Server Call!");
        }

        // else if not a valid item for equipping
        else if (mn_bIsItemEquipped.Value)
        {
            // Set the equipped item stack to empty
            m_oEquippedItemStack = ItemStack.m_oEmpty;

            // Run server rpc to unequip item on each of the clients
            UnequipItemServerRpc();

            // Send debug message to console
            if (m_bDebugMode)
                Debug.Log("EquipItem function call finished with Unequip Server Call!");
        }
    }

    //--------------------------------------------------------------------------------------
    // EquippedItemServerRpc: Server function for initating an equip event for all the clients
    //
    // Param:
    //      ulClientID: ulong value for the players client ID
    //      nItemID: int value for the nItemID in the inventory item database
    //--------------------------------------------------------------------------------------
    [ServerRpc]
    private void EquipItemServerRpc(ulong ulClientID, int nItemID)
    {
        // Send debug message to console
        if (m_bDebugMode)
            Debug.Log("ServerRpc for EquipItem Called!");

        // New null gameobject for the item to be equipped
        GameObject gItemObject = null;

        // Loop through the inventory item database
        foreach (var i in m_oInventoryManger.GetItemDatabase())
        {
            // Get the gameobject to be spawned on the server
            if (i.Key == nItemID)
                gItemObject = i.Value.m_gSceneObject;
        }

        // Spawn object on the server intended to be used as the equipped item
        GameObject oEquippingItem = Instantiate(gItemObject);
        oEquippingItem.transform.position = m_gInHandSpawn.transform.position;
        oEquippingItem.GetComponent<NetworkObject>().SpawnWithOwnership(ulClientID);

        // Get the network object ID for the spawned object to be equipped
        mn_ulEquippedItemNetworkID.Value = oEquippingItem.GetComponent<NetworkObject>().NetworkObjectId;

        // Run client rpc to equip item on each of the clients
        EquipItemClientRpc(mn_ulEquippedItemNetworkID.Value);

        // Set the status of equipped item to true
        mn_bIsItemEquipped.Value = true;
    }

    //--------------------------------------------------------------------------------------
    // EquipItemClientRpc: Client function for equipping items for each client on the server.
    //
    // Param:
    //      ulObjectNetworkID: ulong value for ID of the spawned network object.
    //--------------------------------------------------------------------------------------
    [ClientRpc]
    private void EquipItemClientRpc(ulong ulObjectNetworkID)
    {
        // Send debug message to console
        if (m_bDebugMode)
            Debug.Log("ClientRpc for EquipItem Called!");

        // Destroy the previous equipped item
        Destroy(m_gEquippedItem);

        // Get the equipped item from the list of spawned network objects
        m_gEquippedItem = NetworkSpawnManager.SpawnedObjects[ulObjectNetworkID].gameObject;

        // Set up equipped item for usage, setting parent, pos, rotation and player object
        m_gEquippedItem.GetComponent<Equipable>().SetPlayerScript(transform.parent.GetComponent<Player>());
        m_gEquippedItem.transform.parent = transform;
        m_gEquippedItem.transform.position = m_gInHandSpawn.transform.position;
        m_gEquippedItem.transform.rotation = transform.rotation;

        // Enabled the sprite renderer once everything is ready
        m_gEquippedItem.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    //--------------------------------------------------------------------------------------
    // UnequipItemServerRpc: Server function for initating an unequip event for all the clients
    //--------------------------------------------------------------------------------------
    [ServerRpc]
    private void UnequipItemServerRpc()
    {
        // Send debug message to console
        if (m_bDebugMode)
            Debug.Log("ServerRpc for UnequipItem Called!");

        // Run client rpc for unequipping on each client
        UnequipItemClientRpc();

        // Set the status of equipped item to false
        mn_bIsItemEquipped.Value = false;
    }

    //--------------------------------------------------------------------------------------
    // UnequipItemClientRpc: Client function for unequipping items for each client on the server.
    //--------------------------------------------------------------------------------------
    [ClientRpc]
    private void UnequipItemClientRpc()
    {
        // Send debug message to console
        if (m_bDebugMode)
            Debug.Log("ClientRpc for UnequipItem Called!");

        // if the equipped item isnt null set it to inactive
        if (m_gEquippedItem != null)
            m_gEquippedItem.SetActive(false);
    }
}