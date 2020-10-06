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
using UnityEngine;

//--------------------------------------------------------------------------------------
// InventoryManager Object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class InventoryManager : MonoBehaviour
{
    // public static inventory manager singleton
    public static InventoryManager m_gInstance;

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
    private SelectedStack m_gSelectedStack;

    // private tooltip for the seting tooltip
    private Tooltip m_gToolTip;

    // private bool for if an inventory is open or not.
    private bool m_bIsInventoryOpen = false;
    //--------------------------------------------------------------------------------------

    // REMOVE // TEMP // REMOVE // POSSIBLTY //
    // private player object for getting the player.
    [HideInInspector]
    public Player m_gPlayer; // TEMP // NEEDS TO BE PRIVATE // TEMP
    // REMOVE // TEMP // REMOVE // POSSIBLTY //

    //--------------------------------------------------------------------------------------
    // Initialization.
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // set instance
        m_gInstance = this;

        // get the selected stack component
        m_gSelectedStack = GetComponentInChildren<SelectedStack>();

        // get the tooltip component
        m_gToolTip = GetComponentInChildren<Tooltip>();

        // get the player object
        m_gPlayer = FindObjectOfType<Player>();
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // if the inventory is currently opened
        if (m_bIsInventoryOpen)
        {
            // if the escape key is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Close the inventory container
                CloseContainer();

                // allow the player to move again because an inventory is no longer open
                m_gPlayer.SetFreezePlayer(false);

                // turn off the tool tip
                ActivateToolTip(string.Empty);
            }
        }
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
    public void OpenContainer(Container nContainer)
    {
        // if the current open container is not null
        if (m_oCurrentOpenContainer != null)
        {
            // close the current open container
            m_oCurrentOpenContainer.Close();
        }

        // Set the current open container to passed in container
        m_oCurrentOpenContainer = nContainer;

        // set the inventory opened to true
        m_bIsInventoryOpen = true;

        // stop the player from moving when the inventory is open
        m_gPlayer.SetFreezePlayer(true);

        // set the cursor back to default
        CustomCursor.m_gInstance.SetDefaultCursor();
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

        // set the inventory opened to false
        m_bIsInventoryOpen = false;

        // freeze the player
        m_gPlayer.SetFreezePlayer(false);
    }

    //--------------------------------------------------------------------------------------
    // GetCurrentlyOpenContainer: Get the currently opened container.
    //
    // Return:
    //      Container: Return a container type.
    //--------------------------------------------------------------------------------------
    public Container GetCurrentlyOpenContainer()
    {
        // return the current open container
        return m_oCurrentOpenContainer;
    }

    //--------------------------------------------------------------------------------------
    // IsInventoryOpen: Check if a container is currently opened.
    //
    // Return:
    //      bool: return true or false.
    //--------------------------------------------------------------------------------------
    public bool IsInventoryOpen()
    {
        // return if the inventory is open
        return m_bIsInventoryOpen;
    }

    //--------------------------------------------------------------------------------------
    // ResetInventoryStatus: Reset the current status of the inventory mananger.
    //--------------------------------------------------------------------------------------
    public void ResetInventoryStatus()
    {
        // reset the inventory open status back to false
        m_bIsInventoryOpen = false;

        // unfreeze the player.
        m_gPlayer.SetFreezePlayer(false);
    }

    //--------------------------------------------------------------------------------------
    // GetSelectedStack: Get the currently selected item stack object.
    //
    // Return:
    //      ItemStack: returns the item stack currently selected.
    //--------------------------------------------------------------------------------------
    public ItemStack GetSelectedStack()
    {
        // return the currently selected stack
        return m_oCurrentSelectedStack;
    }

    //--------------------------------------------------------------------------------------
    // SetSelectedStack: Set the currently selected item stack.
    //
    // Param:
    //      oStack: Takes in type ItemStack to set currently selected.
    //--------------------------------------------------------------------------------------
    public void SetSelectedStack(ItemStack oStack)
    {
        // set the currently selected stack
        m_gSelectedStack.SetSelectedStack(m_oCurrentSelectedStack = oStack);
    }

    //--------------------------------------------------------------------------------------
    // ActivateToolTip: Activate the tooltip object for an item stack.
    //
    // Param:
    //      strTitle: Takes in a string for which tooltip to display.
    //--------------------------------------------------------------------------------------
    public void ActivateToolTip(string strTitle)
    {
        // set the tooltip
        m_gToolTip.SetTooltip(strTitle);
    }

    //--------------------------------------------------------------------------------------
    // GetPlayerInventory: Get the player inventory.
    //
    // Return:
    //      Inventory: returns an inventory object for the player inventory.
    //--------------------------------------------------------------------------------------
    public Inventory GetPlayerInventory()
    {
        return m_gPlayer.GetInventory();
    }

    //--------------------------------------------------------------------------------------
    // GetPlayerWeaponsInventory: Get the player weapons inventory.
    //
    // Return:
    //      Inventory: returns an inventory object for the player inventory.
    //--------------------------------------------------------------------------------------
    public Inventory GetPlayerWeaponsInventory()
    {
        return m_gPlayer.GetWeapons();
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