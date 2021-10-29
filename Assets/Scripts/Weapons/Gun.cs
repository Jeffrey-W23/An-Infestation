//--------------------------------------------------------------------------------------
// Purpose: The main logic for the gun object.
//
// Description: This script is the base object for gun in the project. This script
// will handle spawning of bullets, controling ammo, etc.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Gun object. Inheriting from NetworkBehaviour.
//--------------------------------------------------------------------------------------
public class Gun : NetworkBehaviour
{
    // BULLET SETUP //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Bullet Setup:")]

    // Public item for bullet object.
    [LabelOverride("Bullet Item")] [Tooltip("The bullet item that this gun will fire.")]
    public Item m_oBulletBlueprint;

    // Public gameobject for where to spawn the bullet
    [LabelOverride("Bullet Spawn Location")] [Tooltip("A Gameobject for where to exactly spawn the bullet.")]
    public GameObject m_gBulletSpawn;

    // public int for pool size.
    [LabelOverride("Pool Size")] [Tooltip("How many bullets allowed on screen at one time.")]
    public int m_nPoolSize;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // DOWN SIGHTS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Down Sight Settings:")]

    //  public float for smoothing the lerp for fov
    [LabelOverride("FOV Transition Smoothing")] [Tooltip("Smoothing value for transitioning the fov in down sights action.")]
    public float m_fDownSightsFOVSmoothing = 4.0f;

    // public float for smoothing the lerp for the camera
    [LabelOverride("Camera Transition Smoothing")] [Tooltip("Smoothing value for transitioning the camera in down sights action.")]
    public float m_fDownSightsCameraSmoothing = 4.0f;

    // public float for the distance to set when down sights
    [LabelOverride("Distance")] [Tooltip("The distance of the field of view when down sights.")]
    public float m_fDownSightsDistance = 20.0f;

    // public float for the field of view when down sights
    [LabelOverride("Field Of View")] [Tooltip("The width of the field of view when down sights.")]
    public float m_fDownSightsFOV = 30.0f;

    // public float for the zoom of the camera when down sights
    [LabelOverride("Zoom")] [Tooltip("The zoom of the camera when down sights.")]
    public float m_fDownSightsZoom = 10.0f;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // GUN SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Gun Settings:")]

    // public float for the fire rate of the gun
    [LabelOverride("Fire Rate")] [Tooltip("Fire rate of gun based on time.")]
    public float m_fFireRate = 0.3f;

    // public int for the ammo usage of the gun
    [LabelOverride("Ammo Usage")] [Tooltip("The amount of ammo used every shoot taken.")]
    public int m_nAmmoUsage = 1;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // OTHER SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Other Settings:")]

    // public texture2d for the guns crosshair
    [LabelOverride("Gun Crosshair")] [Tooltip("The cursor to use for the guns crosshair.")]
    public Texture2D m_tCrosshair;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PROTECTED VALUES //
    //--------------------------------------------------------------------------------------
    // An Array of GameObjects for bullets.
    protected GameObject[] m_agBulletList;

    // playber object for getting player script
    protected Player m_oPlayer;

    // bool for if the gun is down sights
    protected bool m_bDownSights = false;

    // float for when the bullet can fire next
    protected float m_fNextfire = 0.0f;

    // float for the fire rate timer
    protected float m_fFireRateTimer = 0.0f;
    
    // An int for the current ammo of the gun
    protected int m_nCurrentAmmo = 0;

