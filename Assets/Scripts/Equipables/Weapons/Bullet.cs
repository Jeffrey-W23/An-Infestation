﻿//--------------------------------------------------------------------------------------
// Purpose: The main logic of the Bullet object. 
//
// Description: This script is the base object for bullets in the project. This script
// will mainly handle moving bullets and what to do on collision.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using MLAPI;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Bullet object. Inheriting from NetworkBehaviour.
//--------------------------------------------------------------------------------------
public class Bullet : NetworkBehaviour
{
    // BULLET SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Bullet Settings:")]

    // Public float for the bullet speed.
    [LabelOverride("Speed")] [Tooltip("The speed the bullet will fly from the gun across the map.")]
    public float m_fSpeed = 15.0f;

    // public float for the max travel distance of the bullet
    [LabelOverride("Max Travel Distance")] [Tooltip("The max travel distance of the bullet before despawn.")]
    public float m_fTravelDistance = 50;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // INVENTORY SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Inventory Settings:")]

    // public item value for assigning the item that represents this object in the inventory system
    [LabelOverride("Item In Inventory")] [Tooltip("The item that represents this item in the inventory system.")]
    public Item m_oItemInInventory;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PROTECTED VALUES //
    //--------------------------------------------------------------------------------------
    // protected rigidbody for bullet
    protected Rigidbody2D m_rbRigidBody;

    // protected vector2 for bullet direction.
    protected Vector2 m_v2Direction;

    // protected vector3 for the spawn position of the bullet
    protected Vector3 m_v3SpawnPosition;
    //--------------------------------------------------------------------------------------

    // STANDARD GETTERS / SETTERS //
    //--------------------------------------------------------------------------------------
    // Setter of type Vector3 for setting the spawn position of the bullet
    public void SetSpawnPosition(Vector3 v3Pos) { m_v3SpawnPosition = v3Pos; }

    // Setter of type Vector2 for setting the direction of the bullet
    public void SetDirection(Vector2 v2NewDirection) { m_v2Direction = v2NewDirection; }
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    protected void Awake()
    {
        // Get the bullet Rigidbody
        m_rbRigidBody = GetComponent<Rigidbody2D>();
    }

    //--------------------------------------------------------------------------------------
    // FixedUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    protected void FixedUpdate()
    {
        // Update the bullets velocity, direction etc.
        transform.right = m_v2Direction;
        m_rbRigidBody.velocity = (m_v2Direction * m_fSpeed * Time.fixedDeltaTime);
        m_rbRigidBody.angularVelocity = 0;

        // Check the bullet distance
        CheckBulletDistance();
    }

    //--------------------------------------------------------------------------------------
    // CheckBulletDistance: Check if the bullet has hit the max distance.
    //--------------------------------------------------------------------------------------
    protected void CheckBulletDistance()
    {
        // if the bullet position is past the max distance
        if (Vector3.Distance(m_v3SpawnPosition, transform.position) > m_fTravelDistance)
        {
            // Set to inactive on collision.
            gameObject.SetActive(false);
        }
    }

    //--------------------------------------------------------------------------------------
    // OnTriggerEnter2D: Function is called when the Collider cObject enters the trigger.
    //
    // Param:
    //      cObject: The other Collider invloved in the collision.
    //--------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D cObject)
    {
        // Set to inactive on collision.
        gameObject.SetActive(false);
    }
}