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
    // PROTECTED VALUES //
    //--------------------------------------------------------------------------------------
    // protected int for the amount of slots in this container/inventory.
    protected int m_nSlots;

    // protected gameobject for the prefab of this container.
    protected GameObject m_gPrefab;

    // protected inventory for the inventory for this container.
    protected Inventory m_oInventory;

    // protected inventory for the player inventory
    protected Inventory m_oPlayerInventory;

    // protected list of item slot for the slots of this container.
    protected List<ItemSlot> m_aoSlots = new List<ItemSlot>();
    //--------------------------------------------------------------------------------------

    // STANDARD GETTERS / SETTERS //
    //--------------------------------------------------------------------------------------
    // Getter of type GameObject for getting the prefab of the container, null as handled on children
    public virtual GameObject GetPrefab() { return null; }

    // Getter of type GameObject for getting the Spawned Container
    public GameObject GetSpawnedContainer() { return m_gPrefab; }

    // Getter of type int for getting the container size
    public int GetContainerSize() { return m_nSlots; }

    // Getter of type inventory for getting the inventory of this container
    public Inventory GetInventory() { return m_oInventory; }

    // Getter of type inventory for getting the inventory of the player
    public Inventory GetPlayerInventory() { return m_oPlayerInventory; }
    //--------------------------------------------------------------------------------------

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
        GameObject gInstance = Object.Instantiate(InventoryManager.m_oInstance.m_gSlotPrefab);
        ItemSlot oSlot = gInstance.GetComponent<ItemSlot>();

        // set the postion of the slot to the parent
        gInstance.transform.SetParent(tParent);

        // add the slot to the slots array
        m_aoSlots.Add(oSlot);

        // set the slot to the slot object
        oSlot.SetSlot(oInventory, nId, this);
    }

    //--------------------------------------------------------------------------------------
    // UpdateSlots: Update all the slots in an Inventory.
    //--------------------------------------------------------------------------------------
    public void UpdateSlots()
    {
        // for each slot in slots array
        foreach (ItemSlot i in m_aoSlots)
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
        m_gPrefab = Object.Instantiate(GetPrefab(), InventoryManager.m_oInstance.transform);

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
}