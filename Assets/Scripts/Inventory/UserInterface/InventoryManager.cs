//--------------------------------------------------------------------------------------
// Purpose: The main class for managing the inventory of player and other objects.
//
// Description: This script will handle the opening of an inventory container, making sure
// only one can be open at once, ensure data can pass between different containers, are 
// items selected in an inventory, and other general management.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//--------------------------------------------------------------------------------------
// InventoryManager Object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class InventoryManager : MonoBehaviour
{
    // public static inventory manager singleton
    public static InventoryManager m_oInstance;

    // INVENTORY //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Inventory Settings:")]

    // public gameobject for the slot prefab
    [LabelOverride("Slot Prefab")] [Tooltip("The prefab used to generate slots in an inventory container.")]
    public GameObject m_gSlotPrefab;

    // public list of container getters 
    public List<ContainerGetter> m_aoContainers = new List<ContainerGetter>();

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private container for the currently open container.
    private Container m_oCurrentOpenContainer;

    // private item stack for what stack is currently selected.
    private ItemStack m_oCurrentSelectedStack = ItemStack.m_oEmpty;

    // private selected stack.
    private SelectedStack m_oSelectedStack;

    // private tooltip for the seting tooltip
    private Tooltip m_oToolTip;

    // private Dictionary of int keys and Item values, for a database of all Items in the project
    private Dictionary<int, Item> m_dictItemDatabase = new Dictionary<int, Item>();

    // private bool for if an inventory is open or not.
    private bool m_bIsInventoryOpen = false;
    //--------------------------------------------------------------------------------------

    // STANDARD GETTERS / SETTERS //
    //--------------------------------------------------------------------------------------
    // Getter of type Dictionary (int as key, Item as value) for Item Database value
    public Dictionary<int, Item> GetItemDatabase() { return m_dictItemDatabase; }

    // Getter of type Container for the currently open container value
    public Container GetCurrentlyOpenContainer() { return m_oCurrentOpenContainer; }

    // Getter of type bool for the Is Inventory Open value
    public bool IsInventoryOpen() { return m_bIsInventoryOpen; }

    // Getter of type ItemStack for the current Selected Stack value
    public ItemStack GetSelectedStack() { return m_oCurrentSelectedStack; }

    // Setter for resetting the current inventory status
    public void ResetInventoryStatus() { m_bIsInventoryOpen = false; }

    // Setter for type string for setting the tooltip
    public void ActivateToolTip(string strTitle) { m_oToolTip.SetTooltip(strTitle); }
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Initialization.
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        // Load all item objects in the items resource folder
        var vItems = Resources.LoadAll("Items", typeof(Item)).Cast<Item>().ToArray();

        // Loop through all the item objects
        for (int i = 0; i < vItems.Length; i++)
        {
            // Add the item objects to the item database
            m_dictItemDatabase.Add(i, vItems[i]);
        }
    }

    //--------------------------------------------------------------------------------------
    // Initialization.
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // set instance
        m_oInstance = this;

        // get the selected stack component
        m_oSelectedStack = GetComponentInChildren<SelectedStack>();

        // get the tooltip component
        m_oToolTip = GetComponentInChildren<Tooltip>();
    }

    //--------------------------------------------------------------------------------------
    // GetContainerPrefab: Get the container prefab of a specific title.
    //
    // Param:
    //      strTitle: A string, the title of the prefab to get.
    //
    // Return:
    //      GameObject: Returns a gameobject of the prefab.
    //--------------------------------------------------------------------------------------
    public GameObject GetContainerPrefab(string strTitle)
    {
        // for each container in containers
        foreach (ContainerGetter i in m_aoContainers)
        {
            // Do the container titles match
            if (i.m_strTitle == strTitle)
            {
                // return prefab
                return i.m_gPrefab;
            }
        }

        // return null
        return null;
    }

    //--------------------------------------------------------------------------------------
    // OpenContainer: Open an inventory container.
    //
    // Param:
    //      nContainer: Takes in container type for which container to open.
    //--------------------------------------------------------------------------------------
    public void OpenContainer(Container oContainer)
    {
        // if the current open container is not null
        if (m_oCurrentOpenContainer != null)
        {
            // close the current open container
            m_oCurrentOpenContainer.Close();
        }

        // Set the current open container to passed in container
        m_oCurrentOpenContainer = oContainer;

        // set the inventory opened to true
        m_bIsInventoryOpen = true;

        // set the cursor back to default
        CustomCursor.m_oInstance.SetDefaultCursor();
    }

    //--------------------------------------------------------------------------------------
    // CloseContainer: Close the currently open container.
    //--------------------------------------------------------------------------------------
    public void CloseContainer()
    {
        // if the current open container is not null
        if (m_oCurrentOpenContainer != null)
        {
            // close the current open container
            m_oCurrentOpenContainer.Close();
        }

        // if the current selected stack is not empty
        if (!m_oCurrentSelectedStack.IsStackEmpty())
        {
            // Get the origin inventory of the selected stack
            Inventory oOrigin = m_oSelectedStack.GetOriginInventory();

            // Add the current selected stack back to the origin inventory
            oOrigin.AddItemAtPosition(m_oCurrentSelectedStack, m_oSelectedStack.GetOriginSlot());

            // set the selected stack back to emtpy.
            SetSelectedStack(ItemStack.m_oEmpty, null, 0);
        }

        // turn off the tool tip
        ActivateToolTip(string.Empty);

        // set the inventory opened to false
        m_bIsInventoryOpen = false;

        // set the cursor back to previous
        CustomCursor.m_oInstance.SetPreviousCursor();
    }

    //--------------------------------------------------------------------------------------
    // SetSelectedStack: Set the currently selected item stack.
    //
    // Param:
    //      oStack: Takes in type ItemStack to set currently selected.
    //      oOrigin: Takes in type Inventory, to set the origin inventory of the selected stack.
    //      nSlotOrigin: Takes in int, to set the origin slot of the selected stack.
    //--------------------------------------------------------------------------------------
    public void SetSelectedStack(ItemStack oStack, Inventory oOrigin, int nSlotOrigin)
    {
        // set the currently selected stack
        m_oSelectedStack.SetSelectedStack(m_oCurrentSelectedStack = oStack);

        // check if the origin is not null, if not then set origin to origin of the selected stack.
        if (oOrigin != null)
        {
            m_oSelectedStack.SetOriginInventory(oOrigin);
            m_oSelectedStack.SetOriginSlot(nSlotOrigin);
        }
    }
}

//--------------------------------------------------------------------------------------
// ContainerGetter Object.
//--------------------------------------------------------------------------------------
[System.Serializable]
public class ContainerGetter
{
    // CONTAINER SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Container Settings:")]

    // public string for the container title.
    [LabelOverride("Container Title")] [Tooltip("The title of the container to use this prefab.")]
    public string m_strTitle;

    // public gameobject for prefab
    [LabelOverride("Container Prefab")] [Tooltip("The prefab to be used by a container class.")]
    public GameObject m_gPrefab;
    //--------------------------------------------------------------------------------------
}