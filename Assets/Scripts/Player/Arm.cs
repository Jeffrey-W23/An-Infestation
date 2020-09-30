// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    // float for the distance between mouse and object.
    float m_fDistanceBetween;

    // Weapon prefab.
    public GameObject m_gWeaponPrefab;

    // The Pistol weapon.
    private GameObject m_gPistol;

    //
    public int ArmBend = 120;

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    void Awake()
    {
        // Set the parenting of pistol prefab.
        m_gPistol = Instantiate(m_gWeaponPrefab);
        m_gPistol.transform.parent = transform;
    }

    //--------------------------------------------------------------------------------------
    // LateUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    void LateUpdate()
    {
        // Get mouse inside camera
        Vector3 v3Pos = Camera.main.WorldToScreenPoint(transform.position);

        // update the distance.
        m_fDistanceBetween = Vector3.Distance(v3Pos, Input.mousePosition);

        // Check the distance between the mouse and arm.
        // if far enough away turn the mouse towards mouse.
        // else stop arm rotation.
        if (m_fDistanceBetween > ArmBend)
        {
            // Get the  mouse direction.
            Vector3 v3Dir = Input.mousePosition - v3Pos;

            // Work out the angle.
            float fAngle = Mathf.Atan2(v3Dir.y, v3Dir.x) * Mathf.Rad2Deg;

            // Update the rotation.
            transform.rotation = Quaternion.AngleAxis(fAngle, Vector3.forward);
        }
    }
}
