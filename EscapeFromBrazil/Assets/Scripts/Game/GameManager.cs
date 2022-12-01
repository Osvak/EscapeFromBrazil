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

    // UI
    private int playerHp, enemyHp;
    [SerializeField] private int startingHp = 3;
    private bool updateScore;
    public Slider playerSlider;
    public Slider enemySlider;

    private void Awake()
    {
        playerHp = 3;
        enemyHp = 3;
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
}
