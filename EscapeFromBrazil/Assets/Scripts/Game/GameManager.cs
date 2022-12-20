using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;
using UnityEngine.UI;

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
    private State state;
    public Side side;

    [Header("Enemy variables")]
    [SerializeField]
    public PlayerMovement Player;
    [SerializeField]
    public GameObject Enemy;
    public string enemyUsername;
    public Vector3 enemyPosition;
    public float enemyRot;

    
    [Header("UI")]
    [SerializeField]
    private TMP_Text serverScore;
    [SerializeField]
    private TMP_Text clientScore;

    private int playerHp, enemyHp;
    private int startingHp = 4;
    private bool updateScore;
    public Slider playerSlider;
    public Slider enemySlider;

    [Header("PowerUp")]
    [SerializeField] private PowerUpManager powerUpManager;
    public bool PU_active = false, PU_delete = false;
    public Vector3 PU_pos = Vector3.zero;
    public int PU_type = 0;
    public int PU_activeID = 0,PU_deleteID = 0;


    private GameObject trash;

    private void Awake()
    {
        trash = GameObject.Find("Trash");
        //ResetGame();
        foreach (Transform child in trash.transform)
            GameObject.Destroy(child.gameObject);

        playerHp = startingHp;
        enemyHp = startingHp;

        Player.PlayerReset();

        if (side == Side.SERVER)
        {
            serverScore.text = playerHp.ToString();
            clientScore.text = enemyHp.ToString();
        }
        else
        {
            serverScore.text = enemyHp.ToString();
            clientScore.text = playerHp.ToString();
        }
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
            if(playerHp <= 0 || enemyHp <= 0)
            {
                ResetGame();
            }
            if (side == Side.SERVER)
            {
                serverScore.text = playerHp.ToString();
                clientScore.text = enemyHp.ToString();
            }
            else
            {
                serverScore.text = enemyHp.ToString();
                clientScore.text = playerHp.ToString();
            }
            UpdateSlider();
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

        enemyHp -= 1;
        updateScore = true;
    }

    public void HitPlayer()
    {
        Debug.Log("Enemy hit Player");

        playerHp -= 1;
        updateScore = true;
    }

    private void UpdateSlider()
    {
        if(side == Side.SERVER)
        {
            playerSlider.value = (float)playerHp;
            enemySlider.value = (float)enemyHp;
        }
        else
        {
            playerSlider.value = (float)enemyHp;
            enemySlider.value = (float)playerHp;
        }
    }

    private void ResetGame()
    {
        foreach (Transform child in trash.transform)
            GameObject.Destroy(child.gameObject);

        playerHp = startingHp;
        enemyHp = startingHp;

        Player.PlayerReset();
        powerUpManager.Restart();
    }

    //--PowerUP

    public void SendPowerUP(Vector3 pos, int type, int id)
    {
        PU_active = true;
        PU_pos = pos;
        PU_type = type;
        PU_activeID = id;
    }

    public void PowerUpAppears()
    {
        powerUpManager.SpawnPowerUPClient(new Vector3(PU_pos.x, PU_pos.y, PU_pos.z), PU_type, PU_activeID);
    }

    public void PowerUpDisappears(int id)
    {
        /*
        A esto se llamara cuando el Player coja un powerUp
        Cambiaremos la id y avisaremos al rival de cual debe destruir
         */
        PU_deleteID = id;
        PU_delete = true;
    }

    public void EnemyCatchesPowerUp(int id)
    {
        //cuando reciba que el enemigo a cogido un PowerUp
        powerUpManager.DeletePowerUp(id);
    }

    public void PowerUp1HPPlayer()
    {
        if(playerHp < startingHp) playerHp += 1;
        
        updateScore = true;
    }    
    public void PowerUp1HPEnemy()
    {
        if (enemyHp < startingHp) enemyHp += 1;

        updateScore = true;
    }
}
