using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//Star Class that hold info of each star
public class Star : MonoBehaviour {

    //Star coordinates for this instance of a star
    public IntVector3 coordinates;

    //Dictionary of all stars that this star is connected to and the distances to them
    public Dictionary<Star, float> connectedStars = new Dictionary<Star, float>();

    //Public List of connected stars to see in inspector for debugging purposes
    public List<Star> conStars = new List<Star>();

    //Store normal star color
    public Color starColor = new Color(27 / 255f, 144 / 255f, 214 / 255f);

    //Variable to hold star's matrix
    public float[,] starMatrix = new float[4,4];

    //Test matrices for testing custom matrix methods
    public float[,] matrix2d = { { 3, 8 }, { 4, 6 } };
    public float[,] matrix3d = { { 3, 8, 3 }, { 4, 6, 9 }, { 2, 5, 7 } };

   
    // Start is called before the first frame update
    void Start()
    {
        //get the star matrix using getMatrix method and store it
        starMatrix = Matrix.getMatrix(transform);

        //get determinants of stored 2d and 3d matrices with custom matrix calculation methods
        float det2d = Matrix.detMatrix2d(matrix2d);
        float det3d = Matrix.detMatrix3d(matrix3d);

        //get matrix of minors and inverse of stored 3d matrix with custom matrix calculation methods
        float[,] matrixOfMin = Matrix.matrixOfMin(matrix3d);
        float[,] invMatrix = Matrix.invertMatrix3d(matrix3d);


        //Display Determinants in log
        Debug.Log("2d Matrix Determinant is: " + det2d);
        Debug.Log("3d Matrix Determinant is: " + det3d);

        //Display Matrix of minors values in log
        for (int i = 0; i < 3; i++) {
            for (int n = 0; n < 3; n++) {
                Debug.Log("Matrix of minors is: " + matrixOfMin[i, n]);
            }
        }

        //Display Inverse matrix values in log
        for (int i = 0; i < 3; i++)
        {
            for (int n = 0; n < 3; n++)
            {
                Debug.Log("Inverse of Matrix is: " + invMatrix[i, n]);
            }
        }

        

    }

    // Update is called once per frame
    void Update() {

        //Commented out code that displays each star's matrix for testing purposes
        /*
        string starMatrixString;
        starMatrixString = transform.name + " matrix is:\n";
        for (int i = 0; i < 4; i++) {
            //Debug.Log("[");
            starMatrixString = starMatrixString + "[";
            for (int n = 0; n < 4; n++) {

                //Debug.Log(starMatrix[i, n] + ",");
                starMatrixString = starMatrixString + starMatrix[i, n] + ",";
            }
            //Debug.Log("]\n");
            starMatrixString = starMatrixString + "] ";

        }
        Debug.Log(starMatrixString);
        */
    }
}
