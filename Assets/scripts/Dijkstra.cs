using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

//Dijkstra pathfinding algorithm class
public class Dijkstra : MonoBehaviour {

    //Create list that algorithm adds the stars of the calculated path to and returns
    //public static List<Star> currentPath = new List<Star>();

    //Method to find shortest path between two stars.. Takes in the list of stars in galaxy,
    //start star and end star as parameters
    public static List<Star> FindShortestPath(List<Star> stars, Star startStar, Star endStar) {

        //Dictionary to keep track of the distance it takes to get to each star from the start
        Dictionary<Star, float> distance = new Dictionary<Star, float>();

        //Dictrionary to track what was the previous star visited
        Dictionary<Star, Star> previous = new Dictionary<Star, Star>();

        //List of unvisited stars
        List<Star> unvisitedStars = new List<Star>();


        //Initialize the dictionaries and lists
        foreach (Star star in stars) {
            distance[star] = float.PositiveInfinity;
            previous[star] = null;
            unvisitedStars.Add(star);
        }

        //set distance to start star as zero
        distance[startStar] = 0;

        //Loop that continues until all stars have been visited
        while (unvisitedStars.Count > 0) {

            //Use a lambda expression to grab the star with the lowest distance in the unvisited
            //stars list which ensures that the start star is selected
            Star currentStar = unvisitedStars.OrderBy(s => distance[s]).First();
            

            //if (currentStar == endStar) {
            //    break;
            //}

            //takes the star out of the unvisited list
            unvisitedStars.Remove(currentStar);

            //Loop through each connected star to the current selected star
            foreach (Star neighborStar in currentStar.connectedStars.Keys) {

                //Saves the distance of the current star with the selected connected star
                float tentativeDistance = distance[currentStar] + currentStar.connectedStars[neighborStar];

                //If the calculated distance from the current star to the selected connecting star is shorter than what is currently recorded in the distances dictionary then update dictionary
                if (tentativeDistance < distance[neighborStar]) {
                    distance[neighborStar] = tentativeDistance;

                    //set the previous star of the connected star to the current star if its the shortest connection
                    previous[neighborStar] = currentStar;
                }
            }
        }

        //create list to store the shorest path
        List<Star> shortestPath = new List<Star>();

        //set a car that can be changed to the end star
        Star tempStar = endStar;

        //loop that goes through the dictionary of previous stars starting with the end star until it reached the start star
        while (tempStar != null) {

            //add each star in the previous star dictionary to the shortest path list. Will only add end star to list if there is no path from start to end because there will be no previous star for it in the previous star dictionary.
            shortestPath.Add(tempStar);

            //Keep grabbing the next previous star to the last one. If there is no path from the start star to the end star then there won't be a previous star to the end star
            tempStar = previous[tempStar];
        }

        //reverse the order so that it starts with the start star
        shortestPath.Reverse();

        //Saves the shortest path to this class instance so that it can be referenced
        //currentPath = shortestPath;

        //returns a list of the shortest path
        return shortestPath;
    }
}