    // An int of the current stack of ammo in use
    protected int m_nCurrentAmmoStack = -1;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    protected void Awake()
    {
        // Get the player object
        m_oPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // set the crosshair of the gun
        CustomCursor.m_oInstance.SetCustomCursor(m_tCrosshair);

        // initialize bullet list with size.
        m_agBulletList = new GameObject[m_nPoolSize];

        // Go through each bullet.
        for (int i = 0; i < m_nPoolSize; ++i)
        {
            // Instantiate and set active state.
            m_agBulletList[i] = Instantiate(m_oBulletBlueprint.m_gSceneObject);
            m_agBulletList[i].SetActive(false);
        }
    }

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    protected void Start()
    {
        // Check current ammo
        CheckCurrentAmmo();
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    protected void Update()
    {
        // is player allowed to move
        if (!m_oPlayer.GetFreezePlayer())
        {
            // Shoot the bullet
            ShootBullet();

            // if the fov is off set down sights to false
            if (!m_oPlayer.GetPlayerVisionScript().GetToggleState())
                m_bDownSights = false;

            // Aim down sights
            AimDownSights();
        }
    }

    //--------------------------------------------------------------------------------------
    // ShootBullet: Shoot the bullet from the gun on left click.
    //--------------------------------------------------------------------------------------
    protected void ShootBullet()
    {
        // start the fire rate timer
        m_fFireRateTimer += Time.deltaTime;

        // Check the current ammo before starting fire
        if (CheckCurrentAmmo())
        {
            // If the mouse is pressed.
            if (Input.GetMouseButton(0) && m_fFireRateTimer > m_fNextfire && m_nCurrentAmmo >= 1)
            {
                // set the bullet as fired
                m_fNextfire = m_fFireRateTimer + m_fFireRate;

                // Allocate a bullet from the pool.
                GameObject gBullet = AllocateBullet();

                // if a valid bullet and not null.
                if (gBullet)
                {
                    // Update the postion, rotation and set direction of the bullet, as well as pass on the spawn pos.
                    gBullet.transform.position = m_gBulletSpawn.transform.position;
                    gBullet.transform.rotation = m_gBulletSpawn.transform.rotation;
                    gBullet.GetComponent<Bullet>().SetDirection(transform.right);
                    gBullet.GetComponent<Bullet>().SetSpawnPosition(m_gBulletSpawn.transform.position);
                }

                // update the current ammo of the gun
                UpdateCurrentAmmo(m_nAmmoUsage);
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // Allocate: Allocate bullets to the pool.
    //
    // Return:
    //      GameObject: Current gameobject in the pool.
    //--------------------------------------------------------------------------------------
    protected GameObject AllocateBullet()
    {
        // For each in the pool.
        for (int i = 0; i < m_nPoolSize; ++i)
        {
            // Check if active.
            if (!m_agBulletList[i].activeInHierarchy)
            {
                // Set active state.
                m_agBulletList[i].SetActive(true);

                // return the bullet.
                return m_agBulletList[i];
            }
        }

        // if all fail return null.
        return null;
    }

    //--------------------------------------------------------------------------------------
    // CheckCurrentAmmo: Check the ammo in the inventory system and make sure the gun is
    // up to date with this information.
    //--------------------------------------------------------------------------------------
    protected bool CheckCurrentAmmo()
    {
        // get the player inventory
        Inventory oPlayerInventory = m_oPlayer.GetInventory();

        // bool to check if the player has ammo
        bool bHasAmmo = false;

        // loop through the player inventory
        for (int i = 0; i < oPlayerInventory.GetArray().Count; i++)
        {
            // if the inventory item stack is not empty and not null
            if (!oPlayerInventory.GetStackInSlot(i).IsStackEmpty() && oPlayerInventory.GetStackInSlot(i).GetItem() != null)
            {
                // if the inventory item stack matches the gun blueprint
                if (oPlayerInventory.GetStackInSlot(i).GetItem() == m_oBulletBlueprint)
                {
                    // Set the current ammo to stack to the id of the slot
                    m_nCurrentAmmoStack = oPlayerInventory.GetStackInSlot(i).m_nSlotId;

                    // set the current ammo to the amount in the stack
                    m_nCurrentAmmo = oPlayerInventory.GetStackInSlot(i).GetItemCount();

                    // the player has ammo in the inventory
                    bHasAmmo = true;

                    // break the loop, ammo stack has been found
                    break;
                }
            }

            // the player has no ammo in the inventory
            bHasAmmo = false;
        }

        // if there is no ammo in the inventory
        if (!bHasAmmo)
        {
            // ammo is 0 and there is no current ammo stack
            m_nCurrentAmmo = 0;
            m_nCurrentAmmoStack = -1;
        }

        // return status
        return bHasAmmo;
    }

    //--------------------------------------------------------------------------------------
    // UpdateCurrentAmmo: Update the ammo in the inventory system.
    // 
    // Param:
    //      nAmount: An int for the amount of ammo to use per shot.
    //--------------------------------------------------------------------------------------
    protected void UpdateCurrentAmmo(int nAmount)
    {
        // get the player inventory
        Inventory oPlayerInventory = m_oPlayer.GetInventory();

        // loop through the player inventory
        for (int i = 0; i < oPlayerInventory.GetArray().Count; i++)
        {
            // if the inventory item stack is not empty and not null
            if (!oPlayerInventory.GetStackInSlot(i).IsStackEmpty() && oPlayerInventory.GetStackInSlot(i).GetItem() != null)
            {
                // if the inventory item stack matches the gun blueprint
                if (oPlayerInventory.GetStackInSlot(i).GetItem() == m_oBulletBlueprint)
                {
                    // if the slot id matches the current ammo stack
                    if (oPlayerInventory.GetStackInSlot(i).m_nSlotId == m_nCurrentAmmoStack)
                    {
                        // update the ammo in the player inventory
                        oPlayerInventory.GetStackInSlot(i).DecreaseStack(nAmount);

                        // set the current ammo to the amount in the stack
                        m_nCurrentAmmo = (oPlayerInventory.GetStackInSlot(i).GetItemCount());
                    }
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // AimDownSights: Aim in and out of sights on right click.
    //--------------------------------------------------------------------------------------
    protected void AimDownSights()
    {
        // If the mouse right is pressed
        if (Input.GetMouseButtonDown(1))
        {
            // Toggle the fov back to true
            m_oPlayer.GetPlayerVisionScript().ToggleFOV(true);
            m_oPlayer.GetEnemyRendererScript().ToggleFOV(true);

            // toggle the down sight bool
            m_bDownSights = !m_bDownSights;

            // if aimming down sights
            if (m_bDownSights)
            {
                // set the fov, distance and lerp smoothing of the player vision
                m_oPlayer.GetPlayerVisionScript().AdjustFOV(m_fDownSightsDistance, m_fDownSightsFOV, m_fDownSightsZoom, m_fDownSightsFOVSmoothing, m_fDownSightsCameraSmoothing);

                // set the fov, distance and lerp smoothing of the enemy renderer
                m_oPlayer.GetEnemyRendererScript().AdjustFOV(m_fDownSightsDistance, m_fDownSightsFOV, m_fDownSightsZoom, m_fDownSightsFOVSmoothing, m_fDownSightsCameraSmoothing);
            }

            // else if not down sights
            else
            {
                // Set the player fov back to default
                m_oPlayer.SetFOVDefault();
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // OnDestroy: Function that runs on the destroy of a game object.
    //--------------------------------------------------------------------------------------
    private void OnDestroy()
    {
        // Go through each bullet.
        for (int i = 0; i < m_nPoolSize; ++i)
        {
            // Destory the bullet list
            Object.Destroy(m_agBulletList[i]);
        }
    }
}