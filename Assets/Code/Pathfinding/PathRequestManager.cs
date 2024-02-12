using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{
    //Queue pour storer les path request
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    //Assure qu'il n'y a qu'un seul manager et que ce soit toujours le meme qui soit appele
    static PathRequestManager Instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        Instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    //Methode pour construire une path request demandant un point de depart, un point d'arriver et une action qui store la liste des vector utiliser pour le path et une bool de success ou echec
    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback, bool isFlying)
    {
        //Cree la nouvelle path request
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        //Met la path request dans la queue
        Instance.pathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext(isFlying);
    }

    //S'il y a des request dans la queue et que nous ne somme pas deja en train de process une request
    void TryProcessNext(bool isFlying)
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            //On enleve la request actuelle de la queue et on commence une nouvelle request
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, isFlying);
        }
    }

    public void FinishedProcessingPath(Vector2[] path, bool success, bool isFlying)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext(isFlying);
    }

    //Structure avec tout les element d'une path request
    struct PathRequest
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
}
