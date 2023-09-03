using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for custom matrix calculation methods
public class Matrix : MonoBehaviour {
    //Method for getting the Matrix of an object from the transform
    public static float[,] getMatrix(Transform objectTransform) {
        // get matrix from the Transform
        var matrix = objectTransform.localToWorldMatrix;

        //Store matrix in 4x4 array
        float[,] matrix4d = new float[4, 4];
        for (int i = 0; i < 4; i++) {
            for (int n = 0; n < 4; n++) {
                matrix4d[i, n] = matrix[i, n];
            }
        }

        //Return 4d Matrix
        return matrix4d;
    }

    //Method for finding the determinant of a 2D Matrix
    public static float detMatrix2d(float[,] matrix2d) {

        //Calculate 2d determinant and return it
        float determinant = (matrix2d[0, 0] * matrix2d[1, 1]) - (matrix2d[0, 1] * matrix2d[1, 0]);
        return determinant;
    }

    //Method for finding the determinant of a 3D Matrix
    public static float detMatrix3d(float[,] matrix3d) {

        //Split last two rows of 3D Matrix into array of 2D Matrices for the determinant calculation
        float[][,] matrix2d = {
        new float[,] { { matrix3d[1, 1], matrix3d[1, 2] }, { matrix3d[2, 1], matrix3d[2, 2] } },
        new float[,] { { matrix3d[1, 0], matrix3d[1, 2] }, { matrix3d[2, 0], matrix3d[2, 2] } },
        new float[,] { { matrix3d[1, 0], matrix3d[1, 1] }, { matrix3d[2, 0], matrix3d[2, 1] } }
        };

        //Calculate determinant using Method for finding 2D Matrix determinant
        float determinant = (matrix3d[0, 0] * detMatrix2d(matrix2d[0])) - (matrix3d[0, 1] * detMatrix2d(matrix2d[1])) + (matrix3d[0, 2] * detMatrix2d(matrix2d[2]));
        return determinant;
    }

    //Method for calculating Matrix of Minors
    public static float[,] matrixOfMin(float[,] matrix3d) {
        //Split all rows of 3D Matrix into array of 2D Matrices for the determinant calculations
        float[][,] matrix2d = {
        new float[,] { { matrix3d[1, 1], matrix3d[1, 2] }, { matrix3d[2, 1], matrix3d[2, 2] } },
        new float[,] { { matrix3d[1, 0], matrix3d[1, 2] }, { matrix3d[2, 0], matrix3d[2, 2] } },
        new float[,] { { matrix3d[1, 0], matrix3d[1, 1] }, { matrix3d[2, 0], matrix3d[2, 1] } },
        new float[,] { { matrix3d[0, 1], matrix3d[0, 2] }, { matrix3d[2, 1], matrix3d[2, 2] } },
        new float[,] { { matrix3d[0, 0], matrix3d[0, 2] }, { matrix3d[2, 0], matrix3d[2, 2] } },
        new float[,] { { matrix3d[0, 0], matrix3d[0, 1] }, { matrix3d[2, 0], matrix3d[2, 1] } },
        new float[,] { { matrix3d[0, 1], matrix3d[0, 2] }, { matrix3d[1, 1], matrix3d[1, 2] } },
        new float[,] { { matrix3d[0, 0], matrix3d[0, 2] }, { matrix3d[1, 0], matrix3d[1, 2] } },
        new float[,] { { matrix3d[0, 0], matrix3d[0, 1] }, { matrix3d[1, 0], matrix3d[1, 1] } }
        };

        //Calculate the determinant of all the submatrices to get the Matrix of Minors
        float[,] matrixMin = { { detMatrix2d(matrix2d[0]), detMatrix2d(matrix2d[1]), detMatrix2d(matrix2d[2]) }, { detMatrix2d(matrix2d[3]), detMatrix2d(matrix2d[4]), detMatrix2d(matrix2d[5]) }, { detMatrix2d(matrix2d[6]), detMatrix2d(matrix2d[7]), detMatrix2d(matrix2d[8]) } };

        //Return Matrix of Minors
        return matrixMin;
    }

