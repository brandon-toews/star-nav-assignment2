using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using UnityEngine.XR;

//Class used to draw all the connections between stars in the entire galaxy that inherits from a base class for drawing star connections
public class EntireMap : GenerateStarConnections {
    
    //Create mesh for instance of this class set the scale and mesh side variables to match the UI sliders in the Game Manager
    private void Awake() {
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        scale = GameManager.starConScaleAmount;//0.3f;
        sides = GameManager.starConSideAmount;

        //Intialized the angle variable used to calculate the vertices for the meshes
        angle = 2f * Mathf.PI;
    }

    // Update is called once per frame
    void Update() => Generate();

    //Update the scale and sides variables to match what the sliders are changed to
    void Generate() {

        scale = GameManager.starConScaleAmount;//0.3f;
        sides = GameManager.starConSideAmount;

        //Run the function to draw the connections
        DrawStarConnections();
    }

    //Override for counting how many connections to draw so that the draw function doesn't draw double that connections it needs to
    public override int CountStarConnections() {

        //records each connection
        int counter = 0;

        //list of all that stars that have be investigated so that the connections aren't doubled
        List<Star> countedStars = new List<Star>();

        //Loop to go thru all stars in galaxy
        foreach (Star star in starsToConnect) {

            //loop to go thru each connected star to the selected star
            foreach (KeyValuePair<Star, float> conStar in star.connectedStars) {

                //if the selected connected star isn't already in the countedStars list then count the connection
                if (!countedStars.Contains(conStar.Key)) counter++;

            }

            //Add selected star to the countedStar list to avoid doubling the count
            countedStars.Add(star);
        }

        //return the amount of connections
        return counter;
    }

