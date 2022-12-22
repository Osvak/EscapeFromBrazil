using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    public Vector2 size;
    private Vector3 center;

    public enum PuType
    {
        Null,
        Live1,
        FiringRate
    }

    private int powerUpID = 0;

    public GameObject PowerUpPrefab;

    //new PowerUp
    private bool POPPowerUp = false, deletePowerUP = false;
    private Vector3 newPos;
    private int newType = 0, newID = 0, deleteId = 0;

    private List<GameObject> poweUps = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        center = this.transform.position;
        powerUpID = 0;
        POPPowerUp = false;
        deletePowerUP = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 1, 0.5f);
        Gizmos.DrawCube(this.transform.position, new Vector3(size.x, 0, size.y));
    }

    public void Restart()
    {
        powerUpID = 0;

        for (int i = 0; i < poweUps.Count; i++)
        {
            Destroy(poweUps[i]);
        }

        poweUps.Clear();
    }

    // Update is called once per frame
    void Update()
    {

        if (POPPowerUp)
        {
            POPPowerUp = false;
            GameObject newPowerUp = Instantiate(PowerUpPrefab, this.transform);
            newPowerUp.GetComponent<PoweUp>().StartSpawn(newPos, newType, newID);

            poweUps.Add(newPowerUp);

            powerUpID++;

        }

        if (deletePowerUP)
        {
            deletePowerUP = false;
            EnemyTypePowerUp();
            poweUps[deleteId].SetActive(false);
        }

        if (gameManager.GetState() == State.GAME)
        {
            StartCoroutine(CooldownSpawner());
        }
    }

    IEnumerator CooldownSpawner(){


        yield return new WaitForSeconds(Random.Range(5,11));

        SpawnPowerUp();
    }

    private void SpawnPowerUp()
    {
        
        float randomPosX = Random.Range((center.x - (size.x * 0.5f)), (center.x + (size.x * 0.5f)));
        float randomPosZ = Random.Range((center.z - (size.y * 0.5f)), (center.z + (size.y * 0.5f)));

        Vector3 spawnPos = new Vector3(randomPosX, center.y, randomPosZ);
        
        int type = Random.Range(1, 3);

        GameObject newPowerUp = Instantiate(PowerUpPrefab, this.transform);
        newPowerUp.GetComponent<PoweUp>().StartSpawn(spawnPos,type, powerUpID);

        poweUps.Add(newPowerUp);

        //Avisar a Game Man manager
        gameManager.SendPowerUP(spawnPos, type, powerUpID);
        powerUpID++;
    }

    public void SpawnPowerUPClient(Vector3 pos, int type, int id)
    {
        POPPowerUp = true;
        newPos = pos;
        newType = type;
        newID= id;
    }

    public void DeletePowerUp(int id)
    {
        deletePowerUP = true;
        deleteId = id;
    }


    // The Enemey cached a PowerUp
    private void EnemyTypePowerUp()
    {
        PuType type = (PuType)poweUps[deleteId].GetComponent<PoweUp>().type;
        switch (type)
        {
            case PuType.Null:
                break;
            case PuType.Live1:

                Debug.Log("enemy got extra life");
                gameManager.PowerUp1HPEnemy();
                break;
            case PuType.FiringRate:

                Debug.Log("enemy got increased rate of fire");

                break;
            default:
                break;
        }
    }

    // The Player cached a PowerUp
    public void ActivePowerUp(PuType type,int ID)
    {

        gameManager.PowerUpDisappears(ID);

        poweUps[ID].SetActive(false);

        switch (type)
        {
            case PuType.Live1:

                Debug.Log("Player got extra life");
                gameManager.PowerUp1HPPlayer();
                break;
            case PuType.FiringRate:

                Debug.Log("Player got increased rate of fire");
                gameManager.Player.PowerUpFireRatePlayer();
                break;
            default:
                break;
        }
    }


}
