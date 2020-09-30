//--------------------------------------------------------------------------------------
// Purpose: The main logic for the Player object.
//
// Description: This script will handled most of the typically work that a player has to
// do like movement, interaction, opening menus, etc.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Player object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class Player : MonoBehaviour
{
    // MOVEMENT //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Movement:")]

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

    // private float for the current speed of the player.
    private float m_fCurrentSpeed;

    // private flkoat for the current exhaust level of the player.
    private float m_fRunCurrentExhaust = 0.0f;

    // private bool for if ther player can run or not
    private bool m_bExhausted = false;

    // private bool for freezing the player
    private bool m_bFreezePlayer = false;
    //--------------------------------------------------------------------------------------

    // DELEGATES //
    //--------------------------------------------------------------------------------------
    // Create a new Delegate for handling the interaction functions.
    public delegate void InteractionEventHandler();

    // Create an event for the delegate for extra protection. 
    public InteractionEventHandler InteractionCallback;
    //--------------------------------------------------------------------------------------




    //
    [SerializeField] private FieldOfView fieldOfView;





    //--------------------------------------------------------------------------------------
    // Initialization
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // Get the Rigidbody.
        m_rbRigidBody = GetComponent<Rigidbody2D>();

        // set the current speed of the player to walk
        m_fCurrentSpeed = m_fWalkSpeed;
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // Run the interaction function
        Interaction();
    }

    //--------------------------------------------------------------------------------------
    // FixedUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void FixedUpdate()
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
        Vector3 v3Position = Camera.main.WorldToScreenPoint(transform.position);

        // Get the  mouse direction.
        Vector3 v3Direction = Input.mousePosition - v3Position;

        // Work out the angle.
        float fAngle = Mathf.Atan2(v3Direction.y, v3Direction.x) * Mathf.Rad2Deg;

        // Update the rotation.
        transform.rotation = Quaternion.AngleAxis(fAngle, Vector3.forward);


        Vector3 target = fieldOfView.GetMouseWorldPosition();
        Vector3 aimdir = (target - transform.position).normalized;

        fieldOfView.SetAimDirection(aimdir);
        fieldOfView.SetOrigin(transform.position);
    }

    //--------------------------------------------------------------------------------------
    // GetFreezePlayer: Get the current freeze status of the player. 
    //
    // Return:
    //      bool: the current freeze staus of the player.
    //--------------------------------------------------------------------------------------
    public bool GetFreezePlayer()
    {
        // get the player freeze bool
        return m_bFreezePlayer;
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

    }

    //--------------------------------------------------------------------------------------
    // Interaction: Function interacts on button press with interactables objects.
    //--------------------------------------------------------------------------------------
    public void Interaction()
    {
        // If the interaction button is pressed.
        if (Input.GetKeyUp(KeyCode.E) && InteractionCallback != null)
        {
            // Run interaction delegate.
            InteractionCallback();
        }
    }
}