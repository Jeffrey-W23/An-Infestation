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
using MLAPI.Messaging;
using MLAPI.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Gun object. Inheriting from Equipable.
//--------------------------------------------------------------------------------------
public class Gun : Equipable
{
    // BULLET SETUP //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Bullet Setup:")]

    // Public gameobject for where to spawn the bullet
    [LabelOverride("Bullet Spawn Location")] [Tooltip("A Gameobject for where to exactly spawn the bullet.")]
    public GameObject m_gBulletSpawn;

    // Public item for bullet object.
    [LabelOverride("Bullet Item")] [Tooltip("The bullet item that this gun will fire.")]
    public Item m_oBulletBlueprint;

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

    // PROTECTED VALUES //
    //--------------------------------------------------------------------------------------
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
    protected new void Awake()
    {
        // Run the base awake
        base.Awake();
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    protected new void Update()
    {
        // Run the base update
        base.Update();

        // is player allowed to move and not null, shoot the bullet
        if (m_oPlayer != null && !m_oPlayer.GetFrozenStatus())
            ShootBullet();
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

                // Shoot a bullet on the server
                FireBulletServerRpc();

                // update the current ammo of the gun
                UpdateCurrentAmmo(m_nAmmoUsage);
            }
        }
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
    // ActivateBullet: Grab a bullet from the object pool and prepare it for fire.
    //--------------------------------------------------------------------------------------
    private void ActivateBullet()
    {
        // Grab a bullet from the spawm managers bullet pool
        GameObject gBullet = SpawnManager.m_oInstance.GetBulletFromPool();

        // Prepare bullet for firing
        gBullet.transform.position = m_gBulletSpawn.transform.position;
        gBullet.transform.rotation = m_gBulletSpawn.transform.rotation;
        gBullet.GetComponent<Bullet>().SetDirection(transform.right);
        gBullet.GetComponent<Bullet>().SetSpawnPosition(m_gBulletSpawn.transform.position);

        // Set the bullet to active
        gBullet.SetActive(true);
    }

    //--------------------------------------------------------------------------------------
    // FireBulletServerRpc: A Server function for initating bullet fire.
    //--------------------------------------------------------------------------------------
    [ServerRpc]
    private void FireBulletServerRpc()
    {
        // Tell the clients that this player wishes to fire
        FireBulletClientRpc();
    }

    //--------------------------------------------------------------------------------------
    // FireBulletClientRpc: A Client function for initating bullet fire.
    //--------------------------------------------------------------------------------------
    [ClientRpc]
    private void FireBulletClientRpc()
    {
        // Activate a bullet on all the clients
        ActivateBullet();
    }
}