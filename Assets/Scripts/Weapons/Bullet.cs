//--------------------------------------------------------------------------------------
// Purpose: The main logic of the Bullet object. 
//
// Description: This script is the base object for bullets in the project. This script
// will mainly handle moving bullets and what to do on collision.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Bullet object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class Bullet : MonoBehaviour
{
    // BULLET SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Bullet Settings:")]

    // Public float for the bullet speed.
    [LabelOverride("Speed")]
    [Tooltip("The speed the bullet will fly from the gun across the map.")]
    public float m_fSpeed = 15.0f;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PROTECTED VALUES //
    //--------------------------------------------------------------------------------------
    // protected rigidbody for bullet
    protected Rigidbody2D m_rbRigidBody;

    // protected vector2 for bullet direction.
    protected Vector2 m_v2Direction;
    //--------------------------------------------------------------------------------------




   

    public float damage = 10.0f;

    public bool laser = false;






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
    }

    //--------------------------------------------------------------------------------------
    // SetDirection: Direction setter for the bullet.
    //
    // Param:
    //      v2NewDirection: a Vector2 for the new direction to set.
    //--------------------------------------------------------------------------------------
    public void SetDirection(Vector2 v2NewDirection)
    {
        // Update direction.
        m_v2Direction = v2NewDirection;
    }

    //--------------------------------------------------------------------------------------
    // OnCollisionEnter2D: When this object collides with another object call this function.
    //
    // Param:
    //      cObject: a Collision2D for what object has had a collision.
    //--------------------------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D cObject)
    {
        // Set to inactive on collision.
        gameObject.SetActive(false);
    }
}