    //override to draw connections for entire map
    public override void DrawStarConnections() {

        //get all the connections and store in variable for looping and creating connections
        starConnections = CountStarConnections();

        //Initalize the amount of arrays for all the meshes needed based on how many sides and connections there are
        Vector3[] vertices = new Vector3[4 * sides * starConnections];
        Vector3[] normals = new Vector3[4 * sides * starConnections];
        Vector3 calcNorm = new Vector3();
        Vector2[] uvs = new Vector2[4 * sides * starConnections];
        int[] triangles = new int[6 * sides * starConnections];


        //set star vertice index and counter
        int StarVertIndex = 0;
        int StarVertIndexCounter = 0;

        //set star triangle index and counter
        int StarTriIndex = 0;
        int StarTriIndexCounter = 0;

        //Loop thru each star in list of galaxy
        foreach (Star star in starsToConnect) {

            //set connecting star vertice index and counter
            int conStarVertIndex = 0;
            int conStarVertIndexCounter = 0;

            //set connecting star triangle index and counter
            int conStarTriIndex = 0;
            int conStarTriIndexCounter = 0;
            
            //loop thru each connecting star to selected star
            foreach (KeyValuePair<Star, float> conStar in star.connectedStars) {

                //if the selected connecting star is found in the visitedstars list then don't draw the connection
                if (!visitedStars.Contains(conStar.Key)) {

                    //set angle variable used to calculate where the vertices for each side will be
                    float triangleAngle = angle / (float)sides;

                    //loops thru all mesh sides
                    for (int i = 0; i < sides; i++) {

                        //create and initialize vertix index for each connect to each star based on how many sides are set incremented by four because there are for vertices calculated for each mesh side
                        int vertexIndex = i * 4;

                        //Vertex index is then incremented by the star and connected star indexes
                        vertexIndex += StarVertIndex + conStarVertIndex;

                        // Define the vertices based of which side its drawing and what the scale is set to
                        vertices[vertexIndex] = new Vector3(Mathf.Sin(i * triangleAngle), Mathf.Cos(i * triangleAngle), 0) * scale;
                        vertices[vertexIndex + 1] = new Vector3(Mathf.Sin(i * triangleAngle), Mathf.Cos(i * triangleAngle), 0) * scale;

                        //if at the end of loop then create vertices that match the first set of vertices in the loop
                        if (i == sides - 1) {

                            //Same as forst set of vertices in loop
                            vertices[vertexIndex + 2] = new Vector3(Mathf.Sin(0 * triangleAngle), Mathf.Cos(0 * triangleAngle), 0) * scale;
                            vertices[vertexIndex + 3] = new Vector3(Mathf.Sin(0 * triangleAngle), Mathf.Cos(0 * triangleAngle), 0) * scale;

                        //else create end vertices that match the start ones that will be create in the next run of the loop
                        } else {
                            vertices[vertexIndex + 2] = new Vector3(Mathf.Sin((i + 1) * triangleAngle), Mathf.Cos((i + 1) * triangleAngle), 0) * scale;
                            vertices[vertexIndex + 3] = new Vector3(Mathf.Sin((i + 1) * triangleAngle), Mathf.Cos((i + 1) * triangleAngle), 0) * scale;
                        }


                        //if statement to check which direction the mesh should run so that they are visible
                        if (star.coordinates.z > conStar.Key.coordinates.z) {
                            vertices[vertexIndex] += star.coordinates.vect3;
                            vertices[vertexIndex + 1] += conStar.Key.coordinates.vect3;
                            vertices[vertexIndex + 2] += star.coordinates.vect3;
                            vertices[vertexIndex + 3] += conStar.Key.coordinates.vect3;
                        } else {
                            vertices[vertexIndex] += conStar.Key.coordinates.vect3;
                            vertices[vertexIndex + 1] += star.coordinates.vect3;
                            vertices[vertexIndex + 2] += conStar.Key.coordinates.vect3;
                            vertices[vertexIndex + 3] += star.coordinates.vect3;
                        }

                
                        //run Calculate normals function from GenerateStarConnections base class and store in variable
                        calcNorm = CalculateNormals(vertices[vertexIndex], vertices[vertexIndex + 1], vertices[vertexIndex + 2], vertices[vertexIndex + 3]);

                        // Define the normals
                        normals[vertexIndex] = -calcNorm;
                        normals[vertexIndex + 1] = -calcNorm;
                        normals[vertexIndex + 2] = -calcNorm;
                        normals[vertexIndex + 3] = -calcNorm;

                        // Define the UV coordinates
                        uvs[vertexIndex] = new Vector2(0, 1);
                        uvs[vertexIndex + 1] = new Vector2(0, 0);
                        uvs[vertexIndex + 2] = new Vector2(1, 1);
                        uvs[vertexIndex + 3] = new Vector2(1, 0);


                        //Increment triangle index
                        int triangleIndex = i * 6;

                        //increment triangle index by the star and connected star triangle index
                        triangleIndex += StarTriIndex + conStarTriIndex;

                        // Define the triangles
                        triangles[triangleIndex] = vertexIndex;
                        triangles[triangleIndex + 1] = vertexIndex + 2;
                        triangles[triangleIndex + 2] = vertexIndex + 3;
                        triangles[triangleIndex + 3] = vertexIndex;
                        triangles[triangleIndex + 4] = vertexIndex + 3;
                        triangles[triangleIndex + 5] = vertexIndex + 1;

                        //Increment counters
                        conStarVertIndexCounter += 4;
                        conStarTriIndexCounter += 6;

                        StarVertIndexCounter += 4;
                        StarTriIndexCounter += 6;
                    }

                    //add counter values to connected star indexes
                    conStarVertIndex += conStarVertIndexCounter;
                    conStarTriIndex += conStarTriIndexCounter;

                    //set counters back to zero
                    conStarVertIndexCounter = 0;
                    conStarTriIndexCounter = 0;

                }



            }

            //after drawing all connects that the selected star has add is to visited list so that connections aren't drawn twice
            visitedStars.Add(star);

            //add counter values to star indexes
            StarVertIndex += StarVertIndexCounter;
            StarTriIndex += StarTriIndexCounter;

            //set counters to zero
            StarVertIndexCounter = 0;
            StarTriIndexCounter = 0;
        }

        //clear mesh and then draw meshes with new array values
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        //clear visited stars list to start over
        visitedStars.Clear();
    }
}