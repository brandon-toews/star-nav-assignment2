using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Custom Vector3 struct for holding star coordinates
[System.Serializable]
public struct IntVector3 {

	//create x, y, z variables to hold coordinates
	public int x, y, z;

	//create a vector3 variable to hold coordinates so that calculations can made easily in the connection drawing classes
	public Vector3 vect3;

	//initialize variables when creating instance
	public IntVector3(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
        this.vect3 = new Vector3(x, y, z);
    }

	//operater for quickly adding one IntVector3 to another
    public static IntVector3 operator +(IntVector3 a, IntVector3 b) {
		a.x += b.x;
		a.y += b.y;
		a.z += b.z;
		return a;
	}

}