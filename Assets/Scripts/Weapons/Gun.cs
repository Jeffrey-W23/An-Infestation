//--------------------------------------------------------------------------------------
// Purpose: The main logic for the gun object.
//
// Description: This script is the base object for gun in the project. This script
// will handle spawning of bullets, controling ammo, etc.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Gun object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class Gun : MonoBehaviour
{
    // BULLET SETUP //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Bullet Setup:")]

    // Public gameobject for bullet object.
    [LabelOverride("Bullet Object")] [Tooltip("The bullet that this gun will fire.")]
    public GameObject m_gBulletBlueprint;

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

    // public float for the fire rate of the gun
    [LabelOverride("Transition Smoothing")] [Tooltip("Smoothing value for transitioning the down sights action.")]
    public float m_fDownSightsSmoothing = 4.0f;

    // public float for the fire rate of the gun
    [LabelOverride("Distance")] [Tooltip("The distance of the field of view when down sights.")]
    public float m_fDownSightsDistance = 20.0f;

    // public float for the fire rate of the gun
    [LabelOverride("Field Of View")] [Tooltip("The width of the field of view when down sights.")]
    public float m_fDownSightsFOV = 30.0f;

    // public float for the fire rate of the gun
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

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PROTECTED VALUES //
    //--------------------------------------------------------------------------------------
    // An Array of GameObjects for bullets.
    protected GameObject[] m_gBulletList;

    // playber object for getting player script
    protected Player m_sPlayer;

    // bool for if the gun is down sights
    protected bool m_bDownSights = false;

    // float for the current camera position
    protected float m_fCurrentCameraPos = 8.0f;

    // float for when the bullet can fire next
    protected float m_fNextfire = 0.0f;

    // float for the fire rate timer
    protected float m_fFireRateTimer = 0.0f;

    // float for storing the default camera pos
    protected float m_fCameraPosA;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    protected void Awake()
    {
        // Get the player object
        m_sPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // Set camera zoom
        m_fCameraPosA = Camera.main.orthographicSize;

        // initialize bullet list with size.
        m_gBulletList = new GameObject[m_nPoolSize];

        // Go through each bullet.
        for (int i = 0; i < m_nPoolSize; ++i)
        {
            // Instantiate and set active state.
            m_gBulletList[i] = Instantiate(m_gBulletBlueprint);
            m_gBulletList[i].SetActive(false);
        }
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    protected void Update()
    {
        // is player allowed to move
        if (!m_sPlayer.GetFreezePlayer())
        {
            // Shoot the bullet
            ShootBullet();

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

        // If the mouse is pressed.
        if (Input.GetMouseButton(0) && m_fFireRateTimer > m_fNextfire)
        {
            // set the bullet as fired
            m_fNextfire = m_fFireRateTimer + m_fFireRate;

            // Allocate a bullet to the pool.
            GameObject gBullet = Allocate();

            // if a valid bullet and not null.
            if (gBullet)
            {
                // Update the postion, rotation and set direction of the bullet.
                gBullet.transform.position = m_gBulletSpawn.transform.position;
                gBullet.transform.rotation = m_gBulletSpawn.transform.rotation;
                gBullet.GetComponent<Bullet>().SetDirection(transform.right);
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // AimDownSights: Aim in and out of sights on right click.
    //--------------------------------------------------------------------------------------
    protected void AimDownSights()
    {
        // float for storing the camera pos
        float fCameraPosB = m_fDownSightsZoom;

        // If the mouse right is pressed
        if (Input.GetMouseButtonDown(1))
        {
            // toggle the down sight bool
            m_bDownSights = !m_bDownSights;

            // if aimming down sights
            if (m_bDownSights)
            {
                // set the fov, distance and lerp smoothing of the player vision
                m_sPlayer.GetPlayerVisionScript().SetViewDistance(m_fDownSightsDistance);
                m_sPlayer.GetPlayerVisionScript().SetFOV(m_fDownSightsFOV);
                m_sPlayer.GetPlayerVisionScript().SetLerpSmoothing(m_fDownSightsSmoothing);

                // set the fov, distance and lerp smoothing of the enemy renderer
                m_sPlayer.GetEnemyRendererScript().SetViewDistance(m_fDownSightsDistance);
                m_sPlayer.GetEnemyRendererScript().SetFOV(m_fDownSightsFOV);
                m_sPlayer.GetEnemyRendererScript().SetLerpSmoothing(m_fDownSightsSmoothing);

                // change the camera zoom
                m_fCurrentCameraPos = fCameraPosB;
            }

            // else if not down sights
            else
            {
                // set the fov, distance and lerp smoothing of the player vision
                m_sPlayer.GetPlayerVisionScript().SetDefaultViewDistance();
                m_sPlayer.GetPlayerVisionScript().SetDefaultFOV();
                m_sPlayer.GetPlayerVisionScript().SetDefaultLerpSmoothing();

                // set the fov, distance and lerp smoothing of the enemy renderer
                m_sPlayer.GetEnemyRendererScript().SetDefaultViewDistance();
                m_sPlayer.GetEnemyRendererScript().SetDefaultFOV();
                m_sPlayer.GetEnemyRendererScript().SetDefaultLerpSmoothing();

                // change the camera zoom
                m_fCurrentCameraPos = m_fCameraPosA;
            }
        }

        // Set the new camera position with a lerp
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, m_fCurrentCameraPos, Time.deltaTime * m_fDownSightsSmoothing);
    }

    //--------------------------------------------------------------------------------------
    // Allocate: Allocate bullets to the pool.
    //
    // Return:
    //      GameObject: Current gameobject in the pool.
    //--------------------------------------------------------------------------------------
    protected GameObject Allocate()
    {
        // For each in the pool.
        for (int i = 0; i < m_nPoolSize; ++i)
        {
            // Check if active.
            if (!m_gBulletList[i].activeInHierarchy)
            {
                // Set active state.
                m_gBulletList[i].SetActive(true);

                // return the bullet.
                return m_gBulletList[i];
            }
        }

        // if all fail return null.
        return null;
    }
}