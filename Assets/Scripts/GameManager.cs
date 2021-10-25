using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isChoosingPath; // Either choose path or build.

    private TileManager tileManager;
    private Pathfinder pathfinder;

    void Awake()
    {
        tileManager = GameObject.Find("Grid").GetComponent<TileManager>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();

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
    }

}
