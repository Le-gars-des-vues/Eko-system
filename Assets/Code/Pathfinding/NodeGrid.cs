using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeGrid : MonoBehaviour
{
    //Pour voir la grille
    public bool DisplayGridGizmos;
    public Transform seeker;
    public LayerMask unWalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    [SerializeField] private float raycastOffset;
    [SerializeField] private float raycastLenght;
    [SerializeField] private float closeToGroundSizeX;
    [SerializeField] private float closeToGroundSizeY;

    private bool isCloseToGround;
    public TerrainType[] walkableRegions;
    LayerMask walkableMask;
    Dictionary<int, int> walkableRegionDictionnary = new Dictionary<int, int>();
    [SerializeField] private int farFromGroundPenalty;

    PathNode[,] grid;

    float nodeDiameter;
    public int gridSizeX, gridSizeY;

    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;
    int airPenaltyMin = int.MaxValue;
    int airPenaltyMax = int.MinValue;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value += region.terrainMask.value;
            walkableRegionDictionnary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }

        CreateGrid();
        Debug.Log("Grid created");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unregistering the OnSceneLoaded callback to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method will be called when a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneLoader.MakeActiveScene(SceneLoader.Scene.Forest_Test);
    }

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    void CreateGrid()
    {
        grid = new PathNode[gridSizeX, gridSizeY];
        //Coin en bas a gauche
        Vector2 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics2D.OverlapBox(worldPoint, new Vector2(nodeDiameter * 0.95f, nodeDiameter * 0.95f), 0, unWalkableMask);

                int movementPenalty = 0;
                int airPenalty = 0;

                if (walkable)
                {
                    isCloseToGround = Physics2D.OverlapBox(worldPoint, new Vector2(nodeDiameter * closeToGroundSizeX, nodeDiameter * closeToGroundSizeY), 0, unWalkableMask);
                    if (!isCloseToGround)
                        airPenalty = farFromGroundPenalty;
                }

                if (walkable)
                {
                    RaycastHit2D hit = Physics2D.Raycast(new Vector3(worldPoint.x, worldPoint.y, 9), worldPoint, 10, walkableMask);
                    if (hit.collider != null)
                    {
                        walkableRegionDictionnary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                }

                grid[x, y] = new PathNode(walkable, worldPoint, x, y, isCloseToGround, movementPenalty, airPenalty);
            }
        }
        
        BlurPenaltyMap(3, false);
        BlurPenaltyMap(3, true);
    }

    public void BlurPenaltyMap(int blurSize, bool airPenalty)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] penaltyHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltyVerticalPass = new int[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                if (!airPenalty)
                    penaltyHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
                else
                    penaltyHorizontalPass[0, y] += grid[sampleX, y].airPenalty;
            }

            for (int x = 1; x <gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                if (!airPenalty)
                    penaltyHorizontalPass[x, y] = penaltyHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
                else
                    penaltyHorizontalPass[x, y] = penaltyHorizontalPass[x - 1, y] - grid[removeIndex, y].airPenalty + grid[addIndex, y].airPenalty;
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltyVerticalPass[x, 0] += penaltyHorizontalPass[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)penaltyVerticalPass[x, 0] / (kernelSize * kernelSize));
            if (!airPenalty)
                grid[x, 0].movementPenalty = blurredPenalty;
            else
                grid[x, 0].airPenalty = blurredPenalty;

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                penaltyVerticalPass[x, y] = penaltyVerticalPass[x, y - 1] - penaltyHorizontalPass[x, removeIndex] + penaltyHorizontalPass[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltyVerticalPass[x, y] / (kernelSize * kernelSize));
                if (!airPenalty)
                    grid[x, y].movementPenalty = blurredPenalty;
                else
                    grid[x, y].airPenalty = blurredPenalty;

                if (!airPenalty)
                {
                    if (blurredPenalty > penaltyMax)
                        penaltyMax = blurredPenalty;

                    if (blurredPenalty < penaltyMin)
                        penaltyMin = blurredPenalty;
                }
                else
                {
                    if (blurredPenalty > airPenaltyMax)
                        airPenaltyMax = blurredPenalty;

                    if (blurredPenalty < airPenaltyMin)
                        airPenaltyMin = blurredPenalty;
                }
            }
        }
    }

    public List<PathNode> GetNeighbours(PathNode node)
    {
        List<PathNode> neighboursList = new List<PathNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)// || Mathf.Abs(x) + Mathf.Abs(y) == 2)
                    continue;
                int checkX = node.gridPosX + x;
                int checkY = node.gridPosY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighboursList.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighboursList;
    }

    public PathNode NodeFromWorldPoint(Vector2 worldPos)
    {
        float percentX = ((worldPos.x - transform.position.x) / gridWorldSize.x + 0.5f);
        float percentY = ((worldPos.y - transform.position.y) / gridWorldSize.y + 0.5f);
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        
        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));
        if (grid != null && DisplayGridGizmos)
        {
            //PathNode finderNode = NodeFromWorldPoint(seeker.position);
            foreach (PathNode node in grid)
            {
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(airPenaltyMin, airPenaltyMax, node.airPenalty));
                Gizmos.color = node.isWalkable ? Gizmos.color : Color.red;
                /*
                if (node.isWalkable)
                    Gizmos.color = node.isCloseToGround ? Color.white : Color.black;
                if (finderNode == node)
                    Gizmos.color = Color.cyan;
                */
                Gizmos.DrawCube(node.worldPos, Vector2.one * (nodeDiameter - 0.1f));
            }
        }
    }

    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
