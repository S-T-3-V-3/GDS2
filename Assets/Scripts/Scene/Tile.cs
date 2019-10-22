using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject HexagonEffect;
    public GameObject StarsEffect;
    public TileSprite HexSprite;
    [Space]
    public Tile[] neighbours;
    public int distanceFromCenter;
    public bool isWall = false;
    public bool isEdge = false;
    public bool isInitialized = false;
    public int owningPlayerID = 0;


    Vector3[] traceRotations;
    Vector3 wallOffset = new Vector3(0,1.5f,0);

    TeamID currentTeam = TeamID.NONE;
    GameManager gameManager;
    MapManager mapManager;

    GameObject currentHexEffect;
    GameObject currentStarsEffect;

    float meshSize;

    Material newTeamMat;

    public void Init(GameManager gameManager, int distanceFromCenter, float meshSize) {
        this.gameManager = gameManager;
        this.mapManager = gameManager.GetComponent<MapManager>();
        this.distanceFromCenter = distanceFromCenter;
        this.meshSize = meshSize;
    }

    // TODO: Change this to be controlled externally and call an update material function
    // Player primary control allows for status of friendly/enemy territory update prior to capture
    void OnTriggerEnter(Collider other)
    {   
        if (gameManager.sessionData.roundManager.isStarted == false || isWall) return;

        TeamID targetTeamID = currentTeam;
        int playerID = 0;

        if (other.tag == "Player") {
            targetTeamID = other.GetComponent<PlayerModelController>().owner.teamID;
            playerID = other.GetComponent<PlayerModelController>().owner.playerID;
        }
        else if (other.GetComponent<PlayerPostDeathHandler>() != null) {
            targetTeamID = other.GetComponent<PlayerPostDeathHandler>().targetTeam;
            playerID = other.GetComponent<PlayerPostDeathHandler>().owningPlayerID;
        }

        if (currentTeam != targetTeamID) {
                gameManager.sessionData.score.UpdateTileCount(currentTeam,targetTeamID);
                currentTeam = targetTeamID;
                gameManager.OnTilesChanged.Invoke();
                owningPlayerID = playerID;
                gameManager.sessionData.gameStats.TileCaptured(playerID);

                if (currentHexEffect != null) {
                    GameObject.Destroy(currentHexEffect);
                }
                
                currentHexEffect = GameObject.Instantiate(HexagonEffect,this.transform.position,this.transform.rotation,this.transform);
                currentHexEffect.transform.localPosition = new Vector3(0,0,0.0101f);

                currentStarsEffect = GameObject.Instantiate(StarsEffect,this.transform.position,this.transform.rotation,this.transform);
                currentStarsEffect.transform.localPosition = new Vector3(0,0,0.0101f);

                ParticleSystem.MainModule main = currentHexEffect.GetComponent<ParticleSystem>().main;
                main.startColor = TeamManager.Instance.GetTeamColor(targetTeamID);

                main = currentStarsEffect.GetComponent<ParticleSystem>().main;
                main.startColor = TeamManager.Instance.GetTeamColor(targetTeamID);

                newTeamMat = gameManager.teamManager.GetTeam(targetTeamID).tileMat;

                HexSprite.DoColorChange(TeamManager.Instance.GetTeamColor(targetTeamID));

                //currentHexEffect.GetComponent<ParticleEvents>().OnParticleComplete.AddListener(DoColorChange);
            }
    }

    public TeamID GetTeam() {
        return currentTeam;
    }

    public void FindNeighbours() {
        neighbours = new Tile[6];

        for (int i = 0; i < 6; i++)
        {
            neighbours[i] = GetNeighbour(i);
        }

        isEdge = neighbours.Where(x => x == null).Count() > 0 ? true : false;

        isInitialized = true;
    }

    public void SetWall() {
        isWall = true;
        this.GetComponent<MeshRenderer>().material = gameManager.WallMaterial;
        this.GetComponent<MeshCollider>().enabled = true;
        this.transform.position += wallOffset;
    }

    Tile GetNeighbour(int index) {
        Vector3 neighbourPosition = this.transform.position + GetNeighbouringPosition(index);

        if (isWall) {
            neighbourPosition -= wallOffset;
        }

        if (mapManager.tiles.Where(x => x.transform.position == neighbourPosition).Count() > 0) {
            return mapManager.tiles.Where(x => x.transform.position == neighbourPosition).First();
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

