using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{

    [Header("Assignments")]
    Grid mainGrid;
    Pathfinder pathfinder;
    GameManager gameManager;

    [Space(10)]
    [Header("Lists")]

    public List<Tilemap> tilemapLayers = new List<Tilemap>(); // All the layers of the tilemap renderer
    public List<TileObject> tileObjectList = new List<TileObject>(); // All the tiles and the properties of them
    public List<Node> path = new List<Node>(); // Last generated path

    [Space(10)]
    [Header("States")]
    public bool canBuild = true; // Building - Path choosing state
    public bool isChoosingPath = false;

    public bool isMousePosChanged; // Check if mouse position converted to cell position changed

    [Space(10)]
    [Header("Values")]
    public Vector2Int mouseToCellPosition = new Vector2Int(0, 0); // Mouse position converted to cell position
    Vector2Int gridSize = new Vector2Int(0, 0); // World grid size

    Vector2Int lastTilePosition;
    Vector2Int startPosition;
    Vector2Int targetPosition;

    void Awake() // Do the assignments.
    {
        // Get the GameManager component from GameManager object.
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Get the Grid component from the object. 
        mainGrid = gameObject.GetComponent<Grid>();

        // Get the Pathfinder component from the Pathfinder object.
        if(GameObject.Find("Pathfinder").GetComponent<Pathfinder>())
        {
            pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        }
        else
        {
            print("Cannot find the Pathfinder object.");
        }
    }

    void Start()
    {
        
        
        // Add all tile layers to the list.
        foreach(Tilemap tilemapObject in GetComponentsInChildren<Tilemap>()) 
        {
            if(tilemapObject != null) 
            {
                tilemapLayers.Add(tilemapObject);
            }
            else
            {
                print("No layers created in Grid.");
            }
        }
    }

    void Update()
    {
        // Get mouse position as cell position.
        mouseToCellPosition = (Vector2Int)mainGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        // Check if mouse pos changed.
        isMousePosChanged = (mouseToCellPosition != lastTilePosition);

        // If mouse pos changed, update the preview tile.
        if(isMousePosChanged)
        {
            gameManager.mousePositionText.text = "Mouse Position: " + mouseToCellPosition;

            if(!isChoosingPath)
            {
                // Set a preview tile on the current cell.
                tilemapLayers[2].SetTile((Vector3Int)mouseToCellPosition, tileObjectList[2].tile[0]);
                
                // Set the last cell as null.
                tilemapLayers[2].SetTile((Vector3Int)lastTilePosition, null);
            }
            else
            {
                tilemapLayers[2].SetTile((Vector3Int)lastTilePosition, null);
                tilemapLayers[2].SetTile((Vector3Int)mouseToCellPosition, null);
            }
        }

        // Set last position as the current position.
        lastTilePosition = mouseToCellPosition;

        // Build tiles.
        if(canBuild)
        {
            if(IsInsideOfTheGridBounds(mouseToCellPosition))
            {
                // Add a tile with left click.
                if (Input.GetMouseButton(0)) 
                {
                    AddTile(mouseToCellPosition, 2, 1);
                    UpdatePath();
                }

                // Remove a tile with right click.
                if (Input.GetMouseButton(1)) 
                {
                    RemoveTile(mouseToCellPosition, 1);
                    UpdatePath();
                }
            }
        }
        
        // If currently choosing path
        if(isChoosingPath)
        {
            // Check if mouse position actually changed, and if it is not the same, so that the code does not run every frame.
            if(isMousePosChanged && CheckTileIndex(mouseToCellPosition, 4) != 6)
            {
                // Check if it is inside the bounds of the grid.
                if(IsInsideOfTheGridBounds(mouseToCellPosition))
                {
                    // Change the position of starting node.
                    if(Input.GetMouseButton(0))
                    {
                        RemoveTile(startPosition, 4);
                        AddTile(mouseToCellPosition, 6, 4);
                        startPosition = mouseToCellPosition;
                        
                        UpdatePath();
                    }

                    // Change the position of target node.
                    if(Input.GetMouseButton(1))
                    {
                        RemoveTile(targetPosition, 4);
                        AddTile(mouseToCellPosition, 6, 4);
                        targetPosition = mouseToCellPosition;

                        UpdatePath();
                    }
                }
            }   
        }
    }

    public void AddTile(Vector2Int _cellPosition, int _tileIndex, int _layer) 
    {
        // Check if position is not out of bounds.
        if(_cellPosition.x < 0 || _cellPosition.y < 0)
        {
            // DISABLED: print("Cell position cannot be below 0. Tried to place at: " + _cellPosition);
            return;
        }

        // Check if position is exceding the max grid size.
        if(_cellPosition.x >= gridSize.x || _cellPosition.y >= gridSize.y)
        {
            // DISABLED: print("Cell position cannot be above max grid size. Tried to place at: " + _cellPosition);
            return;
        }

        // Check if there is another object
        if(_tileIndex == CheckTileIndex(_cellPosition, _layer))
        {
            // print("There is already the same tile at this position.");
            return;
        }

        // If index is 0, remove instead.
        if(_tileIndex == 0)
        {
            RemoveTile(_cellPosition, _layer);
            // DISABLED: print("Tried placing with the index of 0. Use RemoveTile() instead.");
            return;
        }

        // Place tile to the correct layer.
        // print("Layer: " + _layer + ", Cell Position: " + _cellPosition + ", Tile Index: " + _tileIndex);
        tilemapLayers[_layer].SetTile((Vector3Int)_cellPosition, tileObjectList[_tileIndex].tile[Random.Range(0, tileObjectList[_tileIndex].tile.Length)]);
        
        // Add tile data to node list.
        pathfinder.nodes[_layer, _cellPosition.x, _cellPosition.y] = new Node(_cellPosition, tileObjectList[_tileIndex].weight, tileObjectList[_tileIndex].passable, _tileIndex, _layer);
    }

    public void RemoveTile(Vector2Int _cellPosition, int _layer)
    {
        // Check if position is not out of bounds.
        if(_cellPosition.x < 0 || _cellPosition.y < 0)
        {
            //print("Cell position cannot be below 0. Tried to place at: " + _cellPosition);
            return;
        }

        // Check if position is exceding the max grid size.
        if(_cellPosition.x >= gridSize.x || _cellPosition.y >= gridSize.y)
        {
            //print("Cell position cannot be above max grid size. Tried to place at: " + _cellPosition);
            return;
        }

        // Remove the tile.
            tilemapLayers[_layer].SetTile((Vector3Int)_cellPosition, tileObjectList[0].tile[0]);
            pathfinder.nodes[_layer, _cellPosition.x, _cellPosition.y] = new Node(_cellPosition, tileObjectList[0].weight, tileObjectList[0].passable, 0, _layer);
    }

    // CREATE A STARTING GRID
    public void CreateGrid(Vector2Int _gridSize)
    {
        // GENERATE STARTING GRID
        
        // Reset tilemap.
        for(int z = 0; z < tilemapLayers.Count; z++)
        {
            for(int x = 0; x < gridSize.x; x++)
            {
               for(int y = 0; y < gridSize.y; y++)
                {
                    tilemapLayers[z].SetTile(new Vector3Int(x, y, 0), tileObjectList[0].tile[0]);
                } 
            }
        }
        
        // Set the local gridsSize variable.
        gridSize = _gridSize;

        // Create node list.
        pathfinder.nodes = new Node[tilemapLayers.Count, _gridSize.x, _gridSize.y];
        pathfinder.gridSize = _gridSize;

        // Create a rectangle with grass tiles.
        for(int x = 0; x < _gridSize.x; x++)
        {
           for(int y = 0; y < _gridSize.y; y++)
           {
               AddTile((new Vector2Int(x, y)), 1, 0);
               AddTile((new Vector2Int(x, y)), 0, 1);
               AddTile((new Vector2Int(x, y)), 0, 2);
               AddTile((new Vector2Int(x, y)), 0, 3);
               AddTile((new Vector2Int(x, y)), 0, 4);
           }
        }

        // Create start and target nodes.
        AddTile(new Vector2Int(1, 1), 6, 4);
        startPosition = new Vector2Int(1, 1);

        AddTile(new Vector2Int(_gridSize.x - 2, _gridSize.y - 2), 6, 4);
        targetPosition = new Vector2Int(_gridSize.x - 2, _gridSize.y - 2);

        // Set camera position.
        Camera.main.transform.position = new Vector3(gridSize.x / 2, gridSize.y / 2, -10);
        Camera.main.orthographicSize = gridSize.y / 2 + 2;

        // Update path.
        UpdatePath();
    }

    // UPDATE THE RENDER OF THE PATH
    public void UpdatePath()
    {
        // Check if the path is actually set.
        if(path != null)
        {
            // Remove the previous path.
            foreach(Node node in path)
            {
                RemoveTile(node.position, 3);
            }
        }

        // Update the path with the new findings.     
        path = pathfinder.FindPath(startPosition, targetPosition, 1, true);

        // Draw the path.
        foreach(Node node in path)
        {
            AddTile(node.position, 5, 3);
        }
    }

    // CHECK THE TILE INDEX AT A SPESIFIC POSITION GIVEN. RETURN THE INDEX OF THE TILE, IF A PROBLEM OCCURS, RETURN "-1"
    public int CheckTileIndex(Vector2Int _positionToCheck, int _layer)
    {



        // Check if pathfinder node list is empty;
        if(pathfinder.nodes == null)
        {
            return -1;
        }

        // Check if the position given is inside the bounds of the grid.
        if(!IsInsideOfTheGridBounds(_positionToCheck))
        {
            return -1;
        }
        if(pathfinder.nodes[_layer, _positionToCheck.x, _positionToCheck.y] != null)
        {
            if(pathfinder.nodes[_layer, _positionToCheck.x, _positionToCheck.y].tileIndex != 0 )
            {
                return pathfinder.nodes[_layer, _positionToCheck.x, _positionToCheck.y].tileIndex;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            return -1;
        }
    }

    // CHECK IF THE GIVEN POSITION IS INSIDE THE BOUNDARIES OF THE CURRENT GRID.
    public bool IsInsideOfTheGridBounds(Vector2Int _positionToCheck)
    {
        return (_positionToCheck.x >= 0 && _positionToCheck.y >= 0 && _positionToCheck.x < gridSize.x && _positionToCheck.y < gridSize.y);
    }
}
