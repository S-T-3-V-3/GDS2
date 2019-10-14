using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public List<Tile> tiles;
    public List<Tile> walls;
    
    GameObject tileParent;
    GameObject wallParent;
    GameManager gameManager;
    MapSettings mapSettings;

    bool innerRadiusSpawned = false;
    bool wallsSpawned = false;
    bool obstaclesSpawned = false;

    void Awake() {
        gameManager = GameManager.Instance;
        mapSettings = gameManager.GetMapSettings();

        tiles = new List<Tile>();
        walls = new List<Tile>();
        
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

    public void UnloadMap() {
        foreach (Tile t in tiles)
            GameObject.Destroy(t.gameObject);

        foreach (Tile w in walls)
            GameObject.Destroy(w.gameObject);

        tiles.Clear();
        walls.Clear();

        GameObject.Destroy(tileParent.gameObject);
        GameObject.Destroy(wallParent.gameObject);

        Destroy(this);
    }

    void UpdateTilePositions() {
        gameManager.tilePositions.Clear();
        gameManager.tilePositions = tiles.Select(x => x.transform.position).ToList();
    }

    // TODO: This needs to be threaded
    void Update() {
        if (!innerRadiusSpawned) {
            CalculateEdges();

            List<Tile> edgeTiles = tiles.Where(x => x.isEdge).ToList();
            foreach (Tile currentEdgeTile in edgeTiles) {
                SpawnTiles(currentEdgeTile);
            }

            if (tiles[tiles.Count -1].distanceFromCenter >= mapSettings.innerRadius) {
                innerRadiusSpawned = true;

                UpdateTilePositions();
            }
                
        }
        else if (innerRadiusSpawned && !wallsSpawned)
        {
            CalculateEdges();

            tiles[0].isEdge = false;

            SpawnWalls();
            wallsSpawned = true;

            foreach(Tile w in walls)
                w.SetWall();
        }
        else if (wallsSpawned && !obstaclesSpawned) {
            for (int i = 0; i < gameManager.GetMapSettings().numObstacles; i++) {                
                Tile tile = GetRandomTile();
                bool isEdge = tile.isEdge;

                while (isEdge) {
                    tile = GetRandomTile();
                    isEdge = tile.isEdge;
                }
                
                NewObstacle(tile);
            }

            obstaclesSpawned = true;
            gameManager.OnMapLoaded.Invoke();
        }
    }

    void NewObstacle(Tile tile) {
        tiles.Remove(tile);
        walls.Add(tile);

        tile.FindNeighbours();
        tile.SetWall();

        UpdateTilePositions();
    }

    Tile GetRandomTile() {
        return tiles[Random.Range(0,tiles.Count - 1)];
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
                tiles.Add(GameObject.Instantiate(gameManager.TilePrefab,tileParent.transform).GetComponent<Tile>());
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
                Tile tile = GameObject.Instantiate(gameManager.TilePrefab,tileParent.transform).GetComponent<Tile>();
                tile.Init(gameManager, distanceFromCenter, mapSettings.tileSize+mapSettings.tilePadding);
                tile.transform.position = newPosition;
                tiles.Add(tile);

                return tile;
            }
        }
        else {
            if (walls.Where(x => x.transform.position == newPosition).Count() == 0) {
                Tile wall = GameObject.Instantiate(gameManager.TilePrefab,tileParent.transform).GetComponent<Tile>();
                wall.Init(gameManager, distanceFromCenter, mapSettings.tileSize+mapSettings.tilePadding);
                wall.transform.position = newPosition;
                wall.transform.parent = wallParent.transform;
                walls.Add(wall);

                return wall;
            }
        }

        return null;
    }

    Vector3 GetNeighbouringPosition(int index) {
        float distance = mapSettings.tileSize + mapSettings.tilePadding;
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

[System.Serializable]
public class MapSettings {
    public float tileSize = 0.85f;
    public float tilePadding = 0.15f;
    public int innerRadius = 10;
    public int outerRadius = 5;
    [Range(0,100)]
    public float falloff = 50;
    [Range(0,10)]
    public int numAdditionalZones = 3;
    [Range(0,30)]
    public int numObstacles = 3;
    [Range(0,10)]
    public int maxObstacleSize = 3;
}