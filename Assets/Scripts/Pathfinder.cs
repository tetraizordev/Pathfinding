using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public Node[,,] nodes;
    public Vector2Int gridSize;

    private void Start() 
    {
        FindPath(new Vector2Int(2,2), new Vector2Int(4,4), 1);
    }

    public List<Node> FindPath(Vector2Int _startingPosition, Vector2Int _endPosisiton, int _layer)
    {
        // Set the position as nodes.
        Node startingNode = nodes[_layer, _startingPosition.x, _startingPosition.y];
        Node targetNode = nodes[_layer, _endPosisiton.x, _endPosisiton.y];

        // Final path to be returned.
        List<Node> finalPath = new List<Node>();

        // Create lists for open and closed nodes.
        List<Node> openNodes = new List<Node>();
        HashSet<Node> closedNodes = new HashSet<Node>();

        openNodes.Add(startingNode);

        while(openNodes.Count > 0)
        {
            Node currentNode = openNodes[0];
            
            for(int i = 1; i < openNodes.Count; i++)
            {
                if(openNodes[i].fCost < currentNode.fCost || openNodes[i].fCost == currentNode.fCost && openNodes[i].hCost < currentNode.hCost)
                {
                    currentNode = openNodes[i];
                }

            }
                
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            if(currentNode == targetNode)
            {
                print("Target found.");
                finalPath = RetracePath(startingNode, targetNode);
            }
                
            foreach(Node neighbour in FindNeighbours(currentNode))
            {
                if(!neighbour.passable || closedNodes.Contains(neighbour))
                {
                    // print(neighbour.passable);
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistanceBetweenNodes(currentNode, neighbour);

                if(newMovementCostToNeighbour < neighbour.gCost || !openNodes.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistanceBetweenNodes(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    
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
        print("Retracing path.");

        List<Node> path = new List<Node>();
        Node currentNode = _targetNode;

        while(currentNode != _startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        
        path.Reverse();

        foreach(Node node in path)
        {
            print(node.position);
        }

        return path;
    }

    public List<Node> FindNeighbours(Node _nodeToSearchAround)
    {
        List<Node> neighbours = new List<Node>();
        for(int x = -1; x < 2 ; x++)
        {
            for(int y = -1; y < 2; y++)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }

                int xToPosCheck = _nodeToSearchAround.position.x + x;
                int yToPosCheck = _nodeToSearchAround.position.y + y;

                if(xToPosCheck >= 0 && xToPosCheck < gridSize.x && yToPosCheck >= 0 && yToPosCheck < gridSize.y)
                {
                    neighbours.Add(nodes[_nodeToSearchAround.layer, xToPosCheck, yToPosCheck]);
                }
            }
        }

        return neighbours;
    }

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
