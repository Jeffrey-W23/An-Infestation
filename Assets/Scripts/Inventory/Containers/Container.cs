//--------------------------------------------------------------------------------------
// Purpose: Object for containing the inventory of an objec.
//
// Description: This class will act as a container for differnt inventorys in the project.
// so the player will have a container, chest, hotbars or enemies. Like a real container
// it can be opened and closed and has access to the players contaienr for easily moving 
// of items from container to container.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Container Object.
//--------------------------------------------------------------------------------------
public class Container
{
    // protected int for the amount of slots in this container/inventory.
    protected int m_nSlots;

    // protected gameobject for the prefab of this container.
    protected GameObject m_gPrefab;

    // protected inventory for the inventory for this container.
    protected Inventory m_oInventory;

    // protected inventory for the player inventory
    protected Inventory m_oPlayerInventory;

    // protected list of item slot for the slots of this container.
    protected List<ItemSlot> m_agSlots = new List<ItemSlot>();

    //--------------------------------------------------------------------------------------
    // Default Constructor.
    //
    // Param:
    //      oInventory: The inventory used for this container.
    //      oPlayerInventory: The inventory used for the player container.
    //      nSlots: the amount of slots in the container/inventory.
    //--------------------------------------------------------------------------------------
    public Container(Inventory oInventory, Inventory oPlayerInventory, int nSlots)
    {
        // set default values
        m_oInventory = oInventory;
        m_oPlayerInventory = oPlayerInventory;
        m_nSlots = nSlots;

        // Open the container
        Open();
    }

    //--------------------------------------------------------------------------------------
    // AddSlot: Add a new slot to an inventory container.
    //
    // Param:
    //      oInventory: The inventory used for this container.
    //      nId: The id of the item stack to add to this slot.
    //      tParent: The parent transform for this slot.
    //--------------------------------------------------------------------------------------
    public void AddSlot(Inventory oInventory, int nId, Transform tParent)
    {
        // get the slot component
        GameObject gInstance = Object.Instantiate(InventoryManager.m_gInstance.m_gSlotPrefab);
        ItemSlot gSlot = gInstance.GetComponent<ItemSlot>();

        // set the postion of the slot to the parent
        gInstance.transform.SetParent(tParent);

        // add the slot to the slots array
        m_agSlots.Add(gSlot);

        // set the slot to the slot object
        gSlot.SetSlot(oInventory, nId, this);
    }

    //--------------------------------------------------------------------------------------
    // UpdateSlots: Update all the slots in an Inventory.
    //--------------------------------------------------------------------------------------
    public void UpdateSlots()
    {
        // for each slot in slots array
        foreach (ItemSlot i in m_agSlots)
        {
            // Update the slot
            i.UpdateSlot();
        }
    }

    //--------------------------------------------------------------------------------------
    // Open: What to do when the container is opened.
    //--------------------------------------------------------------------------------------
    public void Open()
    {
        // instantiate the container prefab at the inventory manager
        m_gPrefab = Object.Instantiate(GetPrefab(), InventoryManager.m_gInstance.transform);

        // set the prefab to draw at the back
        m_gPrefab.transform.SetAsFirstSibling();
    }

    //--------------------------------------------------------------------------------------
    // Close: What to do when the container is closed.
    //--------------------------------------------------------------------------------------
    public void Close()
    {
        // destroy prefab container object
        Object.Destroy(m_gPrefab);
    }

    //--------------------------------------------------------------------------------------
    // GetPrefab: Get the container prefab for this container. 
    //
    // Return:
    //      GameObject: Returns the prefab of this container.
    //--------------------------------------------------------------------------------------
    public virtual GameObject GetPrefab()
    {
        // return null, will be handled in children
        return null;
    }

    //--------------------------------------------------------------------------------------
    // GetSpawnedContainer: Get the spawned prefab for this container. 
    //
    // Return:
    //      GameObject: Returns the spawned prefab of this container.
    //--------------------------------------------------------------------------------------
    public GameObject GetSpawnedContainer()
    {
        // return the prefab
        return m_gPrefab;
    }

    //--------------------------------------------------------------------------------------
    // GetContainerSize: Get the size of the container
    //
    // Return:
    //      int: returns the size of the container.
    //--------------------------------------------------------------------------------------
    public int GetContainerSize()
    {
        // return the container inventory
        return m_nSlots;
    }

    //--------------------------------------------------------------------------------------
    // GetInventory: Get the inventory of this container.
    //
    // Return:
    //      Inventory: returns the inventory of this container.
    //--------------------------------------------------------------------------------------
    public Inventory GetInventory()
    {
        // return the container inventory
        return m_oInventory;
    }

    //--------------------------------------------------------------------------------------
    // GetPlayerInventory: Get the player inventory.
    //
    // Return:
    //      Inventory: returns the player inventory.
    //--------------------------------------------------------------------------------------
    public Inventory GetPlayerInventory()
    {
        // return the player inventory
        return m_oPlayerInventory;
    }
}
