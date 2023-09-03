using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]

//Base Class that EntireMap and SuggestedRoute inherits from
public class GenerateStarConnections : MonoBehaviour { 

    //Create mesh for instance
    public Mesh mesh;

    //create star connections counter
    public int starConnections;

    //Create starsToConnect list
    public List<Star> starsToConnect = new List<Star>();

    //Create visited stars list
    public List<Star> visitedStars = new List<Star>();


    //Create Scale variable
    public float scale = 0.3f;

    //create side variable
    public int sides = 4;

    //create angle variable
    public float angle;

    //empty method to be overridden
    public virtual int CountStarConnections() {
        return 0;
    }

    //empty method to be overridden
    public virtual void DrawStarConnections() {}

    //Method to calculate normals, takes in the vertices
    public Vector3 CalculateNormals(Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 vert4) {

        //create and store two vectors from the supplied vertices
        Vector3 vector1 = vert2 - vert1;
        Vector3 vector2 = vert3 - vert1;

        //Initialize vector3 variable to store cross product
        Vector3 crossProduct = new Vector3();

        //calculate and store cross product of two vectors
        crossProduct.x = vector1.y * vector2.z - vector1.z * vector2.y;
        crossProduct.y = vector1.z * vector2.x - vector1.x * vector2.z;
        crossProduct.z = vector1.x * vector2.y - vector1.y * vector2.x;

        //return the normalized cross product for the normals
        return crossProduct.normalized;

    }

    //method for calculating magnitudes,
    float CalcMag(Vector3 vec) {
        return Mathf.Sqrt((vec.x * vec.x) + (vec.y * vec.y) + (vec.z * vec.z));
    }

    //method for calculating dot product of two vector3s
    float CalcDot(Vector3 a, Vector3 b) {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    //for debugging, got from internet, just copy and pasted

    //method to draw normals thru gizmos for debugging in scene view
    void OnDrawGizmos() {

        //retrieve and store mesh filter
        MeshFilter filter = GetComponent<MeshFilter>();

        //if there is a mesh filter
        if (filter) {

            //store shared mesh in mesh variable to feed into other method
            Mesh mesh = filter.sharedMesh;
            if (mesh) {

                //pass mesh into method to take values for drawing normals
                ShowTangentSpace(mesh);
            }
        }
    }

    //method to create mesh 
    void ShowTangentSpace(Mesh mesh) {

        //create arrays, store vertices and normals from mesh
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        //loop thru arracy of vertices
        for (int i = 0; i < vertices.Length; i++) {

            //call override method to draw gizmos
            ShowTangentSpace(
            transform.TransformPoint(vertices[i]),
            transform.TransformDirection(normals[i])
            );
        }
    }

    //method to draw each normal of each vertice
    void ShowTangentSpace(Vector3 vertex, Vector3 normal) {
        Gizmos.color = UnityEngine.Color.green;
        Gizmos.DrawLine(vertex, vertex + normal);
    }
}