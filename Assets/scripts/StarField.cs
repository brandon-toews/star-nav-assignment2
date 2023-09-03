using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

//StarField Class for generating a galaxy of stars
public class StarField : MonoBehaviour {

    //For storing Star prefab
    public Star starPrefab;

    //3D array for storing the stars in the array at there specific coordinates
    private Star[,,] starLocations;

    //list to store all the stars that have been generated
    public static List<Star> activeStars;

    //Public variable that can be set in the inspector which specifies the physical space that the stars will be generated in
    public IntVector3 size;

    //Public variable that can be set in the inspector which specifies how many seconds between each star being generated
    public float generationStepDelay;

    //Public variable that can be set in the inspector which specifies how many stars will be generated
    public int numOfStars;

    //how many star connections there are in the galaxy
    public static int starConnections = 0;

    private void Awake() {
        //Initialize star location array to the maximum size of the galaxy
        starLocations = new Star[size.x, size.y, size.z];

        //initialize star list
        activeStars = new List<Star>();
    }

    //Generate method for generating stars in the galaxy
    public IEnumerator Generate() {
        //Seconds delay between star creations
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay);

        //Generate first star and store in star list
        DoFirstGenerationStep(activeStars);

        //loop thru how many stars are supposed to be generated
        while (numOfStars > 0) {

            //Wait for the delay in between star creations
            yield return delay;

            //Create next star and store in star list
            DoNextGenerationStep(activeStars);
            //Deincrement as each star is created
            numOfStars--;
        }

        //Run method to connect the newly generated stars
        connectStars();

        //set instance that draws star connections with the list of generated stars
        GameManager.Instance.myEntireMapInstance.starsToConnect = activeStars;
        
    }

    //Method to connect stars
    private void connectStars() {

        //loop thru each star in the galaxy
        foreach (Star star in activeStars) {

            //Variables to hold the next closest star and distance just in case there are no stars within range to connect to
            float nextClosestDistance = float.MaxValue;
            Star nextClosestStar = null;

            //loop thru all of the other stars in the galaxy
            foreach (Star otherStar in activeStars) {
                //store calculated distance between two stars to use later to decide if stars are close enough to connect
                float distance = calculateStarDistance(star.coordinates, otherStar.coordinates);

                //If the other star from the list isn't the first star and the distance is closer than the currently recorded closest star
                if (otherStar != star && distance < nextClosestDistance) {

                    //store new next closest star and distance
                    nextClosestDistance = distance;
                    nextClosestStar = otherStar;
                }


                //If the other star from the list isn't the first star and the distance is within 20 then connect stars
                if (otherStar != star && distance < 20) {
                    //Add the other star to the first star's dictionary with the distance
                    star.connectedStars.Add(otherStar, distance);
                    //Add the other star to the first star's list so we can see what stars its connected to in the inspector
                    star.conStars.Add(otherStar);

                    //count the connection
                    starConnections++;
                    
                    //print connection to console for debugging
                    UnityEngine.Debug.Log("Star " + star.name + " is connected to " + otherStar.name);
                }
            }

            //If the star isn't close enough to any other star to be connected then connect it to its next closest star
            if (star.connectedStars.Count == 0) {

                //Add the next closest star to the first star's dictionary with the distance
                star.connectedStars.Add(nextClosestStar, nextClosestDistance);

                // Add the first star to the next closest star dictionary with the distance
                nextClosestStar.connectedStars.Add(star, nextClosestDistance);

                //Add the next closest star to the first star's list so we can see what stars its connected to in the inspector
                star.conStars.Add(nextClosestStar);

                //Add the first star to the next closest star's list so we can see what stars its connected to in the inspector
                nextClosestStar.conStars.Add(star);
            }
        }
    }

    //method for calculating distance between two stars taking in their IntVector3 coordinates
    private float calculateStarDistance(IntVector3 a, IntVector3 b) {
        float distance = (float)Math.Sqrt(Math.Pow(b.x - a.x, 2) + Math.Pow(b.y - a.y, 2) + Math.Pow(b.z - a.z, 2));
        return distance;
    }

    //Method for creating the first star in the galaxy taking in list to add created star to
    private void DoFirstGenerationStep(List<Star> activeStars) {
        //instantiate star using Create star method with random coordinates within the range set
        activeStars.Add(CreateStar(RandomCoordinates));
    }

    //Method for creating the rest of the stars in the galaxy taking in list to add created star to
    private void DoNextGenerationStep(List<Star> activeStars) {
        //save random coordinates to initialize newly created star with
        IntVector3 coordinates = RandomCoordinates;

        //If coordinates are within the set range and there isn't a star that is already at that location
        if (ContainsCoordinates(coordinates) && GetStar(coordinates) == null) {
            //Create and add star to list of stars in galaxy
            activeStars.Add(CreateStar(coordinates));
        }
    }

    //Method for creating a star
    private Star CreateStar(IntVector3 coordinates) {
        //Instantiate star with saved star prefab
        Star newStar = Instantiate(starPrefab) as Star;

        //Store newly created star in the star location array at the specified coordinates
        starLocations[coordinates.x, coordinates.y, coordinates.z] = newStar;

        //Set star's coordinates
        newStar.coordinates = coordinates;
        //name star after it's coordinates
        newStar.name = "Star " + coordinates.x + ", " + coordinates.y + ", " + coordinates.z;
        //set the star's parent transform to this Starfield instance's transfomrm
        newStar.transform.parent = transform;
        //set stars transform position to the coordinates
        newStar.transform.localPosition = new Vector3(coordinates.x, coordinates.y, coordinates.z);
        //return the new star to be added to the list of stars in the galaxy
        return newStar;
    }

    //method to check if a star already exists in a certain location
    public Star GetStar(IntVector3 coordinates) {
        //return the star if it exists
        return starLocations[coordinates.x, coordinates.y, coordinates.z];
    }

    //method to generate random coordinates
    public IntVector3 RandomCoordinates {
        get {
            //return coordinates withing the range set in the inspector
            return new IntVector3(UnityEngine.Random.Range(0, size.x), UnityEngine.Random.Range(0, size.y), UnityEngine.Random.Range(0, size.z));
        }
    }

    //Method to check that coordinates are within the set ranges
    public bool ContainsCoordinates(IntVector3 coordinate) {
        return coordinate.x >= 0 && coordinate.x < size.x && coordinate.y >= 0 && coordinate.y < size.y && coordinate.z >= 0 && coordinate.z < size.z;
    }
}