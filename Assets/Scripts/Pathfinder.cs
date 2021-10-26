using System.Collections.Generic;
using UnityEngine;

/*

USAGE:
In order to use the code, call FindPath() with a start and target node from the automatically made
nodes list -created in Start() function of TileManager script- to get a list of nodes that contains
the path closest to the target.

*/



public class Pathfinder : MonoBehaviour
{
    public Node[,,] nodes;
    public Vector2Int gridSize;

    public List<Node> FindPath(Vector2Int _startingPosition, Vector2Int _endPosisiton, int _layer, bool _checkCornerCutting)
    {
        // Set the position as nodes.
        Node startingNode = nodes[_layer, _startingPosition.x, _startingPosition.y];
        Node targetNode = nodes[_layer, _endPosisiton.x, _endPosisiton.y];

        // Start timer.
        float timer = Time.realtimeSinceStartup;

        // Final path to be returned.
        List<Node> finalPath = new List<Node>();

        // Create lists for open and closed nodes.
        List<Node> openNodes = new List<Node>();
        HashSet<Node> closedNodes = new HashSet<Node>();

        // Initialize open node list with the starting node.
        openNodes.Add(startingNode);

        // Loop until no node remains in open nodes list.
        while(openNodes.Count > 0)
        {
            // Start from the first node in open list.
            Node currentNode = openNodes[0];
            
            // Take the node with the lowest F Cost as the current. Continue from that.
            for(int i = 1; i < openNodes.Count; i++)
            {
                // TODO: CHANGE THIS WITH A BETTER SORTING ALGORITHM.
                if(openNodes[i].fCost < currentNode.fCost || openNodes[i].fCost == currentNode.fCost && openNodes[i].hCost < currentNode.hCost)
                {
                    currentNode = openNodes[i];
                }

            }
            
            // Current node has checked, remove from the open nodes list.
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            // Check if the algorithm finished.
            if(currentNode == targetNode)
            {
                // Retrace the path from end node's parent.
                finalPath = RetracePath(startingNode, targetNode);
                print("Path found at " + (Time.realtimeSinceStartup - timer)*1000 + " ms.");
            }
                
            // If algorithm not finished yet, open neigbours of current node to the open list.
            foreach(Node neighbour in FindNeighbours(currentNode, _checkCornerCutting))
            {
                // If the neighbour is not passable or is already checked, skip to the next one.
                if(!neighbour.passable || closedNodes.Contains(neighbour))
                {
                    continue;
                }

                // Calculate cost.
                int newMovementCostToNeighbour = currentNode.gCost + GetDistanceBetweenNodes(currentNode, neighbour);

                // If neighbour is closer,set it up. If neigbour is not in the open list, initialize it's values.
                if(newMovementCostToNeighbour < neighbour.gCost || !openNodes.Contains(neighbour))
                {
                    // Calculate the costs.
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistanceBetweenNodes(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    
                    // If it's not already in open list, add it.
                    if(!openNodes.Contains(neighbour))
                    {
                        openNodes.Add(neighbour);
                    }
                }
            }
        }

        return finalPath;
    }

    public List<Node> RetracePath(Node _startNode, Node _targetNode)
    {
        // Create an empty path.
        List<Node> path = new List<Node>();

        Node currentNode = _targetNode;

        // Follow the parent nodes until the start node.
        while(currentNode != _startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        
        // Reverse the path to make [0] the starting node.
        path.Reverse();

        // Return the path as a list of Nodes.
        return path;
    }

    // Get the neighbours of a node.
    public List<Node> FindNeighbours(Node _nodeToSearchAround, bool _checkCornerCutting)
    {
        List<Node> neighbours = new List<Node>();
        for(int x = -1; x < 2 ; x++)
        {
            for(int y = -1; y < 2; y++)
            {
                // Skip the node to search around.
                if(x == 0 && y == 0)
                {
                    continue;
                }

                int xToPosCheck = _nodeToSearchAround.position.x + x;
                int yToPosCheck = _nodeToSearchAround.position.y + y;

                // See if the position has a node in it.
                if(xToPosCheck >= 0 && xToPosCheck < gridSize.x && yToPosCheck >= 0 && yToPosCheck < gridSize.y)
                {
                    Node nodeToBeAdded = nodes[_nodeToSearchAround.layer, xToPosCheck, yToPosCheck];

                    if(_checkCornerCutting)
                    {
                        if(Mathf.Abs(x) + Mathf.Abs(y) == 2)
                        {
                            if(!nodes[_nodeToSearchAround.layer, _nodeToSearchAround.position.x + x, _nodeToSearchAround.position.y].passable && !nodes[_nodeToSearchAround.layer, _nodeToSearchAround.position.x, _nodeToSearchAround.position.y + y].passable)
                            {
                                continue;
                            }
                        }
                    }
                    neighbours.Add(nodeToBeAdded);
                }
            }
        }

        return neighbours;
    }

    // Find the approximate distance between the two nodes. (Diagonal cost is 14, horizontal & vertical cost is 10)
    public int GetDistanceBetweenNodes(Node nodeA, Node nodeB)
    {
        int xDistance = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        int yDistance = Mathf.Abs(nodeA.position.y - nodeB.position.y);

        if(xDistance > yDistance)
        {
            return 14*(yDistance) + 10*(xDistance-yDistance);
        }
        else
        {
            return 14*(xDistance) + 10*(yDistance-xDistance);
        }
    }
}
