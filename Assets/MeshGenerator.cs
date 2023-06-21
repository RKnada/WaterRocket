using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public int numberOfPoints = 10;
    public float radius = 1f;

    private void Start()
    {
        Vector3[] points = new Vector3[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfPoints;
            points[i] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = points;
        int[] triangles = new int[(numberOfPoints - 2) * 3];
        for (int i = 0; i < numberOfPoints - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        mesh.triangles = triangles;
    }
}
