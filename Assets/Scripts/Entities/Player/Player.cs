﻿//--------------------------------------------------------------------------------------
// Purpose: The main logic for the Player object.
//
// Description: This script will handled most of the typical work that a player has to
// do like movement, interaction, opening menus, etc.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

//--------------------------------------------------------------------------------------
// Player object. Inheriting from NetworkBehaviour.
//--------------------------------------------------------------------------------------
public class Player : NetworkBehaviour
{
    // MOVEMENT //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Movement Settings:")]

    // public float value for the walking speed.
    [LabelOverride("Walking Speed")] [Tooltip("The speed of which the player will walk in float value.")]
    public float m_fWalkSpeed = 5.0f;

    // public float value for the walking speed.
    [LabelOverride("Running Speed")] [Tooltip("The speed of which the player will run in float value.")]
    public float m_fRunSpeed = 7.0f;

    // public float value for max exhaust level.
    [LabelOverride("Running Exhaust")] [Tooltip("The max level of exhaustion the player can handle before running is false.")]
    public float m_fRunExhaust = 3.0f;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // MOVEMENT //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("FOV Settings:")]

    // public gameobject for the player vision
    [LabelOverride("Player Vision")] [Tooltip("The gameobject for the players main field of view object.")]
    public GameObject m_gPlayerVision;

    // public gameobject for the enemy renderer
    [LabelOverride("Enemy Renderer")] [Tooltip("The gameobject for the players enemy renderer field of view object.")]
    public GameObject m_gEnemyRenderer;

    // public gameobject for the inner vision renderer
    [LabelOverride("Inner Vision Renderer")] [Tooltip("The gameobject for the players inner field of view object.")]
    public GameObject m_gInnerVisionRenderer;

    // public gameobject for the inner enemy renderer
    [LabelOverride("Inner Enemy Renderer")] [Tooltip("The gameobject for the players inner enemy renderer field of view object.")]
    public GameObject m_gInnerEnemyRenderer;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // INVENTORY //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Inventory Settings:")]

    // public int for the inventory size
    [LabelOverride("Inventory Size")] [Tooltip("The size of the Inventory for the player object.")]
    public int m_nInventorySize = 6;

    // public int for the weapon inventory size
    [LabelOverride("Weapons Solts")] [Tooltip("The amount of solts available for weapon pickups.")]
    public int m_nWeaponSlots = 3;

    // public list of item type enums for incompatible inventory items
    [LabelOverride("Incompatible Items")] [Tooltip("Items that are incompatible with the player inventory.")]
    public List<EItemType> m_aeIncompatibleInventoryItems;

    // public list of item type enums for incompatible weapons
    [LabelOverride("Incompatible Weapons")] [Tooltip("Weapons incompatible with the weapons slot.")]
    public List<EItemType> m_aeIncompatibleWeapons;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // DEBUG //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Debug:")]

    // public bool for turning the debug info off and on.
    [LabelOverride("Display Debug Info?")] [Tooltip("Turns off and on debug information in the unity console.")]
    public bool m_bDebugMode = true;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private rigidbody.
    private Rigidbody2D m_rbRigidBody;

    // private gameobject for the players arm object
    private GameObject m_gArm;

    // private sprite renderer for the players body
    private SpriteRenderer m_srBody;

    // private camera for the players camera object
    private Camera m_cPlayerCamera;

    // private FieldOfView object for player vison FOV
    private FieldOfView m_oPlayerVisionScript;

    // private FieldOfView object for enemy renderer FOV
    private FieldOfView m_oEnemyRendererScript;

    // private inventory for the player object
    private Inventory m_oInventory;

    // private inventory for the players current weapons
    private Inventory m_oWeapons;

    // private inventory manager for the inventory manager instance
    private InventoryManager m_oInventoryManger;

    // private array of initial keycodes for selecting weapon index.
    private KeyCode[] m_akInitWeaponSelectorControls = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

    // private quaternion for setting the cameras inital rotation
    private Quaternion m_qInitRotation;

    // private array of needed keycodes for selecting weapon index. 
    private KeyCode[] m_akWeaponSelectorControls;

    // private float for the current speed of the player.
    private float m_fCurrentSpeed;

    // private flkoat for the current exhaust level of the player.
    private float m_fRunCurrentExhaust = 0.0f;

