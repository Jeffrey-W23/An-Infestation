//--------------------------------------------------------------------------------------
// Purpose: 
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
    //
    [SerializeField] private LayerMask layermask;

    //
    private Mesh mesh;

    //
    public Vector3 origin;

    //
    private float startingAngle;

    //
    private float fov;

    //--------------------------------------------------------------------------------------
    // initialization
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // Create new mesh component
        mesh = new Mesh();

        // Set the mesh filter to the newly created mesh
        GetComponent<MeshFilter>().mesh = mesh;

        //
        origin = Vector3.zero;

        //
        fov = 80.0f;
    }


    private void LateUpdate()
    {        
        //
        int rayCount = 50;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        float viewDistance = 15.0f;

        // Setup the new mesh
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] tris = new int[rayCount * 3];

        //
        vertices[0] = origin;

        //
        int vertexIndex = 1;
        int triangleIndex = 0;

        //
        for (int i = 0; i <= rayCount; i++)
        {
            //
            Vector3 vertex;

            //
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layermask);

            //
            if (raycastHit2D.collider == null)
            {
                //
                vertex = origin + (GetVectorFromAngle(angle) * viewDistance);
            }

            //
            else
            {

                //
                vertex = raycastHit2D.point;
            }

            //
            vertices[vertexIndex] = vertex;

            //
            if (i > 0)
            {
                //
                tris[triangleIndex + 0] = 0;
                tris[triangleIndex + 1] = vertexIndex - 1;
                tris[triangleIndex + 2] = vertexIndex;

                //
                triangleIndex += 3;
            }

            //
            vertexIndex++;

            //
            angle -= angleIncrease;
        }
 
        // Apply newly created mesh to the mesh component
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = tris;
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

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVector(aimDirection) + fov / 2.0f;
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
}