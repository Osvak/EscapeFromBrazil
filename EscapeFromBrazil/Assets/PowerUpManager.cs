using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    public Vector2 size;
    private Vector3 center;

    public GameObject PowerUp;
    // Start is called before the first frame update
    void Start()
    {
        center = this.transform.position;   
    }

    // Update is called once per frame
    void Update()
    {

        if (gameManager.side == Side.SERVER)
        {

        }

        if(Input.GetKeyDown(KeyCode.Q)) SpawnPowerUp();


    }

    private void SpawnPowerUp()
    {
        float randomPosX = Random.Range((center.x - (size.x * 0.5f)), (center.x + (size.x * 0.5f)));
        float randomPosZ = Random.Range((center.z - (size.y * 0.5f)), (center.z + (size.y * 0.5f)));

        Vector3 spawnPos = new Vector3(randomPosX, center.y, randomPosZ);

        GameObject powerUp = Instantiate(PowerUp,spawnPos,PowerUp.transform.rotation,this.transform);

    }

    public void SpawnPowerUPClient(Vector3 pos, int type)
    {
        GameObject powerUp = Instantiate(PowerUp, pos, PowerUp.transform.rotation, this.transform);
        powerUp.GetComponent<PoweUp>().SetType(type);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1,0,1,0.5f);
        Gizmos.DrawCube(this.transform.position, new Vector3(size.x,0,size.y));
    }

    public void ActivePowerUp(PoweUp.PuType type)
    {

        switch (type)
        {
            case PoweUp.PuType.Live1:

                Debug.Log("Vida Extra");

                break;
            case PoweUp.PuType.FiringRate:

                Debug.Log("Aumento de cadencia");

                break;
            default:
                break;
        }
    }
}
