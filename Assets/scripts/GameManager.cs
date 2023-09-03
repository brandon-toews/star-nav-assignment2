using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;


//Class to manage UI elements and run the simulation
public class GameManager : MonoBehaviour {

    //Canvas to remind user of the cam controls and to allow the user to alter the scale and side of the mesh connections between the stars in the galaxy
    [SerializeField] public GameObject CamCanvas;

    //Canvas used when selecting stars to navigate from and too. Shows what stars have been selected on the right and on the left shows the path and distances to each star from the start
    [SerializeField] public GameObject StarNavCanvas;

    //Shows user what scale and sides are set to
    [SerializeField] private TextMeshProUGUI starConScaleAmountText;
    [SerializeField] private TextMeshProUGUI starConSideAmountText;

    //Sliders used by user to set scale and sides
    [SerializeField] private UnityEngine.UI.Slider starConScaleSlider;
    [SerializeField] private UnityEngine.UI.Slider starConSideSlider;

    //text to show what stars have been selected and what the suggested path is
    public TextMeshProUGUI startStarText;
    public TextMeshProUGUI endStarText;
    public TextMeshProUGUI suggestedRouteText;

    //Slider variables amounts
    public static float starConScaleAmount = 0.3f;
    public static int starConSideAmount = 4;

    //Instance of class
    public static GameManager Instance;

    //Starfield prefab
    public StarField starFieldPrefab;

    //Starfield Instance
    public StarField starFieldInstance;

    //prefab of EntireMap class to draw connections of the galaxy
    public EntireMap entireMapPrefab;

    //EntireMap Instance
    public EntireMap myEntireMapInstance;

    //dictionary that holds the start star and end star to navigate
    public static Dictionary<string, Star> findPath = new Dictionary<string, Star>();

    //List to save the current suggested route of the selected stars
    public static List<Star> suggestedRoute = new List<Star>();

    //prefab of SuggestedRoute class to draw connections of the route
    public SuggestedRoute drawSuggestedRoutePrefab;

    //SuggestedRoute Instance
    public SuggestedRoute drawSuggestedRouteInstance;

    // Start is called before the first frame update
    private void Start() {

        //initialize instance
        Instance = this;

        //Run function to instantiate starfield, begin its star generation method, and instantiate connection drawing instances
        BeginGame();
    }

    private void BeginGame() {

        //turn cam canvas on to start
        CamCanvas.SetActive(true);

        //turn star nav canvas off to start until a star is selected
        StarNavCanvas.SetActive(false);

        //initialize by instantiating prefab
        myEntireMapInstance = Instantiate(entireMapPrefab) as EntireMap;

        //initialize by instantiating prefab
        starFieldInstance = Instantiate(starFieldPrefab) as StarField;

        //Prompt Starfield instance to generate stars in galaxy
        StartCoroutine(starFieldInstance.Generate());

        //convert scale and side amounts to strings to display to user in UI
        starConScaleAmountText.text = starConScaleAmount.ToString();
        starConSideAmountText.text = starConSideAmount.ToString();

        //add and set start and end star elements to null to start
        findPath.Add("start", null);
        findPath.Add("end", null);

        //initialize by instantiating prefab
        drawSuggestedRouteInstance = Instantiate(drawSuggestedRoutePrefab) as SuggestedRoute;
    }

    // Update is called once per frame
    private void Update() {

        //If space bar is pressed run restart method
        if (Input.GetKeyDown(KeyCode.Space)) RestartGame();

        //If an end star has been selected then run pathfinding and draw suggested path
        if (findPath["end"]) {

            //set the starsToConnect list in suggested route instance to whatever list that Dijkstra returns so that it will draw the route
            drawSuggestedRouteInstance.starsToConnect = Dijkstra.FindShortestPath(StarField.activeStars, findPath["start"], findPath["end"]);

            //Save that route in GameManagers suggested route list to display the route and distances
            suggestedRoute = drawSuggestedRouteInstance.starsToConnect;

            //Run display route method to return a string that is used by UI to display the star route and distances to user
            suggestedRouteText.text = DisplayStarRoute();

        } else {

            //If there is no end star selected then clear the starsToConnect list in suggested route instance so that the previous routeis no longer drawn 
            drawSuggestedRouteInstance.starsToConnect.Clear();

            //set set displayed route to nothing
            suggestedRouteText.text = "";
        }

        //run method to update the UI text field to display which stars are selected by user
        UpdateStarSelections();
        
    }

    //method to restart simulation with new configuration of stars
    private void RestartGame() {

        //Stop everything and destory all created instances
        StopAllCoroutines();
        Destroy(starFieldInstance.gameObject);
        Destroy(myEntireMapInstance.gameObject);
        Destroy(drawSuggestedRouteInstance.gameObject);

        //reset scale and side slider values back to default
        GameManager.Instance.starConScaleSlider.value = 0.3f;
        GameManager.Instance.starConSideSlider.value = 4;

        //clear find path list so that BeginGame method can reinitialize it
        findPath.Clear();

        //Run function to instantiate starfield, begin its star generation method, and instantiate connection drawing instances
        BeginGame();
    }

    //Method to update the scale and sides slider amounts to reflect what the user has choosen with the sliders
    public static void UpdateSliders() {

        //If cam canvas is on then only the cam sliders need to be updated with corresponding UI element
        if (GameManager.Instance.CamCanvas.activeSelf == true) {

            //set Scale amount to value set on slider
            starConScaleAmount = GameManager.Instance.starConScaleSlider.value;

            //Display Scale slider value in text box so that user knows what it is
            GameManager.Instance.starConScaleAmountText.text = starConScaleAmount.ToString();

            //set Side amount to value set on slider
            starConSideAmount = (int)GameManager.Instance.starConSideSlider.value;

            //Display Side slider value in text box so that user knows what it is
            GameManager.Instance.starConSideAmountText.text = starConSideAmount.ToString();
        }
    }

    //method to update the UI to show what stars the user has selected
    public static void UpdateStarSelections() {

        //if there is a start star selected then set UI text field to the name of that star
        if (findPath["start"]) Instance.startStarText.text = findPath["start"].name;
        //if not then set UI text field to nothing
        else Instance.startStarText.text = "";

        //if there is a end star selected then set UI text field to the name of that star
        if (findPath["end"]) Instance.endStarText.text = findPath["end"].name;
        //if not then set UI text field to nothing
        else Instance.endStarText.text = "";
    }

    //method to return string of suggested star route and distances to be displayed in UI
    public static string DisplayStarRoute() {

        //create string variable to return later
        string starRoute = "";

        //create distance variable to keep track of distances and add to string
        float distance = 0;

        //If a route has been calculated then
        if (suggestedRoute.Count > 1) {

            //loop thru each star in route list
            for (int i = 0; i <= suggestedRoute.Count - 1; i++) {

                //add name of star and distance to that star in the string
                starRoute = starRoute + suggestedRoute[i].name + " = " + distance + "\n";

                //if we haven't reached last star in list then add the distance to the next star
                if (i < suggestedRoute.Count - 1) distance = distance + MathF.Round(suggestedRoute[i].connectedStars[suggestedRoute[i + 1]], 2);
            }

            //After loop has finnished close string with the final distance from start to end star
            starRoute = starRoute + "Total Distance = " + distance;

            //If there is no route suggested than tell user
        } else starRoute = "No route from:\n" + findPath["start"].name + "\nTo\n" + suggestedRoute[0].name;


        //return string of route and distance or string notifying user that a route from the selected start star and end star is not possible
        return starRoute;
    }
}