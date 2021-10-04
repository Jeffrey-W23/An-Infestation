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
using MLAPI;

//--------------------------------------------------------------------------------------
// FieldOfView object. Inheriting from NetworkedBehaviour.
//--------------------------------------------------------------------------------------
public class FieldOfView : NetworkedBehaviour
{
    // FOV SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("FOV Settings:")]

    // public layer mask for the layers that will effect the mesh
    [LabelOverride("Effect Layers")] [Tooltip("The layers that will make an effect on the rendering of the fov.")]
    public LayerMask m_lmEffectLayers;

    // public float for size of the field of view
    [LabelOverride("Field Of View")] [Tooltip("The size (or width) of the field of view.")]
    public float m_fFOV = 80.0f;

    // public float for the distance of the mesh
    [LabelOverride("View Distance")] [Tooltip("The view distance of the field of view.")]
    public float m_fViewDistance = 15.0f;

    // public float for the inital and default view of the player camera.
    [LabelOverride("Default Camera Zoom")] [Tooltip("The default zoom of the camera on the player.")]
    public float m_fCameraSize = 8;

    // public float for the size of the camera when toggled off
    [LabelOverride("Disabled Camera Zoom")] [Tooltip("The zoom of the camera when the field of view is disabled.")]
    public float m_fToggleCameraSize = 6;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // LERP SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Lerp Settings:")]

    // public float for the fov smoothing value
    [LabelOverride("FOV Smoothing")] [Tooltip("The Smoothing value for the lerp between different distances and fields of view.")]
    public float m_fFOVSmoothing = 4;

    // public float for smoothing the camera lerp
    [LabelOverride("Camera Smoothing")] [Tooltip("The Smoothing value for the lerp between different camera sizes")]
    public float m_fCameraSmoothing = 4;

    // public float for smoothing the lerp of fov during toggle 
    [LabelOverride("Toggle Smoothing")] [Tooltip("The Smoothing value for the lerp between an enabled field of view and disabled one.")]
    public float m_fToggleSmoothing = 10;

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

    // float for current fov smoothing
    private float m_fCurrentFOVSmoothing;

    // float for mesh starting angle
    private float m_fStartingAngle;

    // float for lerp view distance storing
    private float m_fLerpViewDistance;

    // float for lerp fov storing
    private float m_fLerpFOV;
    
    // private float for the current camera size
    private float m_fCurrentCameraSize;
    
    // private float for the current camera smoothing value
    private float m_fCurrentCameraSmoothing = 0;

    // private bool for the current toggle of the fov
    private bool m_bFOVToggle = true;

    // private camera representing the main player camera
    private Camera m_cMainCamera;
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

        // set the current fov smoothing to the public value
        m_fCurrentFOVSmoothing = m_fFOVSmoothing;

        // set the starting angle to 0
        m_fStartingAngle = 0;

        // set the lerp view distance to the public value
        m_fLerpViewDistance = m_fViewDistance;

        // set the lerp fov to the public value
        m_fLerpFOV = m_fFOV;

        // Set the current camera size to the set size
        m_fCurrentCameraSize = m_fCameraSize;

        // set the current camera smoothing to default
        m_fCurrentCameraSmoothing = m_fCameraSmoothing;    
    }

    //--------------------------------------------------------------------------------------
    // LateUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void LateUpdate()
    {
        // Check if there is any changes to the current fov and view distance. if there is lerp to the new value.
        m_fCurrentFOV = Mathf.Lerp(m_fCurrentFOV, m_fLerpFOV, Time.deltaTime * m_fCurrentFOVSmoothing);
        m_fCurrentViewDistance = Mathf.Lerp(m_fCurrentViewDistance, m_fLerpViewDistance, Time.deltaTime * m_fCurrentFOVSmoothing);

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
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // Set the new camera position with a lerp
        if (m_cMainCamera != null)
            m_cMainCamera.orthographicSize = Mathf.Lerp(m_cMainCamera.orthographicSize, m_fCurrentCameraSize, Time.deltaTime * m_fCurrentCameraSmoothing);
    }

    //--------------------------------------------------------------------------------------
    // ToggleFOV: Toggle the Field Of View values from Default to Nothing.
    //
    // Param:
    //      bState: The state of the toggle, on or off.
    //--------------------------------------------------------------------------------------
    public void ToggleFOV(bool bState)
    {
        // change the toggle var to reflect request
        m_bFOVToggle = bState;

        // if the fov is toggled
        if (bState)
        {
            // set fov to default
            SetFOVDefault();
        }

        // else if the fov is not toggled
        else if (!bState)
        {
            // adjust the fov values, turning off the fov
            AdjustFOV(0, 0, m_fToggleCameraSize, m_fToggleSmoothing, m_fCurrentCameraSmoothing);
        }
    }

    //--------------------------------------------------------------------------------------
    // AdjustFOV: Adjust the Field Of View values to a custom setting.
    //
    // Param:
    //      fViewDistance: The view distance of the FOV vision.
    //      fFOV: The width of the FOV vision.
    //      fCameraSize: The zoom of the camera.
    //      fFOVSmoothing: The smoothing value for lerping the new fov settings.
    //      fCameraSmoothing: The smoothing value for lerping the new camera size.
    //--------------------------------------------------------------------------------------
    public void AdjustFOV(float fViewDistance, float fFOV, float fCameraSize, float fFOVSmoothing, float fCameraSmoothing) 
    {
        m_fLerpFOV = fFOV;
        m_fLerpViewDistance = fViewDistance;
        m_fCurrentCameraSize = fCameraSize;
        m_fCurrentFOVSmoothing = fFOVSmoothing;
        m_fCurrentCameraSmoothing = fCameraSmoothing;
    }

    //--------------------------------------------------------------------------------------
    // SetFOVDefault: Set the Field Of View back to default settings.
    //--------------------------------------------------------------------------------------
    public void SetFOVDefault()
    {
        m_fLerpViewDistance = m_fViewDistance;
        m_fLerpFOV = m_fFOV;
        m_fCurrentCameraSize = m_fCameraSize;
        m_fCurrentFOVSmoothing = m_fFOVSmoothing;
        m_fCurrentCameraSmoothing = m_fCameraSmoothing;
    }

    //--------------------------------------------------------------------------------------
    // GetToggleState: Get the current status of the Field Of View.
    //
    // Returns:
    //      bool: a bool representing if the fOV is toggled or not.
    //--------------------------------------------------------------------------------------
    public bool GetToggleState()
    {
        return m_bFOVToggle;
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
    // SetMainCamera: Function for setting the main camera of the player.
    //--------------------------------------------------------------------------------------
    public void SetMainCamera(Camera cCamera)
    {
        // set the passed in camera to main camera
        m_cMainCamera = cCamera;
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
}