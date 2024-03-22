using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathfindingDOTS : MonoBehaviour
{
    //Le grid utiliser pour le pathfinding
    NodeGrid grid;

    //Les cout de mouvements de case
    private const int MOVE_HORIZONTAL = 10;
    private const int MOVE_DIAGONAL = 14;

    private void Awake()
    {
        grid = GetComponent<NodeGrid>();
    }

    //Methode pour creer un path
    public void FindPath(PathRequest request, Action<PathResult> callback, bool isFlying)
    {
        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;

        //On definit les points de depart et d'arriver
        PathNode startNode = grid.NodeFromWorldPoint(request.pathStart);
        PathNode targetNode = grid.NodeFromWorldPoint(request.pathEnd);

        if (startNode.isWalkable && targetNode.isWalkable)
        {
            //On stock les cases a verifier dans un heap
            Heap<PathNode> openSet = new Heap<PathNode>(grid.MaxSize);
            //On stock les cases deja verifier dans un hashset
            HashSet<PathNode> closedSet = new HashSet<PathNode>();
            //On ajoute la node de depart dans le heap des cases a verifier
            openSet.Add(startNode);

            //Tant qu'il y a des cases a verifier
            while (openSet.Count > 0)
            {
                //On classe les nodes en ordre de cout de mouvement
                PathNode currentNode = openSet.RemoveFirst();
                //On met la node actuelle dans le hashset des cases deja verifier
                closedSet.Add(currentNode);

                //Si on a atteint la node finale
                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    //On trace le path
                    break;
                }

                //Pour chaque case voisines
                foreach (PathNode neighbour in grid.GetNeighbours(currentNode))
                {
                    //Si elle n'est pas walkable, on la skip
                    if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                        continue;

                    int newMoveCostToNeighbour;
                    //On regarde le cout potentiel de la case voisine
                    if (!isFlying)
                        newMoveCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty + neighbour.airPenalty;
                    else
                        newMoveCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;

                    //Si celui-ci est plus plus bas ou qu'il n'est pas dans la liste des cases a verifier
                    if (newMoveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMoveCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.cameFromNode = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        else
            Debug.Log("Can't find path");
        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    Vector2[] RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.cameFromNode;
        }
        Vector2[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector2[] SimplifyPath(List<PathNode> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridPosX - path[i].gridPosX, path[i - 1].gridPosY - path[i].gridPosY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPos);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(PathNode nodeA, PathNode nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridPosX - nodeB.gridPosX);
        int distanceY = Mathf.Abs(nodeA.gridPosY - nodeB.gridPosY);

        if (distanceX > distanceY)
        {
            return distanceY * MOVE_DIAGONAL + (distanceX - distanceY) * MOVE_HORIZONTAL;
        }
        else
        {
            return distanceX * MOVE_DIAGONAL + (distanceY - distanceX) * MOVE_HORIZONTAL;
        }
    }
}
