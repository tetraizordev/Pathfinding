using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isChoosingPath; // Either choose path or build.

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

    public bool isBuilding;

    void Awake()
    {
        tileManager = GameObject.Find("Grid").GetComponent<TileManager>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        mainCamera = Camera.main;
        interfaceObject = GameObject.Find("Interface");

        buildModeButton = interfaceObject.transform.Find("MainPanel/BuildingButton").GetComponent<Button>();
        pathfindingModeButton = interfaceObject.transform.Find("MainPanel/PathPlacementButton").GetComponent<Button>();


    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            // Toggle between chosing path and building.
            isChoosingPath = !isChoosingPath;

            // Enable building.
            if(!isChoosingPath)
            {
                tileManager.canBuild = true;
                tileManager.isChoosingPath = false;

                print("Toggled to building.");
            }

            // Enable path choosing.
            if(isChoosingPath)
            {
                tileManager.canBuild = false;
                tileManager.isChoosingPath = true;

                print("Toggled to path choosing.");
            } 

        }

        horizontalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");
        Vector2 moveVector = new Vector2(horizontalAxis, verticalAxis).normalized;

        mainCamera.transform.position += (Vector3)moveVector * Time.deltaTime * 10;

        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        if(mouseScroll != 0)
        {
            mainCamera.orthographicSize -= mouseScroll * Time.deltaTime * 1000;
        }
    }

    void InterfaceManagement()
    {
        if(isBuilding)
        {

        }


    }

    public void ChooseBuildMode()
    {
        buildModeButton.interactable = false;
        pathfindingModeButton.interactable = true;
        
    }

    public void ChoosePathfindingMode()
    {
        buildModeButton.interactable = true;
        pathfindingModeButton.interactable = false;
    }

}