    // private int for the current postion of the weapon selection.
    private int m_nWeaponSelectorPos = 0;

    // private bool for if ther player can run or not
    private bool m_bExhausted = false;

    // private bool for freezing the player
    private bool m_bFreezePlayer = false;
    //--------------------------------------------------------------------------------------

    // PRIVATE NETWORKED VARS //
    //--------------------------------------------------------------------------------------
    // new private network bool for keeping track of current state of the FOV
    private NetworkVariableBool mn_bFOVToggle = new NetworkVariableBool(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, true);

    // new private network variable for body color, default will be white
    private NetworkVariableColor mn_cBodyColor = new NetworkVariableColor(Color.white);
    //--------------------------------------------------------------------------------------

    // DELEGATES //
    //--------------------------------------------------------------------------------------
    // Create a new Delegate for handling the interaction functions.
    public delegate void InteractionEventHandler(Player oPlayer);

    // Create an event for the delegate for extra protection. 
    public InteractionEventHandler InteractionCallback;
    //--------------------------------------------------------------------------------------

    // STANDARD GETTERS / SETTERS //
    //--------------------------------------------------------------------------------------
    // Getter of type FieldOfView for Player Vision Script
    public FieldOfView GetPlayerVisionScript() { return m_oPlayerVisionScript; }

    // Getter of type FieldOfView for Enemy Renderer Script
    public FieldOfView GetEnemyRendererScript() { return m_oEnemyRendererScript; }

    // Getter of type Inventory for Player Inventory
    public Inventory GetInventory() { return m_oInventory; }

    // Getter of type Inventory for Player Weapons
    public Inventory GetWeapons() { return m_oWeapons; }

    // Getter for type Bool for the Freeze Player value.
    public bool GetFreezePlayer() { return m_bFreezePlayer; }
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Initialization
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // Get the Rigidbody.
        m_rbRigidBody = GetComponent<Rigidbody2D>();

        // get the player arm object.
        m_gArm = transform.Find("Arm").gameObject;

        // Get the sprite renderer for the players body
        m_srBody = transform.Find("Body").gameObject.GetComponent<SpriteRenderer>();

        // Get the camera object of the player
        m_cPlayerCamera = transform.Find("PlayerCamera").GetComponent<Camera>();

        // set the current speed of the player to walk
        m_fCurrentSpeed = m_fWalkSpeed;

        // set the inventory of the player
        m_oInventory = new Inventory(m_nInventorySize, m_aeIncompatibleInventoryItems);

