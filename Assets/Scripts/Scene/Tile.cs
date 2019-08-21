using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Tile[] neighbours;
    public int distanceFromCenter;
    public bool isWall = false;
    public bool isEdge = false;
    public bool isInitialized = false;


    Vector3[] traceRotations;

    TeamID currentTeam = TeamID.NONE;
    GameManager gameManager;
    TileManager tileManager;
    float meshSize;

    public void Init(GameManager gameManager, int distanceFromCenter, float meshSize) {
        this.gameManager = gameManager;
        this.tileManager = gameManager.GetComponent<TileManager>();
        this.distanceFromCenter = distanceFromCenter;
        this.meshSize = meshSize;
    }

    // TODO: Change this to be controlled externally and call an update material function
    // Player primary control allows for status of friendly/enemy territory update prior to capture
    void OnTriggerEnter(Collider other)
    {   
        if (other.GetComponent<PlayerController>() == null || isWall) return;

        PlayerController overlappingPlayer = other.GetComponent<PlayerController>();

        if (currentTeam != overlappingPlayer.teamID) {
            gameManager.sessionData.score.UpdateScore(currentTeam,overlappingPlayer.teamID);
            currentTeam = overlappingPlayer.teamID;
            gameManager.OnScoreUpdated.Invoke();
        }

        this.gameObject.GetComponent<MeshRenderer>().material = gameManager.GetTeamManager().GetTeamColor(overlappingPlayer.teamID);
    }

    public TeamID GetTeam() {
        return currentTeam;
    }

    public void FindNeighbours() {
        neighbours = new Tile[6];

        for (int i = 0; i < 6; i++)
        {
            neighbours[i] = FindNeighbour(i);
        }

        isEdge = neighbours.Where(x => x == null).Count() > 0 ? true : false;

        isInitialized = true;
    }

    public void SetWall() {
        isWall = true;
        this.GetComponent<MeshRenderer>().material = gameManager.GetTeamManager().GetTeamColor(TeamID.PURPLE);
        this.GetComponent<MeshCollider>().enabled = true;
        this.transform.position += new Vector3(0,1,0);
    }

    Tile FindNeighbour(int index) {
        Vector3 neighbourPosition = this.transform.position + GetNeighbouringPosition(index);
        if (tileManager.tiles.Where(x => x.transform.position == neighbourPosition).Count() > 0) {
            return tileManager.tiles.Where(x => x.transform.position == neighbourPosition).First();
        }
        else
        {
            return null;
        }
    }

    Vector3 GetNeighbouringPosition(int index) {
        switch (index) {
            case 0:
                return new Vector3(0,0,2f*meshSize);
            case 1:
                return new Vector3((3*meshSize)/Mathf.Sqrt(3),0,meshSize);
            case 2:
                return new Vector3((3*meshSize)/Mathf.Sqrt(3),0,-meshSize);
            case 3:
                return new Vector3(0,0,-2f*meshSize);
            case 4:
                return new Vector3((-3*meshSize)/Mathf.Sqrt(3),0,-meshSize);
            case 5:
                return new Vector3((-3*meshSize)/Mathf.Sqrt(3),0,meshSize);
            default:
                return Vector3.zero;
        }
    }
}

