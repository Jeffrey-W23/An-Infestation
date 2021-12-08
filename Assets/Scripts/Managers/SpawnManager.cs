//--------------------------------------------------------------------------------------
// Purpose: Manage different objects that spawn during the game.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// SpawnManager object. Inheriting from NetworkBehaviour.
//--------------------------------------------------------------------------------------
public class SpawnManager : NetworkBehaviour
{
    // BULLET SETUP //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Bullet Pool Setup:")]

    // Public item for bullet object.
    [LabelOverride("Bullet Item")] [Tooltip("The bullet item that this gun will fire.")]
    public Item m_oBulletBlueprint;

    // public int for pool size.
    [LabelOverride("Pool Size")] [Tooltip("How many bullets allowed on screen at one time.")]
    public int m_nBulletPoolSize;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // NETWORK OBJECT SPAWN SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Objects to Spawn on Server Start:")]

    // public array of network objects for objects to spawn on the server
    [LabelOverride("Objects to Spawn")] [Tooltip("A list of objects to spawn on server start.")]
    public List<NetworkObject> m_anoServerStartObjects;

    // public array of transforms for the locations to spawn the items on server
    [LabelOverride("Spawn Locations")] [Tooltip("A list of transforms for the spawn locations of server start objects.")]
    public List<Transform> m_atServerStartSpawnLocations;
    //--------------------------------------------------------------------------------------

    // PUBLIC HIDDEN //
    //--------------------------------------------------------------------------------------
    // new singleton for getting the spawn manager
    [HideInInspector]
    public static SpawnManager m_oInstance;
    //--------------------------------------------------------------------------------------

    // PROTECTED VALUES //
    //--------------------------------------------------------------------------------------
    // A private Array of GameObjects for bullets.
    private GameObject[] m_agBulletPool;
    //--------------------------------------------------------------------------------------

    // GETTERS / SETTERS //
    //--------------------------------------------------------------------------------------
    // Getter of type GameObject for getting a bullet from the bullet object pool
    public GameObject GetBulletFromPool() { return GetObjectFromPool(m_agBulletPool); }
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // set instance
        m_oInstance = this;

        // Initialize bullet object pool
        m_agBulletPool = new GameObject[m_nBulletPoolSize];

        // Create a new object pool of bullets
        CreateObjectPool(m_nBulletPoolSize, m_oBulletBlueprint.m_gSceneObject, m_agBulletPool);
    }

    //--------------------------------------------------------------------------------------
    // Initialization
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        // Subscribe to network manager events for server starting
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
    }

    //--------------------------------------------------------------------------------------
    // OnDestroy: Function that will call on this gameObjects destruction.
    //--------------------------------------------------------------------------------------
    private void OnDestroy()
    {
        // if the network manager is valid
        if (NetworkManager.Singleton == null)
            return;

        // Unsubscribe to network manager events
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
    }

    //--------------------------------------------------------------------------------------
    // HandleServerStarted: Event triggered when the server is started
    //--------------------------------------------------------------------------------------
    private void HandleServerStarted()
    {
        // if player is a host client 
        if (NetworkManager.Singleton.IsHost)
        {
            // Spawn items on the server
            SpawnItemsOnStartUpServerRpc();
        }
    }

    //--------------------------------------------------------------------------------------
    // SpawnItemsOnStartUpServerRpc: A server function for spawning items for all the clients.
    //--------------------------------------------------------------------------------------
    [ServerRpc]
    private void SpawnItemsOnStartUpServerRpc()
    {
        // Loop through all the objects to spawn
        for (int i = 0; i < m_anoServerStartObjects.Count; i++)
        {
            // Instantiate network objects in the scene
            NetworkObject noSpawnObject = Instantiate(m_anoServerStartObjects[i], m_atServerStartSpawnLocations[i].position, Quaternion.identity);

            // Check if the spawning item is a pickup item, if so set the item count
            if (noSpawnObject.GetComponent<ItemPickup>() != null)
                noSpawnObject.GetComponent<ItemPickup>().SetItemCount(noSpawnObject.GetComponent<ItemPickup>().m_nItemCount);

            // Spawn instantiated object on the server
            noSpawnObject.Spawn();
        }
    }

    //--------------------------------------------------------------------------------------
    // SpawnItemServerRpc: Server function for spawning a specific item on the server.
    //
    // Param:
    //      nItemID: int value for the item ID of the item to be spawned.
    //      nItemCount: int value for the stack count of the item to be spawned.
    //      ulClientID: ulong value for the player client ID
    //--------------------------------------------------------------------------------------
    [ServerRpc(RequireOwnership = false)]
    public void SpawnItemServerRpc(int nItemID, int nItemCount, ulong ulClientID)
    {
        // Get the clients network object, return if not valid.
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(ulClientID, out NetworkClient ncClient))
            return;

        // Get the clients player object, return if not valid
        if (!ncClient.PlayerObject.TryGetComponent<Player>(out Player oPlayer))
            return;

        // New null gameobject for the item to be equipped
        GameObject gItemObject = null;

        // Loop through the inventory item database
        foreach (var i in InventoryManager.m_oInstance.GetItemDatabase())
        {
            // Get the gameobject to be spawned on the server
            if (i.Key == nItemID)
                gItemObject = i.Value.m_gPickUpObject;
        }

        // Instantiate object in the scene
        GameObject gSpawnObject = Instantiate(gItemObject);

        // Set the position to the front of the player
        Vector3 v3SpawnLocation = (oPlayer.transform.right * 1.6f) + oPlayer.transform.position;
        gSpawnObject.transform.position = new Vector3(v3SpawnLocation.x, v3SpawnLocation.y, 0.1199999f);

        // Set the count of the objects item stack
        gSpawnObject.GetComponent<ItemPickup>().SetItemCount(nItemCount);
        
        // Spawn object on the server
        gSpawnObject.GetComponent<NetworkObject>().Spawn();
    }

    //--------------------------------------------------------------------------------------
    // CreateObjectPool: Create a new object pool, based on the passed in data.
    //
    // Params:
    //      nPoolSize: The size of the object pool.
    //      gObject: The GameObject to add to the pool.
    //      agObjectPool: The Array to assign pool items.
    //--------------------------------------------------------------------------------------
    private void CreateObjectPool(int nPoolSize, GameObject gObject, GameObject[] agObjectPool)
    {
        // loop through the object pool
        for (int i = 0; i < nPoolSize; ++i)
        {
            // Instaniate a new object in the scene
            GameObject gNewObject = Instantiate(gObject);

            // Set new instaniated object to inactive
            gNewObject.SetActive(false);

            // Add the new object to the pool
            agObjectPool[i] = gNewObject;
        }
    }

    //--------------------------------------------------------------------------------------
    // GetObjectFromPool: Get an Object from a selected object pool. 
    //
    // Params:
    //      agObjectPool: The Object Pool to pull an object from.
    //
    // Return:
    //      GameObject: An inactive object from the pool, ready for usage.
    //--------------------------------------------------------------------------------------
    public GameObject GetObjectFromPool(GameObject[] agObjectPool)
    {
        // Loop through the object pool
        for (int i = 0; i < agObjectPool.Length; ++i)
        {
            // Check if the object in pool is inactive
            if (!agObjectPool[i].activeInHierarchy)
            {
                // Return the object in pool
                return agObjectPool[i];
            }
        }

        // Else return null
        return null;
    }
}