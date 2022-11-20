using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;

public enum State 
{
    NONE,
    LOBBY,
    GAME
}

public enum Side
{
    SERVER,
    CLIENT
}

public class GameManager : MonoBehaviour
{
    public GameObject gameSetup;
    public GameObject cam;

    // Enemy variables
    [SerializeField]
    private PlayerMovement Player;
    [SerializeField]
    private GameObject Enemy;
    public string enemyUsername;
    public Vector3 enemyPosition;
    public float enemyRot;
    private State state;
    public Side side;

    [SerializeField]
    private TMP_Text serverScore;
    [SerializeField]
    private TMP_Text clientScore;

    private int playerLive, enemyLive;
    private bool updateScore;

    private void Awake()
    {
        playerLive = 0;
        enemyLive = 0;
    }
    void Start()
    {
        gameSetup.SetActive(true);
        cam.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        cam.transform.position = new Vector3(0, 100f, 0);
        
    }

    void Update()
    {
        Enemy.transform.position = enemyPosition;
        Enemy.transform.GetChild(0).localRotation = Quaternion.Euler(new Vector3(0, enemyRot, 0));

        if (updateScore)
        {
            if (side == Side.SERVER)
            {
                serverScore.text = playerLive.ToString();
                clientScore.text = enemyLive.ToString();
            }
            else
            {
                serverScore.text = enemyLive.ToString();
                clientScore.text = playerLive.ToString();
            }

            updateScore = false;
        }
    }

    public void SetState(State newState)
    {
        state = newState;
        Player.SetState(state);
    }
    public State GetState()
    {
        return state;
    }

    public void HitEnemy()
    {
        Debug.Log("Player hit Enemy");

        enemyLive += 1;
        updateScore = true;
    }

    public void HitPlayer()
    {
        Debug.Log("Enemy hit Player");

        playerLive += 1;
        updateScore = true;
    }
}
