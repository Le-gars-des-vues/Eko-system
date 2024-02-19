using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathResult> results = new Queue<PathResult>();

    //Assure qu'il n'y a qu'un seul manager et que ce soit toujours le meme qui soit appele
    static PathRequestManager Instance;
    Pathfinding pathfinding;

    private void Awake()
    {
        Instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        if (results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    //Methode pour construire une path request demandant un point de depart, un point d'arriver et une action qui store la liste des vector utiliser pour le path et une bool de success ou echec
    public static void RequestPath(PathRequest request, bool isFlying)
    {
        ThreadStart threadStart = delegate
        {
            Instance.pathfinding.FindPath(request, Instance.FinishedProcessingPath, isFlying);
        };
        threadStart.Invoke();
    }

    public void FinishedProcessingPath(PathResult result)
    {
        lock (results)
        {
            results.Enqueue(result);
        }
    }
}

public struct PathResult
{
    public Vector2[] path;
    public bool success;
    public Action<Vector2[], bool> callback;

    public PathResult(Vector2[] path, bool success, Action<Vector2[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}

//Structure avec tout les element d'une path request
public struct PathRequest
{
    public Vector2 pathStart;
    public Vector2 pathEnd;
    public Action<Vector2[], bool> callback;

    //Constructeur de path request
    public PathRequest(Vector2 _start, Vector2 _end, Action<Vector2[], bool> _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }
}
