//--------------------------------------------------------------------------------------
// Purpose: Render an FOV Mesh.
//
// Description: This script will generate and render the field of view vision cone for
// the player. It is created using a mesh that takes into account other objects around it
// to change its shape.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// FieldOfView object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class FieldOfView : MonoBehaviour
{
    // FOV SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("FOV Settings:")]

    // public layer mask for the layers that will effect the mesh
    [LabelOverride("Effect Layers")] [Tooltip("The layers that will make an effect on the rendering of the fov.")]
    public LayerMask m_lmEffectLayers;

    // public float for the distance of the mesh
    [LabelOverride("View Distance")] [Tooltip("The view distance of the field of view.")]
    public float m_fViewDistance = 15.0f;

    // public float for size of the field of view
    [LabelOverride("Field Of View")] [Tooltip("The size (or width) of the field of view.")]
    public float m_fFOV = 80.0f;

    // public float for the lerp smoothing value
    [LabelOverride("Lerp Smoothing")] [Tooltip("The Smoothing value for the lerp between different distances and fields of view.")]
    public float m_fLerpSmoothing = 4;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // mesh object for drawing the vision cone / fov
    private Mesh m_meshVisionCone;

    // vector 3 for the mesh origin
    private Vector3 m_v3Origin;

    // float for the current fov
    private float m_fCurrentFOV;

    // float for the current view distance
    private float m_fCurrentViewDistance;

    // float for current lerp smoothing
    private float m_fCurrentLerpSmoothing;

    // float for mesh starting angle
    private float m_fStartingAngle;

    // float for lerp view distance storing
    private float m_fLerpViewDistance;

    // float for lerp fov storing
    private float m_fLerpFOV;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // Create new mesh component
        m_meshVisionCone = new Mesh();

        // Set the mesh filter to the newly created mesh
        GetComponent<MeshFilter>().mesh = m_meshVisionCone;

        // set the origin to zero
        m_v3Origin = Vector3.zero;

        // set the current fov to the public value
        m_fCurrentFOV = m_fFOV;

        // set the current view distance to the public value
        m_fCurrentViewDistance = m_fViewDistance;

        // set the current lerp smoothing to the public value
        m_fCurrentLerpSmoothing = m_fLerpSmoothing;

        // set the starting angle to 0
        m_fStartingAngle = 0;

        // set the lerp view distance to the public value
        m_fLerpViewDistance = m_fViewDistance;

        // set the lerp fov to the public value
        m_fLerpFOV = m_fFOV;
    }

    //--------------------------------------------------------------------------------------
    // LateUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void LateUpdate()
    {
        // Check if there is any changes to the current fov and view distance. if there is lerp to the new value.
        m_fCurrentFOV = Mathf.Lerp(m_fCurrentFOV, m_fLerpFOV, Time.deltaTime * m_fCurrentLerpSmoothing);
        m_fCurrentViewDistance = Mathf.Lerp(m_fCurrentViewDistance, m_fLerpViewDistance, Time.deltaTime * m_fCurrentLerpSmoothing);

        // Set up the mesh values
        int nRayCount = 50;
        float fAngle = m_fStartingAngle;
        float fAngleIncrease = m_fCurrentFOV / nRayCount;
        
        // set up vertices
        Vector3[] av3Vertices = new Vector3[nRayCount + 1 + 1];
        Vector2[] av2UV = new Vector2[av3Vertices.Length];
        
        // set up[ triangles 
        int[] anTriangles = new int[nRayCount * 3];
        av3Vertices[0] = m_v3Origin;
        
        // set up indexs
        int nVertexIndex = 1;
        int nTriangleIndex = 0;

        // loop all the rays
        for (int i = 0; i <= nRayCount; i++)
        {
            // new vector3 for a vertex
            Vector3 v3Vertex;

            // Cast at ray out from the origin
            RaycastHit2D rcRaycastHit2D = Physics2D.Raycast(m_v3Origin, GetVectorFromAngle(fAngle), m_fCurrentViewDistance, m_lmEffectLayers);

            // if collider is null, meaning nothing is hit
            if (rcRaycastHit2D.collider == null)
            {
                // draw at maximum distance
                v3Vertex = m_v3Origin + (GetVectorFromAngle(fAngle) * m_fCurrentViewDistance);
            }

            // else if the collider has hit
            else
            {
                // place vertex exaclty on the point where hit
                v3Vertex = rcRaycastHit2D.point;
            }

            // add new vertex to vertices
            av3Vertices[nVertexIndex] = v3Vertex;

            // If not the first ray
            if (i > 0)
            {
                // generate triangles
                anTriangles[nTriangleIndex + 0] = 0;
                anTriangles[nTriangleIndex + 1] = nVertexIndex - 1;
                anTriangles[nTriangleIndex + 2] = nVertexIndex;
                nTriangleIndex += 3;
            }

            // move on to next angle
            nVertexIndex++;
            fAngle -= fAngleIncrease;
        }

        // Apply newly created mesh to the mesh component
        m_meshVisionCone.vertices = av3Vertices;
        m_meshVisionCone.uv = av2UV;
        m_meshVisionCone.triangles = anTriangles;

        // ensure mesh renderers outside of camera view
        m_meshVisionCone.RecalculateBounds();
    }

    //--------------------------------------------------------------------------------------
    // GetVectorFromAngle: Get a vector 3 value from a float angle.
    //
    // Param:
    //      fAngle: The angle to calculate vector
    //
    // Return:
    //      Vector3: The vector 3 from float angle
    //--------------------------------------------------------------------------------------
    private Vector3 GetVectorFromAngle(float fAngle)
    {
        // calculate vector from angle
        float fAngleRad = fAngle * (Mathf.PI / 180.0f);
        Vector3 v3NewAngle = new Vector3(Mathf.Cos(fAngleRad), Mathf.Sin(fAngleRad));

        // return the new angle
        return v3NewAngle;
    }

    //--------------------------------------------------------------------------------------
    // GetAngleFromVector: Get float angle from a vector 3
    //
    // Param:
    //      v3Direction: A vector 3 for the angle direction.
    //
    // Return:
    //      float: float for the angle from vector 3.
    //--------------------------------------------------------------------------------------
    private float GetAngleFromVector(Vector3 v3Direction)
    {
        //normalize the direction
        v3Direction = v3Direction.normalized;

        // calculate angle
        float fAngle = Mathf.Atan2(v3Direction.y, v3Direction.x) * Mathf.Rad2Deg;

        // make sure the angle isnt 0
        if (fAngle < 0)
            fAngle += 360;

        // return the angle
        return fAngle;
    }

    //--------------------------------------------------------------------------------------
    // GetMouseWorldPosition: Get the mouse position in the world.
    //
    // Return:
    //      Vector3: The mouse position.
    //--------------------------------------------------------------------------------------
    public Vector3 GetMouseWorldPosition()
    {
        // vector 3 for the mouse position in the world
        Vector3 v3Vector = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);

        // set the z vector to 0
        v3Vector.z = 0.0f;
        
        // return the mosue pos
        return v3Vector;
    }

    //--------------------------------------------------------------------------------------
    // GetMouseWorldPositionWithZ: Get the mouse position in the world in 3D space.
    //
    // Return:
    //      Vector3: The mouse position.
    //--------------------------------------------------------------------------------------
    public Vector3 GetMouseWorldPositionWithZ()
    {
        // return the mouse pos
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    //--------------------------------------------------------------------------------------
    // GetMouseWorldPositionWithZ: Get the mouse position in the world in 3D space taking in
    // custom camera object.
    //
    // Param:
    //      cCamera: The camera used for calculating mouse pos.
    //
    // Return:
    //      Vector3: The mouse position.
    //--------------------------------------------------------------------------------------
    public Vector3 GetMouseWorldPositionWithZ(Camera cCamera)
    {
        // return the mouse pos
        return GetMouseWorldPositionWithZ(Input.mousePosition, cCamera);
    }

    //--------------------------------------------------------------------------------------
    // GetMouseWorldPositionWithZ: Get the mouse position in the world in 3D space taking in
    // custom camera object and screen pos.
    //
    // Param:
    //      v3ScreenPos: Vector3 for the position of screen in world.
    //      cCamera: The camera used for calculating mouse pos.
    //
    // Return:
    //      Vector3: The mouse position.
    //--------------------------------------------------------------------------------------
    public Vector3 GetMouseWorldPositionWithZ(Vector3 v3ScreenPos, Camera cCamera)
    {
        // vector 3 for the mouse position in the world
        Vector3 v3WorldPos = cCamera.ScreenToWorldPoint(v3ScreenPos);

        // return the mouse pos
        return v3WorldPos;
    }

    //--------------------------------------------------------------------------------------
    // SetOrigin: Set the origin of the field of view.
    //
    // Param:
    //      v3Origin: A vector 3 value to set to the orign value.
    //--------------------------------------------------------------------------------------
    public void SetOrigin(Vector3 v3Origin)
    {
        // set the member orign
        m_v3Origin = v3Origin;
    }

    //--------------------------------------------------------------------------------------
    // SetAimDirection: Set the direction of the field of view.
    //
    // Param:
    //      v3AimDirection: A vector 3 value to set to the starting angle value.
    //--------------------------------------------------------------------------------------
    public void SetAimDirection(Vector3 v3AimDirection)
    {
        // Set aim direction taking into account the current fov
        m_fStartingAngle = GetAngleFromVector(v3AimDirection) + m_fCurrentFOV / 2.0f;
    }

    //--------------------------------------------------------------------------------------
    // SetFOV: Set the Lerp FOV value.
    //
    // Param:
    //      fValue: A float value to set to the Lerp FOV value.
    //--------------------------------------------------------------------------------------
    public void SetFOV(float fValue)
    {
        // set the lerp fov
        m_fLerpFOV = fValue;
    }

    //--------------------------------------------------------------------------------------
    // SetViewDistance: Set the Lerp View Distance value.
    //
    // Param:
    //      fValue: A float value to set to the Lerp View Distance value.
    //--------------------------------------------------------------------------------------
    public void SetViewDistance(float fValue)
    {
        // set the lerp view distance
        m_fLerpViewDistance = fValue;
    }

    //--------------------------------------------------------------------------------------
    // SetLerpSmoothing: Set the Lerp Smoothing value.
    //
    // Param:
    //      fValue: A float value to set to the Lerp Smoothing value.
    //--------------------------------------------------------------------------------------
    public void SetLerpSmoothing(float fValue)
    {
        // set the current lerp smoothing
        m_fCurrentLerpSmoothing = fValue;
    }

    //--------------------------------------------------------------------------------------
    // SetDefaultViewDistance: Sets Lerp View Distance value back to the default.
    //--------------------------------------------------------------------------------------
    public void SetDefaultViewDistance()
    {
        // set the lerp view distance back to default
        m_fLerpViewDistance = m_fViewDistance;
    }

    //--------------------------------------------------------------------------------------
    // SetDefaultFOV: Sets Lerp FOV value back to the default.
    //--------------------------------------------------------------------------------------
    public void SetDefaultFOV()
    {
        // set the lerp fov back to default
        m_fLerpFOV = m_fFOV;
    }

    //--------------------------------------------------------------------------------------
    // SetDefaultLerpSmoothing: Sets Lerp Smoothing value back to the default.
    //--------------------------------------------------------------------------------------
    public void SetDefaultLerpSmoothing()
    {
        // set the current lerp smoothing back to default
        m_fCurrentLerpSmoothing = m_fLerpSmoothing;
    }
}