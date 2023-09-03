using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.Drawing;

//Class to run the camera in the simulation and manage racasting to select stars
public class PlayerControls : MonoBehaviour
{
    //Speed of camera controls
    private const float speed = 8f;
    
    //Create var to store Raycast hit objects
    public static GameObject CurrentSelectedObject;


    //Camera movement function
    void MovePlayer()
    {
        //Get horizontal keyboard input
        float LeftRight = Input.GetAxis("Horizontal");
        //Create and store movement in Vector3 & multiply by speed and time
        Vector3 moveX = new Vector3(LeftRight, 0, 0) * Time.deltaTime * speed;
        //Translates horizontal keyboard input into sidetoside cam movement
        transform.Translate(moveX, Space.Self);


        //Get vertical keyboard input
        float ForwardBack = Input.GetAxis("Vertical");
        //Create and store movement in Vector3 & multiply by speed and time
        Vector3 moveZ = new Vector3(0, 0, ForwardBack) * Time.deltaTime * speed;
        //Translates vertical keyboard input into forward cam movement
        transform.Translate(moveZ, Space.Self);


        //Create variable to store Up & Down keyboard input
        float UpDown = 0;
        //If Q key pressed then camera moves down
        if (Input.GetKey(KeyCode.Q)) UpDown = -1;
        //If E key pressed then camera moves up
        if (Input.GetKey(KeyCode.E)) UpDown = 1;
        //Create and store movement in Vector3 & multiply by speed and time
        Vector3 moveY = new Vector3(0, UpDown, 0) * Time.deltaTime * speed;
        //Translates Q & E keyboard input into UP/DOWN cam movement
        transform.Translate(moveY, Space.Self);

        //Create variable to store Rotate left/right keyboard input
        float rotateLeftRight = 0;
        //If Z key pressed then camera looks left
        if (Input.GetKey(KeyCode.Z)) rotateLeftRight = -3;
        //If X key pressed then camera looks right
        if (Input.GetKey(KeyCode.X)) rotateLeftRight = 3;
        //Create and store movement in Vector3 & multiply by speed and time
        Vector3 moveRotY = new Vector3(0, rotateLeftRight, 0) * Time.deltaTime * speed;
        //Translates Z & X keyboard input into looking left/right cam movement
        transform.Rotate(moveRotY, Space.Self);

        //Create variable to store Rotate up/down keyboard input
        float rotateUpDown = 0;
        //If R key pressed then camera looks up
        if (Input.GetKey(KeyCode.R)) rotateUpDown = -3;
        //If F key pressed then camera looks down
        if (Input.GetKey(KeyCode.F)) rotateUpDown = 3;
        //Create and store movement in Vector3 & multiply by speed and time
        Vector3 moveRotX = new Vector3(rotateUpDown, 0, 0) * Time.deltaTime * speed;
        //Translates Z & X keyboard input into looking up/down cam movement
        transform.Rotate(moveRotX, Space.Self);
    }

    //Raycasting Function
    void Raycasting()
    {
        //Don't do raycast if mouse is overtop of UI element
       if (!EventSystem.current.currentSelectedGameObject)
       {

            //When mouse left click do raycast
            if (Input.GetMouseButtonDown(0))
            {
                //Create ray from main cam to mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Whatever is hit will be stores in "hit' var
                RaycastHit hit;

                //If something was hit then execute
                if (Physics.Raycast(ray, out hit)) {

                   
                    //If raycast hits star
                    if (hit.collider.tag == "Star") {

                        //if star was hit disable cam canvas and enable star nav canvas to show star path info
                        GameManager.Instance.CamCanvas.SetActive(false);
                        GameManager.Instance.StarNavCanvas.SetActive(true);

                        //If start star hasn't been selected yet
                        if (GameManager.findPath["start"] == null) {

                            //store hit star as start star
                            GameManager.findPath["start"] = hit.collider.GetComponent<Star>();

                            //Change star color to red to indicate to user that it has been selected
                            hit.collider.GetComponent<Renderer>().material.color = UnityEngine.Color.red;

                            //if start star has already been selected and it hasn't been clicked on again and there is no end star thats been selected yet
                        } else if (GameManager.findPath["start"] && GameManager.findPath["start"] != hit.collider.GetComponent<Star>() && GameManager.findPath["end"] == null) {

                            //store hit star as end star
                            GameManager.findPath["end"] = hit.collider.GetComponent<Star>();

                            //Change star color to red to indicate to user that it has been selected
                            hit.collider.GetComponent<Renderer>().material.color = UnityEngine.Color.red;

                            //if both an start and end star have already been selected then user is trying find a path between two different stars now. So clear previous selections and store new start star
                        } else {

                            //Change star color back to normal color to indicate to user that it is no longer selected
                            GameManager.findPath["start"].GetComponent<Renderer>().material.color = GameManager.findPath["start"].starColor;
                            //set star star to nothing
                            GameManager.findPath["start"] = null;

                            //Change star color back to normal color to indicate to user that it is no longer selected
                            GameManager.findPath["end"].GetComponent<Renderer>().material.color = GameManager.findPath["end"].starColor;
                            //set end star to nothing
                            GameManager.findPath["end"] = null;

                            //store new start star selection
                            GameManager.findPath["start"] = hit.collider.GetComponent<Star>();
                            //Change star color to red to indicate to user that it has been selected
                            hit.collider.GetComponent<Renderer>().material.color = UnityEngine.Color.red;
                        }
                    }

                  //Nothing was hit so clear selections and return unselected stars to original color
                } else { 

                    //Switch canvas' back to default
                    GameManager.Instance.CamCanvas.SetActive(true);
                    GameManager.Instance.StarNavCanvas.SetActive(false);

                    //if a start star was selected
                    if (GameManager.findPath["start"]) {
                        //Change star color back to normal color to indicate to user that it is no longer selected
                        GameManager.findPath["start"].GetComponent<Renderer>().material.color = GameManager.findPath["start"].starColor;
                        //set star star to nothing
                        GameManager.findPath["start"] = null;
                    }

                    //if a end star was selected
                    if (GameManager.findPath["end"]) {
                        //Change star color back to normal color to indicate to user that it is no longer selected
                        GameManager.findPath["end"].GetComponent<Renderer>().material.color = GameManager.findPath["end"].starColor;
                        //set end star to nothing
                        GameManager.findPath["end"] = null;
                    }
                }

            }
       }
    }

    // Update is called once per frame
    void Update()
    {
        //Calls function that takes in user input to move camera
        MovePlayer();

        //Calls function to handle racasting from camera
        Raycasting();

        // If user presses escape program ends
        if (Input.GetKey("escape")) UnityEngine.Application.Quit();//UnityEditor.EditorApplication.isPlaying = false;

    }
}
