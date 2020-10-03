//--------------------------------------------------------------------------------------
// Purpose: Render an FOV Mesh.
//
// Description: 
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

    //
    [LabelOverride("Effect Layers")] [Tooltip("The layers that will make an effect on the rendering of the fov.")]
    public LayerMask m_lmEffectLayers;

    //
    [LabelOverride("View Distance")] [Tooltip("")]
    public float m_fViewDistance = 15.0f;

    //
    [LabelOverride("Field Of View")] [Tooltip("")]
    public float m_fFOV = 80.0f;

    //
    [LabelOverride("Lerp Smoothing")] [Tooltip("")]
    public float m_fLerpSmoothing = 4;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    //
    private Mesh m_meshVisionCone;

    //
    private Vector3 m_v3Origin;

    //
    private float m_fCurrentFOV;

    //
    private float m_fCurrentViewDistance;

    //
    private float m_fCurrentLerpSmoothing;

    //
    private float m_fStartingAngle;

    //
    private float m_fLerpViewDistance;

    //
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









        //
        for (int i = 0; i <= nRayCount; i++)
        {
            //
            Vector3 v3Vertex;

            //
            RaycastHit2D rcRaycastHit2D = Physics2D.Raycast(m_v3Origin, GetVectorFromAngle(fAngle), m_fCurrentViewDistance, m_lmEffectLayers);

            //
            if (rcRaycastHit2D.collider == null)
            {
                //
                v3Vertex = m_v3Origin + (GetVectorFromAngle(fAngle) * m_fCurrentViewDistance);
            }

            //
            else
            {

                //
                v3Vertex = rcRaycastHit2D.point;
            }

            //
            av3Vertices[nVertexIndex] = v3Vertex;

            //
            if (i > 0)
            {
                //
                anTriangles[nTriangleIndex + 0] = 0;
                anTriangles[nTriangleIndex + 1] = nVertexIndex - 1;
                anTriangles[nTriangleIndex + 2] = nVertexIndex;

                //
                nTriangleIndex += 3;
            }

            //
            nVertexIndex++;

            //
            fAngle -= fAngleIncrease;
        }






        // Apply newly created mesh to the mesh component
        m_meshVisionCone.vertices = av3Vertices;
        m_meshVisionCone.uv = av2UV;
        m_meshVisionCone.triangles = anTriangles;

        // ensure mesh renderers outside of camera view
        m_meshVisionCone.RecalculateBounds();
    }















    private Vector3 GetVectorFromAngle(float angle)
    {
        //
        float angleRad = angle * (Mathf.PI / 180.0f);
        Vector3 newAngle = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

        //
        return newAngle;
    }

    private float GetAngleFromVector(Vector3 dir)
    {
        //
        dir = dir.normalized;

        //
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //
        if (n < 0)
            n += 360;

        //
        return n;
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0.0f;
        return vec;
    }

    public Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    public Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }

    public Vector3 GetMouseWorldPositionWithZ(Vector3 screenPos, Camera worldCamera)
    {
        Vector3 worldPos = worldCamera.ScreenToWorldPoint(screenPos);
        return worldPos;
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