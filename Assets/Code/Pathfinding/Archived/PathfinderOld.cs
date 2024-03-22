using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class Pathfinder
{
    //Cout pour les cases horizontal et diagonal
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    //La grille utiliser dans le pathfinding
    private GridMap<Node> gridMap;
    //La liste des nodes a verifier pour la pathfinding
    private List<Node> openList;
    //La liste des nodes deja verifier
    private List<Node> closedList;

    public static Pathfinder Instance { get; private set; }

    //Constructeur de grille specifique au pathfinding
    public Pathfinder(int width, int height, float cellSize, Vector3 origin)
    {
        Instance = this;
        gridMap = new GridMap<Node>(width, height, cellSize, origin, (GridMap<Node> g, int x, int y) => new Node(g, x, y));
    }

    public GridMap<Node> GetGrid()
    {
        return gridMap;
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        gridMap.GetXY(startWorldPosition, out int startX, out int startY);
        gridMap.GetXY(endWorldPosition, out int endX, out int endY);

        List<Node> path = FindPath(startX, startY, endX, endY);
        if (path == null)
            return null;
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (Node pathNode in path)
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * gridMap.GetCellSize() + Vector3.one * gridMap.GetCellSize() * 0.5f);
            return vectorPath;
        }
    }

    //Fonction pour trouver le chemin
    public List<Node> FindPath(int startX, int startY, int endX, int endY)
    {
        //La premier node est celle a l'origine de la grille
        Node startNode = gridMap.GetGridObject(startX, startY);
        //La derniere node est la position de la cible
        Node endNode = gridMap.GetGridObject(endX, endY);

        if (!startNode.isWalkable || !endNode.isWalkable)
            return null;

        //Creation des listes
        openList = new List<Node> { startNode };
        closedList = new List<Node>();

        //On parcours la grille en regardant le fCost pour se rendre a la cible de chaque case
        for (int x = 0; x < gridMap.GetWidth(); x++)
        {
            for (int y = 0; y < gridMap.GetHeight(); y++)
            {
                Node pathNode = gridMap.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }
        //On calcule les cout de la premier case
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        //Pendant qu'il y a des case a verifier
        while (openList.Count > 0)
        {
            //On check pour celles avec le fCost le plus bas
            Node currentNode = GetLowestFCost(openList);
            //Si la node actuelle est la dernier
            if (currentNode == endNode)
            {
                //On a atteind la derniere node
                return CalculatePath(endNode);
            }
            //Une fois que c'est fait, on enleve la node de la liste a verifier et on la met dans la liste des node deja verifier
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //Pour chaque node dans la liste des voisins
            foreach (Node neighbourNode in GetNeighbourList(currentNode))
            {
                if (neighbourNode != null)
                {
                    //Si la liste contien deja la node, on l'ignore
                    if (closedList.Contains(neighbourNode))
                        continue;
                    if (!neighbourNode.isWalkable)
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }

                    //On regade si passer par la node voisine offre un meilleur chemin que notre chemin actuel
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    //Si oui :
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        //La node precedente du voisin devient notre node actuelle
                        neighbourNode.cameFromNode = currentNode;
                        //Le gCost de la node voisine devient le tentativeGCost
                        neighbourNode.gCost = tentativeGCost;
                        //On calcule le hCost
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                        //On calcule le fCost;
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode))
                            openList.Add(neighbourNode);
                    }
                }
            }
        }
        //Plus de node a verifier et aucune fit
        return null;
    }

    //Fonction qui renvois la liste des nodes voisines
    private List<Node> GetNeighbourList(Node currentNode)
    {
        List<Node> neighbourList = new List<Node>();
        //Si une node voisine est a l'interieur de la grille, on l'ajoute a la liste des nodes voisines
        if (currentNode.x - 1 >= 0)
        {
            //Gauche
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            //Gauche en bas
            if (currentNode.y - 1 >= 0)
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            //Gauche en haut
            if (currentNode.y + 1 >= 0)
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < gridMap.GetWidth())
        {
            //Droite
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            //Droite en base
            if (currentNode.y - 1 >= 0)
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            //Droite en haut
            if (currentNode.y + 1 < gridMap.GetHeight())
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        //En bas
        if (currentNode.y - 1 >= 0)
            neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        //En haut
        if (currentNode.y + 1 < gridMap.GetHeight())
            neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    public Node GetNode(int x, int y)
    {
        return gridMap.GetGridObject(x, y);
    }


    private List<Node> CalculatePath(Node endNode)
    {
        List<Node> path = new List<Node>();
        path.Add(endNode);
        Node currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(Node a, Node b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private Node GetLowestFCost(List<Node> pathNodeList)
    {
        Node lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}
*/
