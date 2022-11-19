using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State 
{
    NONE,
    LOBBY,
    GAME
}

public class GameManager : MonoBehaviour
{
    public GameObject gameSetup;
    public GameObject cam;

    // Enemy variables
    private GameObject Enemy;
    public string enemyUsername;
    public Vector3 enemyPosition;
    public float enemyRot;

    void Start()
    {
        gameSetup.SetActive(true);
        cam.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        cam.transform.position = new Vector3(0, 100f, 0);
        Enemy = GameObject.Find("Enemy");
        
    }

    void Update()
    {
        Enemy.transform.position = enemyPosition;
        Enemy.transform.GetChild(0).localRotation = Quaternion.Euler(new Vector3(0, enemyRot, 0));
    }

}
