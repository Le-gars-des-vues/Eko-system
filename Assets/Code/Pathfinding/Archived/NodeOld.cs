using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
//Class pour creer les objets PathNode qui seront utiliser pour le pathfinding
public class Node
{
    //Prend la grille dans laquelle les PathNode sont creer
    private GridMap<Node> gridMap;
    //Emplacement des nodes en X et Y
    public int x;
    public int y;

    //Cout pour se rendre a la cible
    public int gCost;
    //Cout depuis la case de depart
    public int hCost;
    //Combinaison du gCost et hCost
    public int fCost;

    //Est-ce que la surface est marchable?
    public bool isWalkable;
    //La derniere node passee dans le pathfinding
    public Node cameFromNode;

    //Constructeur de PathNode
    public Node(GridMap<Node> gridMap, int x, int y)
    {
        this.gridMap = gridMap;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    //Fonction pour calculer le fCost
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
        gridMap.TriggerGridObjectChanged(x, y);
    }

    //Utilise pour le debug
    public override string ToString()
    {
        return x + "," + y;
    }
}
*/

