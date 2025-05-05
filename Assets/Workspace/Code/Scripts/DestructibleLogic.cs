using UnityEngine;

public class DestructibleLogic : MonoBehaviour
{
    public float voxelSize = 0.5f;
    public Material cubeMaterial;

    void Start()
    {
        CreateLegoChunks();
    }

    void CreateLegoChunks()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;

        // Convert bounds to world space
        Vector3 min = transform.TransformPoint(bounds.min);
        Vector3 max = transform.TransformPoint(bounds.max);

        for (float x = min.x; x < max.x; x += voxelSize)
        {
            for (float y = min.y; y < max.y; y += voxelSize)
            {
                for (float z = min.z; z < max.z; z += voxelSize)
                {
                    Vector3 voxelCenter = new Vector3(x + voxelSize / 2, y + voxelSize / 2, z + voxelSize / 2);

                    // TEMP: skip check (you can replace with the intersection check from previous)
                    CreateVoxel(voxelCenter);
                }
            }
        }

        gameObject.SetActive(false);
    }

    void CreateVoxel(Vector3 position)
    {
        Debug.Log("Spawning cube at " + position);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = Vector3.one * voxelSize;

        // Add material to the voxel cube
        if (cubeMaterial != null)
        {
            cube.GetComponent<Renderer>().material = cubeMaterial;
        }

        // Add Rigidbody to the voxel
        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.mass = 1f; // Adjust mass as needed (this can be changed to fit your needs)
        rb.useGravity = true; // Ensures gravity is applied
    }

    // Naive inside-mesh check: raycast in one direction (if used)
    bool IsPointInsideMesh(Vector3 point)
    {
        int hitCount = 0;
        Ray ray = new Ray(point, Vector3.up);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            hitCount++;
        }

        return hitCount % 2 == 1;
    }
}