    //Method for inverting a 3d Matrix
    public static float[,] invertMatrix3d(float[,] matrix3d) {
        //Store Matrix determinant
        float det = detMatrix3d(matrix3d);

        //Variable to store the inverse Matrix, Start by storing the Matrix of Minors
        float[,] invMatrix = matrixOfMin(matrix3d);

        //Initialize array to store numbers to be transposed
        float[] transNums = { 0, 0, 0, 0, 0, 0 };

        //Loop thru Matrix of Minors,
        //Since Matrix of Cofactors just alternates between +1 and -1 set CoFactor to 1 and change it after each multiplication
        //Set transpose number index to 0 to keep track of which values are to be stored for transposing later
        //Set "t" variable to 0 and loop thru for storing each transpose number in array
        for (int coFac = 1, i = 0, transInd = 0, t = 0; i < 3; i++) {

            for (int n = 0; n < 3; n++) {

                //Multiply value in Matrix by Cofactor
                invMatrix[i, n] *= coFac;

                //if row number is equal to transpose number index it is not a number that will be tranposed therefore don't store it in the transpose number array
                if (n != transInd) {
                    //store value in Matrix to transpose array
                    transNums[t] = invMatrix[i, n];
                    //increment transpose number array
                    t++;
                }

                //Change the sign of the cofactor to multiply the next value in the matrix by
                coFac *= -1;

            }
            //Increment transpose index number
            transInd++;
        }

        //Loop to transpose matrix
        for (int i = 0, transInd = 0, t = 0; i < 3; i++) {

            for (int n = 0; n < 3; n++) {
                //if column number is equal to transpose number index it is not a number that will be tranposed therefore don't switch it
                if (n != transInd) {
                    //Switch value in matrix to the transpose number
                    invMatrix[n, i] = transNums[t];
                    //increment transpose number array
                    t++;
                }

                //Divide each value in matrix by the Determinant
                invMatrix[n, i] /= det;

            }
            //Increment transpose index number
            transInd++;
        }


        //Return inverse of the given matrix
        return invMatrix;

    }

    //Method to calculate a rotation matrix taking in two Vector3s
    public static Matrix4x4 RotateMatrix(Vector3 startStar, Vector3 targetStar) {

        // Define the original transform matrix and the desired unit vector3
        Matrix4x4 transformMatrix = Matrix4x4.identity;//define actual matrix
        Vector3 unitVector3 = targetStar;

        // Normalize the vector to be a unit vector
        unitVector3 = Vector3.Normalize(unitVector3);

        // Find the angle between the two directions
        float cosAngle = Vector3.Dot(startStar, unitVector3);
        float angle = (float)Math.Acos(cosAngle);

        // Find the rotation angles around each axis
        float sinX = (transformMatrix[2, 1] * unitVector3.z) - (transformMatrix[3, 1] * unitVector3.y);
        float cosX = (float)Math.Sqrt(1 - (sinX * sinX));
        float sinY = (transformMatrix[1, 1] * unitVector3.z) - (transformMatrix[3, 1] * unitVector3.x);
        float cosY = (float)Math.Sqrt(1 - (sinY * sinY));
        float sinZ = (transformMatrix[1, 2] * unitVector3.x) - (transformMatrix[1, 1] * unitVector3.y);
        float cosZ = (float)Math.Sqrt(1 - (sinZ * sinZ));

        // Create the rotation matrices for each axis
        Matrix4x4 rotX = new Matrix4x4();
        rotX.SetRow(0, new Vector4(1, 0, 0, 0));
        rotX.SetRow(1, new Vector4(0, cosX, -sinX, 0));
        rotX.SetRow(2, new Vector4(0, sinX, cosX, 0));
        rotX.SetRow(3, new Vector4(0, 0, 0, 1));

        Matrix4x4 rotY = new Matrix4x4();
        rotY.SetRow(0, new Vector4(cosY, 0, sinY, 0));
        rotY.SetRow(1, new Vector4(0, 1, 0, 0));
        rotY.SetRow(2, new Vector4(-sinY, 0, cosY, 0));
        rotY.SetRow(3, new Vector4(0, 0, 0, 1));

        Matrix4x4 rotZ = new Matrix4x4();
        rotZ.SetRow(0, new Vector4(cosZ, -sinZ, 0, 0));
        rotZ.SetRow(1, new Vector4(sinZ, cosZ, 0, 0));
        rotZ.SetRow(2, new Vector4(0, 0, 1, 0));
        rotZ.SetRow(3, new Vector4(0, 0, 0, 1));


        // Apply the rotations to the transform matrix
        Matrix4x4 newTransformMatrix = transformMatrix * rotX * rotY * rotZ;

        //Return the calculated rotation matrix
        return newTransformMatrix;
    }
}