        // set the weapons inventory of the player
        m_oWeapons = new Inventory(m_nWeaponSlots, m_aeIncompatibleWeapons);
    }

    //--------------------------------------------------------------------------------------
    // Initialization
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        // get the inventory manager instance
        m_oInventoryManger = InventoryManager.m_oInstance;

        // Ensure inventory isnt open when game starts
        m_oInventoryManger.ResetInventoryStatus();

        // set the count of the keyboard controls to the size of the weapon inventory
        m_akWeaponSelectorControls = new KeyCode[m_oWeapons.GetArray().Count];

        // loop through the keyboard controls
        for (int i = 0; i < m_akWeaponSelectorControls.Length; i++)
        {
            // Set the keyboard controls avalible for weapon selection.
            m_akWeaponSelectorControls[i] = m_akInitWeaponSelectorControls[i];
        }

        // get fov components
        m_oPlayerVisionScript = m_gPlayerVision.GetComponent<FieldOfView>();
        m_oEnemyRendererScript = m_gEnemyRenderer.GetComponent<FieldOfView>();

        // Set the main camera for the FOV renderers
        m_oPlayerVisionScript.SetMainCamera(m_cPlayerCamera);
        m_oEnemyRendererScript.SetMainCamera(m_cPlayerCamera);

        // If this player is a connecting client
        if (!IsLocalPlayer)
        {
            // Disable Fog Of War for all connected clients
            m_oPlayerVisionScript.DisableFogOfWar(true);
            m_gInnerVisionRenderer.GetComponent<FieldOfView>().DisableFogOfWar(true);

            // turn off any camera object that is not the local player
            m_cPlayerCamera.gameObject.SetActive(false);
        }

        // Set the initial rotation of this camera
        m_qInitRotation = m_cPlayerCamera.transform.rotation;
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // Check if current player object is the local player
        if (IsLocalPlayer)
        {
            // if the player is not frozen
            if (!m_bFreezePlayer)
            {
                // run the interaction function
                Interaction();

                // Update the in hand object of player
                UpdateInHand();
            }

            // Open and close the inventory system
            OpenCloseInventory();
        }

        // if the player is not frozen toggle the fov on and off
        if (!m_bFreezePlayer)
            ToggleFOV();
    }

    //--------------------------------------------------------------------------------------
    // FixedUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void FixedUpdate()
    {
        // Check if current player object is the local player
        if (IsLocalPlayer)
        {
            // is player allowed to move
            if (!m_bFreezePlayer)
            {
                // rotate player based on mouse postion.
                Rotate();

                // run the movement function to move player.
                Movement();
            }
        }

        // rotate fov based on mouse position.
        RotateFieldOfView();
    }

    //--------------------------------------------------------------------------------------
    // LateUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void LateUpdate()
    {
        // ensure the camera does not rotate with the parent object, if it is the local player
        if (IsLocalPlayer)
            m_cPlayerCamera.transform.rotation = m_qInitRotation;
    }

    //--------------------------------------------------------------------------------------
    // OnEnable: Function that will call when this gameObject is enabled.
    //--------------------------------------------------------------------------------------
    private void OnEnable()
    {
        // subscribe to value change event for body color
        mn_cBodyColor.OnValueChanged += OnBodyColorChange;
    }

    //--------------------------------------------------------------------------------------
    // OnDestroy: Function that will call on this gameObjects destruction.
    //--------------------------------------------------------------------------------------
    private void OnDestroy()
    {
        // Unsubscribe from body colors on change event
        mn_cBodyColor.OnValueChanged -= OnBodyColorChange;
    }

    //--------------------------------------------------------------------------------------
    // Movement: Move the player rigidbody, for both walking and sprinting.
    //--------------------------------------------------------------------------------------
    private void Movement()
    {
        // Get the Horizontal and Vertical axis.
        Vector3 v2MovementDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f).normalized;

        // Move the player
        m_rbRigidBody.MovePosition(transform.position + v2MovementDirection * m_fCurrentSpeed * Time.fixedDeltaTime);

        // if the players holds down left shift
        if (Input.GetKey(KeyCode.LeftShift) && !m_bExhausted)
        {
            // current player speed equals run speed
            m_fCurrentSpeed = m_fRunSpeed;

            // tick the current exhaust level up 
            m_fRunCurrentExhaust += Time.deltaTime;

            // if the current exhaust is above the max
            if (m_fRunCurrentExhaust > m_fRunExhaust)
            {
                // player is exhausted and speed is now walking.
                m_bExhausted = true;
                m_fCurrentSpeed = m_fWalkSpeed;
            }
        }

        // else if shift isnt down
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            // current speed is walking speed.
            m_fCurrentSpeed = m_fWalkSpeed;

            // tick the current exhaust level down 
            m_fRunCurrentExhaust -= Time.deltaTime;

            // if the current exhaust is below 0 keep at 0
            if (m_fRunCurrentExhaust < 0.0f)
                m_fRunCurrentExhaust = 0.0f;

            // if the current exhaust is below the max then exhausted false
            if (m_fRunCurrentExhaust < m_fRunExhaust)
                m_bExhausted = false;
        }
    }

    //--------------------------------------------------------------------------------------
    // Roatate: Rotate the player from the mouse movement.
    //--------------------------------------------------------------------------------------
    private void Rotate()
    {
        // Get mouse inside camera 
        Vector3 v3Position = transform.Find("PlayerCamera").GetComponent<Camera>().WorldToScreenPoint(transform.position);

        // Get the  mouse direction.
        Vector3 v3Direction = Input.mousePosition - v3Position;

        // Work out the angle.
        float fAngle = Mathf.Atan2(v3Direction.y, v3Direction.x) * Mathf.Rad2Deg;

        // Update the rotation.
        transform.rotation = Quaternion.AngleAxis(fAngle, Vector3.forward);
    }

    //--------------------------------------------------------------------------------------
    // RotateFieldOfView: Rotate the Field Of Vision  from the mouse movement.
    //--------------------------------------------------------------------------------------
    private void RotateFieldOfView()
    {
        // Send aim direction to player vision and enemy renderer
        m_oPlayerVisionScript.SetAimDirection(transform.right);
        m_oEnemyRendererScript.SetAimDirection(transform.right);

        // Set position of the vision cone and enemy renderer
        m_oPlayerVisionScript.SetOrigin(transform.position);
        m_oEnemyRendererScript.SetOrigin(transform.position);

        // Set the origin of the inner vision objects
        m_gInnerVisionRenderer.GetComponent<FieldOfView>().SetOrigin(transform.position);
        m_gInnerEnemyRenderer.GetComponent<FieldOfView>().SetOrigin(transform.position);
    }

    //--------------------------------------------------------------------------------------
    // SetFOVDefault: Set back the default settings for the field of view.
    //--------------------------------------------------------------------------------------
    public void SetFOVDefault()
    {
        // if the toggle state is true
        if (m_oPlayerVisionScript.GetToggleState())
        {
            // set the fov, distance and lerp smoothing of the player vision and enemy renederer
            m_oPlayerVisionScript.SetFOVDefault();
            m_oEnemyRendererScript.SetFOVDefault();
        }
    }

    //--------------------------------------------------------------------------------------
    // ToggleFOV: Switch the FOV on or off with keyboard press
    //--------------------------------------------------------------------------------------
    private void ToggleFOV()
    {
        // Check if current player object is the local player
        if (IsLocalPlayer)
        {
            // Toggle the FOV boolean used for turning FOV off and on
            if (Input.GetKeyDown(KeyCode.F) && m_oPlayerVisionScript.GetToggleState())
                mn_bFOVToggle.Value = false;
            else if (Input.GetKeyDown(KeyCode.F) && !m_oPlayerVisionScript.GetToggleState())
                mn_bFOVToggle.Value = true;
        }

        // If FOV toggle is true
        if (mn_bFOVToggle.Value)
        {
            // Set the vision and enemy renderer to true
            m_oPlayerVisionScript.ToggleFOV(true);
            m_oEnemyRendererScript.ToggleFOV(true);

            // If this isnt the local player
            if (!IsLocalPlayer)
            {
                // Turn the inner FOV for vison and enemy renderer back to true
                m_gInnerVisionRenderer.SetActive(true);
                m_gInnerEnemyRenderer.SetActive(true);
            }
        }

        // else if the toggle is false
        else
        {
            // Set the vision and enemy renderer to false
            m_oPlayerVisionScript.ToggleFOV(false);
            m_oEnemyRendererScript.ToggleFOV(false);

            // If this isnt the local player
            if (!IsLocalPlayer)
            {
                // Turn the inner FOV for vison and enemy renderer back to false
                m_gInnerVisionRenderer.SetActive(false);
                m_gInnerEnemyRenderer.SetActive(false);
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // OpenCloseInventory: Open/Close the Inventory container of the player object.
    //--------------------------------------------------------------------------------------
    private void OpenCloseInventory()
    {
        // if the i key is down and the inventory is closed
        if (Input.GetKeyDown(KeyCode.I) && !m_oInventoryManger.IsInventoryOpen())
        {
            // Open the player inventory
            m_oInventoryManger.OpenContainer(new PlayerContainer(m_oWeapons, m_oInventory, m_nInventorySize));

            // freeze the player
            SetFreezePlayer(true);
        }

        // if the i key is down and the inventory is open
        else if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape) && m_oInventoryManger.IsInventoryOpen())
        {
            // close the inventory container
            m_oInventoryManger.CloseContainer();

            // confirm the in hand item is correct
            ConfirmPlayerHand();

            // unfreeze the player
            SetFreezePlayer(false);
        }
    }

    //--------------------------------------------------------------------------------------
    // UpdateInHand: Update the current object in the player hand on key press.
    //--------------------------------------------------------------------------------------
    private void UpdateInHand()
    {
        // loop through the weapon selector keys
        for (int i = 0; i < m_akWeaponSelectorControls.Length; i++)
        {
            // if a number key corresponds with a weapon slot
            if (Input.GetKeyDown(m_akWeaponSelectorControls[i]))
            {
                // Confirm that it is indeed a change in selection
                if (m_nWeaponSelectorPos != i)
                {
                    // move the weapon selector pos
                    m_nWeaponSelectorPos = i;

                    // change the weapon in the hand of the player
                    ChangeItemInHand(m_nWeaponSelectorPos);
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // ChangeWeaponInHand: Change the weapon object in the player hand.
    //
    // Param:
    //      nIndex: The index to corresponding weapon item stack.
    //--------------------------------------------------------------------------------------
    private void ChangeItemInHand(int nIndex)
    {
        // Set the fov back to default
        SetFOVDefault();

        // get the item from weapons inventory based on passed in index
        ItemStack oItem = m_oWeapons.GetStackInSlot(nIndex);

        // if item stack is not empty, set the item to in hand
        if (!oItem.IsStackEmpty())
            m_gArm.GetComponent<Arm>().SetInHandItemStack(oItem);

        // if item is empty
        if (oItem.IsStackEmpty())
        {
            // Set the in hand item stack to empty
            m_gArm.GetComponent<Arm>().SetInHandItemStack(ItemStack.m_oEmpty);

            // Set the cursor back to default
            CustomCursor.m_oInstance.SetDefaultCursor();
        }
    }

    //--------------------------------------------------------------------------------------
    // ConfirmPlayerHand: Confirm the object in hand matches the current selection.
    //--------------------------------------------------------------------------------------
    private void ConfirmPlayerHand()
    {
        // get the item that is currently selected on the hotbar
        ItemStack oItem = m_oWeapons.GetStackInSlot(m_nWeaponSelectorPos);

        // Is the currently selected item current in the players hand?
        // if not put the item in the player hand
        if (oItem != m_gArm.GetComponent<Arm>().GetInHandItemStack())
            ChangeItemInHand(m_nWeaponSelectorPos);
        
        // else if the currently selected item is empty but the item in the players hand is not empty then 
        // the hand needs updating to remove incorrect selection.
        else if (oItem.IsStackEmpty() && m_gArm.GetComponent<Arm>().GetInHandItemStack().IsStackEmpty())
            ChangeItemInHand(m_nWeaponSelectorPos);
    }

    //--------------------------------------------------------------------------------------
    // SetFreezePlayer: Set the freeze status of the player object. Used for ensuring the
    // player stays still, good for open menus or possibly cut scenes, etc.
    //
    // Param:
    //      bFreeze: bool for setting the freeze status.
    //--------------------------------------------------------------------------------------
    public void SetFreezePlayer(bool bFreeze)
    {
        // set the player freeze bool
        m_bFreezePlayer = bFreeze;

        // make sure the player is forzen further by constricting ridgidbody
        if (bFreeze)
            m_rbRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        else if (!bFreeze)
            m_rbRigidBody.constraints = RigidbodyConstraints2D.None;

        // freeze the player arm
        m_gArm.GetComponent<Arm>().SetFreezeArm(bFreeze);
    }

    //--------------------------------------------------------------------------------------
    // Interaction: Function interacts on button press with interactables objects.
    //--------------------------------------------------------------------------------------
    private void Interaction()
    {
        // If the interaction button is pressed.
        if (Input.GetKeyUp(KeyCode.E) && InteractionCallback != null)
        {
            // Run interaction delegate.
            InteractionCallback(gameObject.GetComponent<Player>());

            // Confirm the player hand
            ConfirmPlayerHand();
        }
    }

    //--------------------------------------------------------------------------------------
    // SetBodyColorRPC: A server function to Set a new color for the players body 
    // sprite renderer.
    //
    // Param:
    //      cColor: Color value for setting new body color
    //--------------------------------------------------------------------------------------
    [ServerRpc (RequireOwnership = false)]
    public void SetBodyColorServerRpc(Color cColor)
    {
        // Check if color is valid
        if (cColor == null)
            return;

        // Change body color and update body renderer
        mn_cBodyColor.Value = cColor;
    }

    //--------------------------------------------------------------------------------------
    // UpdatePlayerColor: Event to update the sprite renderer color for the players body.
    //
    // Params:
    //      cOldColor: The previous color before the change event triggered.
    //      cNewColor: The new color that triggered the change event.
    //--------------------------------------------------------------------------------------
    private void OnBodyColorChange(Color cOldColor, Color cNewColor)
    {
        // Check if player is a client
        if (!IsClient)
            return;

        // Set the color of the body sprite renderer to network var for player color
        m_srBody.color = cNewColor;
    }
}