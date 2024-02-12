using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/*
public class Setup : MonoBehaviour
{
    private PathfinderHandmade pathfinding;
    [SerializeField] private Transform origin;
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize;
    [SerializeField] private MonkeyPathfinding monkey;

    private void Start()
    {
        pathfinding = new PathfinderHandmade(gridWidth, gridHeight, cellSize, origin.position);
        Debug.Log(pathfinding.GetGrid().GetCellSize());
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
            pathfinding.GetGrid().GetXY(mousePos, out int x, out int y);
            List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * cellSize + Vector3.one * (cellSize / 2), new Vector3(path[i + 1].x, path[i + 1].y) * cellSize + Vector3.one * (cellSize / 2), Color.green);
            }
            monkey.SetTargetPosition(mousePos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
            pathfinding.GetGrid().GetXY(mousePos, out int x, out int y);
            pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
        }
    }
}
*/
