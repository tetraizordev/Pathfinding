using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("States")]
    public bool isChoosingPath = false;
    public bool isBuilding = true;

    [Space(10)]
    [Header("Assignments")]
    private TileManager tileManager;
    private Pathfinder pathfinder;
    private Camera mainCamera;
    private GameObject interfaceObject;

    private Button buildModeButton;
    private Button pathfindingModeButton;

    private Text buildModeTitle;
    private Text pathfindingModeTitle;

    private Toggle cornerCuttingToggle;

    private float horizontalAxis;
    private float verticalAxis;

    private InputField horizontalWorldSizeInput;
    private InputField verticalWorldSizeInput;

    public Text mousePositionText;
    public Text pathUpdateTimeText;

    private Text tips2Text;


    // Do the initial assignments.
    void Awake() 
    {
        tileManager = GameObject.Find("Grid").GetComponent<TileManager>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        mainCamera = Camera.main;
        interfaceObject = GameObject.Find("Interface");

        buildModeButton = interfaceObject.transform.Find("MainPanel/BuildingButton").GetComponent<Button>();
        pathfindingModeButton = interfaceObject.transform.Find("MainPanel/PathPlacementButton").GetComponent<Button>();

        horizontalWorldSizeInput = interfaceObject.transform.Find("WorldCreationPanel/HorizontalInput").GetComponent<InputField>();
        verticalWorldSizeInput = interfaceObject.transform.Find("WorldCreationPanel/VerticalInput").GetComponent<InputField>();

        mousePositionText = interfaceObject.transform.Find("MainPanel/InfoPanel/MousePosition").GetComponent<Text>();
        pathUpdateTimeText = interfaceObject.transform.Find("MainPanel/InfoPanel/PathUpdateTime").GetComponent<Text>();

        tips2Text = interfaceObject.transform.Find("MainPanel/InfoPanel/Tips2").GetComponent<Text>();
        
    }

    // Camera movement.
    void Update() 
    {
        // Get input for Camera movement.
        horizontalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");
        
        // Normalize
        Vector2 moveVector = new Vector2(horizontalAxis, verticalAxis).normalized;

        // Get input for Camera zoom.
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        
        // Translate Camera
        mainCamera.transform.position += (Vector3)moveVector * Time.deltaTime * 10;

        if(mouseScroll != 0)
        {
            mainCamera.orthographicSize -= mouseScroll * Time.deltaTime * 1000;
        }
    }

    // Toggle between building and path choosing.
    public void ToggleBuildMode()
    {
        // Toggle between choosing path and building.
        isChoosingPath = !isChoosingPath;

        // Enable building.
        if(!isChoosingPath)
        {
            tileManager.canBuild = true;
            tileManager.isChoosingPath = false;
        }

        // Enable path choosing.
        if(isChoosingPath)
        {
            tileManager.canBuild = false;
            tileManager.isChoosingPath = true;
        } 
    }   

    // Handle the "Choose Build Mode" button.
    public void ChooseBuildMode()
    {
        buildModeButton.interactable = false;
        pathfindingModeButton.interactable = true;
        tips2Text.text = "Draw obstacles with left click, remove them with right click.";
        ToggleBuildMode();
        
    }

    // Handle the "Choose Build Mode" button.
    public void ChoosePathfindingMode()
    {
        buildModeButton.interactable = true;
        pathfindingModeButton.interactable = false;
        tips2Text.text = "Drag left click to move start position, right click to move target position.";
        ToggleBuildMode();
    }

    // Handle the "Create World" button and send the operation to TileManager.
    public void CreateWorldButton()
    {
        if(horizontalWorldSizeInput.text != "" && verticalWorldSizeInput.text != "")
        {
            tileManager.CreateGrid(new Vector2Int(int.Parse(horizontalWorldSizeInput.text), int.Parse(verticalWorldSizeInput.text)));
        } 
    }
}
