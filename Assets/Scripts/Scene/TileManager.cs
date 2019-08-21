using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public float size;
    public float padding = 0;
    [Space]
    public List<Tile> tiles;
    public List<Tile> walls;
    public int innerRadius = 10;
    public int outerRadius = 5;
    [Range(0,100)]
    public float falloff;
    [Range(0,10)]
    public int numAdditionalZones;
    [Range(0,10)]
    public int numObstacles;

    GameObject tileParent;
    GameObject wallParent;
    GameManager gameManager;

    bool innerRadiusSpawned = false;
    bool wallsSpawned = false;

    void Start() {
        gameManager = this.GetComponent<GameManager>();
        
        if (tileParent == null)
        {
            tileParent = new GameObject("Tile Container");
        }

        if (wallParent == null)
        {
            wallParent = new GameObject("Wall Container");
        }

        CreateTile(Vector3.zero, -1);
    }

    void Update() {
        if (!innerRadiusSpawned) {
            CalculateEdges();

            List<Tile> edgeTiles = tiles.Where(x => x.isEdge).ToList();
            foreach (Tile currentEdgeTile in edgeTiles) {
                SpawnTiles(currentEdgeTile);
            }

            if (tiles[tiles.Count -1].distanceFromCenter >= innerRadius)
                innerRadiusSpawned = true;
        }
        else if (innerRadiusSpawned && !wallsSpawned)
        {
            CalculateEdges();

            SpawnWalls();
            wallsSpawned = true;

            foreach(Tile w in walls)
                w.SetWall();
        }
    }

    void CalculateEdges() {
        foreach (Tile t in tiles.Where(x => x.isEdge || x.isInitialized == false))
            t.FindNeighbours(); 
    }

    void SpawnTiles(Tile currenTile) {
        List<Tile> neighbourTiles = new List<Tile>();
        neighbourTiles.AddRange(currenTile.neighbours);

        for (int i = 0; i < 6; i++)
        {
            if (neighbourTiles[i] == null) 
                neighbourTiles[i] = CreateTile(currenTile.transform.position, i, currenTile.distanceFromCenter + 1);
        }  
    }

    void SpawnWalls() {
        List<Tile> edgeTiles = tiles.Where(x => x.isEdge).ToList();

        foreach (Tile edge in edgeTiles) {
            for (int i = 0; i < 6; i++)
            {
                if(edge.neighbours[i] == null) {
                    CreateTile(edge.transform.position, i, edge.distanceFromCenter + 1, true);
                }
            }
        }
    }

    void AddNeighbours(Tile currentTile) {
        for (int i = 0; i < 6; i++)
        {
            if (currentTile.neighbours[i] == null) {
                tiles.Add(GameObject.Instantiate(tilePrefab,tileParent.transform).GetComponent<Tile>());
            }
        }
    } 

    Tile CreateTile(Vector3 currentPosition, int neighborIndex, int distanceFromCenter = 0, bool isWall = false) {
        Vector3 newPosition;
        if (neighborIndex != -1)
            newPosition = GetNeighbouringPosition(neighborIndex) + currentPosition;
        else
            newPosition = Vector3.zero;

        if(!isWall) {
            if (tiles.Where(x => x.transform.position == newPosition).Count() == 0) {
                Tile tile = GameObject.Instantiate(tilePrefab,tileParent.transform).GetComponent<Tile>();
                tile.Init(gameManager, distanceFromCenter, size+padding);
                tile.transform.position = newPosition;
                tiles.Add(tile);

                return tile;
            }
        }
        else {
            if (walls.Where(x => x.transform.position == newPosition).Count() == 0) {
                Tile wall = GameObject.Instantiate(tilePrefab,tileParent.transform).GetComponent<Tile>();
                wall.Init(gameManager, distanceFromCenter, size+padding);
                wall.transform.position = newPosition;
                wall.transform.parent = wallParent.transform;
                walls.Add(wall);

                return wall;
            }
        }

        return null;
    }

    Vector3 GetNeighbouringPosition(int index) {
        float distance = size + padding;
        switch (index) {
            case 0:
                return new Vector3(0,0,2f*distance);
            case 1:
                return new Vector3((3*distance)/Mathf.Sqrt(3),0,distance);
            case 2:
                return new Vector3((3*distance)/Mathf.Sqrt(3),0,-distance);
            case 3:
                return new Vector3(0,0,-2f*distance);
            case 4:
                return new Vector3((-3*distance)/Mathf.Sqrt(3),0,-distance);
            case 5:
                return new Vector3((-3*distance)/Mathf.Sqrt(3),0,distance);
            default:
                return Vector3.zero;
        }
    }
}