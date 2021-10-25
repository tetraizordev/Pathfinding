using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{

    [Header("Assignments")]
    Grid mainGrid;
    Pathfinder pathfinder;

    [Space(10)]
    [Header("Lists")]

    public List<Tilemap> tilemapLayers = new List<Tilemap>();

    public List<TileObject> tileObjectList = new List<TileObject>();

    [Space(10)]
    [Header("States")]
    public bool canBuild = true;
    public bool isChoosingPath;

    [Space(10)]
    [Header("Values")]

    public Vector3Int mouseToCellPosition = new Vector3Int(0, 0, 0);

    Vector3Int lastTilePosition;

    public Vector2Int gridSize = new Vector2Int(24, 24);

    int tileChosenIndex = 2;

    void Start()
    {
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

        
        // GENERATE STARTING GRID

        // Create node list.
        pathfinder.nodes = new Node[tilemapLayers.Count, gridSize.x, gridSize.y];
        pathfinder.gridSize = gridSize;

        // Create a rectangle with grass tiles.
        for(int x = 0; x < gridSize.x; x++)
        {
           for(int y = 0; y < gridSize.y; y++)
           {
               AddTile((new Vector2Int(x, y)), 1, 0);
               AddTile((new Vector2Int(x, y)), 0, 1);
               AddTile((new Vector2Int(x, y)), 0, 2);
               AddTile((new Vector2Int(x, y)), 0, 3);
           }
        }
        
    }

    void Update()
    {
        // Get mouse position as cell position.
        mouseToCellPosition = mainGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        // If cell position has changed, update the preview tile.
        if (mouseToCellPosition != lastTilePosition) 
        {
            if(canBuild)
            {
                // Set a preview tile on the current cell.
                tilemapLayers[2].SetTile(mouseToCellPosition, tileObjectList[tileChosenIndex].tile[Random.Range(0, tileObjectList[tileChosenIndex].tile.Length)]);
            
                // Set the last cell as null.
                tilemapLayers[2].SetTile(lastTilePosition, null);
            }
            else
            {
                tilemapLayers[2].SetTile(lastTilePosition, null);
                tilemapLayers[2].SetTile(mouseToCellPosition, null);
            }
        }

        // Set last position as the current position.
        lastTilePosition = mouseToCellPosition;
        
        if(canBuild)
        {
            // Add a tile with left click.
            if (Input.GetMouseButton(0)) 
            {
                AddTile((Vector2Int)mouseToCellPosition, 2, 1);
            }

            // Remove a tile with right click.
            if (Input.GetMouseButton(1)) 
            {
                RemoveTile((Vector2Int)mouseToCellPosition, 1);
            }

        }

        if(isChoosingPath)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                foreach(Node node in pathfinder.FindPath(new Vector2Int(1,1), new Vector2Int(gridSize.x - 1, gridSize.y - 1), 1))
                {
                    print(node.position);
                    AddTile(node.position, 5, 3);
                }
            }

            if(Input.GetKeyDown(KeyCode.G))
            {
                foreach(Node node in pathfinder.FindNeighbours(pathfinder.nodes[1, mouseToCellPosition.x, mouseToCellPosition.y]))
                {
                    print(node.position);
                }
            }
        }

        // Check the index of the mouse position.
        if(Input.GetKeyDown(KeyCode.Space))
        {
            print(CheckTileIndex((Vector2Int)mouseToCellPosition, 1));
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
        print("Layer: " + _layer + ", Cell Position: " + _cellPosition + ", Tile Index: " + _tileIndex);
        tilemapLayers[_layer].SetTile((Vector3Int)_cellPosition, tileObjectList[_tileIndex].tile[Random.Range(0, tileObjectList[_tileIndex].tile.Length)]);
        
        // Add tile data to node list.
        pathfinder.nodes[_layer, _cellPosition.x, _cellPosition.y] = new Node(_cellPosition, tileObjectList[_tileIndex].weight, tileObjectList[_tileIndex].passable, _tileIndex, _layer);
    }

    public void RemoveTile(Vector2Int _cellPosition, int _layer)
    {
        // Check if position is not out of bounds.
        if(_cellPosition.x < 0 || _cellPosition.y < 0)
        {
            print("Cell position cannot be below 0. Tried to place at: " + _cellPosition);
            return;
        }

        // Check if position is exceding the max grid size.
        if(_cellPosition.x >= gridSize.x || _cellPosition.y >= gridSize.y)
        {
            print("Cell position cannot be above max grid size. Tried to place at: " + _cellPosition);
            return;
        }

        // Remove the tile.
            tilemapLayers[_layer].SetTile((Vector3Int)_cellPosition, tileObjectList[0].tile[0]);
            pathfinder.nodes[_layer, _cellPosition.x, _cellPosition.y] = new Node(_cellPosition, tileObjectList[0].weight, tileObjectList[0].passable, 0, _layer);
    }

    // TODO: Comment here.
    public int CheckTileIndex(Vector2Int _positionToCheck, int _layer)
    {
        if(_positionToCheck.x < 0 || _positionToCheck.y < 0)
        {
            // DISABLED: print("Out of bounds at " + _positionToCheck + ". Returning -1.");
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
            // DISABLED: print("Tile is null at " + _positionToCheck + ". Returning -1.");
            return -1;
        }
    }
}
