using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;
using System.Threading;
using System.IO;

public class UDP_Server : MonoBehaviour
{ 
    Socket newSocket;
    IPEndPoint ipep;
    EndPoint client;

    public TMP_InputField userNameText;

    MemoryStream serializeStream;
    MemoryStream deserializeStream;

    private string username;
    private string enemyUsername;

    private State state;

    public GameObject joinChatGO;
    public GameObject backgroundGO;
    public GameObject joinGameGO;
    public GameObject joinGameButton;
    public GameObject score;
    public GameObject gameManager;
    public TMP_Text versusText;
    private bool updateText = false;

    public GameObject player;
    private Shooting ShootManager;
    private PlayerMovement playerMov;

    [HideInInspector]
    public GameManager gameManagerComp;

    private void Awake()
    {
        gameManagerComp = gameManager.GetComponent<GameManager>();
        gameManagerComp.side = Side.SERVER;
        ShootManager = player.GetComponent<Shooting>();
        playerMov = player.GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        state = State.NONE;
    }

    void Update()
    {

        if (state == State.GAME)
        {
            StartCoroutine(SendInfo());
        }

        if (state == State.GAME) StartCoroutine(SendInfo());

        if(updateText) 
        {
            versusText.text = username + " vs " + enemyUsername;
            Serialize();
            updateText = false;
            joinGameButton.SetActive(true);
        }
    }

    public void CreateServer()
    {
        Socketing();

        username = userNameText.text;

        joinChatGO.SetActive(false);
        joinGameGO.SetActive(true);
        versusText.text = "Waiting for an oponent...";
        state = State.LOBBY;
        gameManagerComp.SetState(state);
    }

    public void EnterGame()
    {
        state = State.GAME;
        backgroundGO.SetActive(false);
        joinGameGO.SetActive(false);
        score.SetActive(true);
        gameManager.SetActive(true);
        gameManagerComp.SetState(state);
        playerMov.tagName.text = username;
        Serialize();
    }

    private void Socketing()
    {
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 8000); // un puerto para el host, IPAddress.Any
        newSocket.Bind(ipep);

        // Inicializar Client
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        client = (EndPoint)(sender);

        Thread thread = new Thread(ReceieveClients);

        thread.Start();
    }

    private void ReceieveClients()
    {
        while (true)
        {
            byte[] data = new byte[2048];
            int recv = newSocket.ReceiveFrom(data, ref client);
            deserializeStream = new MemoryStream(data);
            Deserialize();
            updateText = true;
        }
    }

    private void Serialize()
    {
        serializeStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(serializeStream);
        
        writer.Write((int)state);
        switch(state)
        {
            case State.NONE:
                break;
            case State.LOBBY:
                writer.Write(username);
                break;
            case State.GAME:
                writer.Write(player.transform.position.x);
                writer.Write(player.transform.position.y);
                writer.Write(player.transform.position.z);
                writer.Write(player.transform.GetChild(0).transform.rotation.eulerAngles.y);
                writer.Write(ShootManager.shootP);
                ShootManager.shootP = false;
                writer.Write(playerMov.hit);
                if (playerMov.hit)
                {
                    playerMov.hit = false;
                }

                // PowerUp
                writer.Write(gameManagerComp.PU_active);
                if (gameManagerComp.PU_active)
                {
                    writer.Write(gameManagerComp.PU_pos.x);
                    writer.Write(gameManagerComp.PU_pos.y);
                    writer.Write(gameManagerComp.PU_pos.z);
                    writer.Write(gameManagerComp.PU_type);
                    writer.Write(gameManagerComp.PU_activeID);
                    gameManagerComp.PU_active = false;
                }
                // Delete PowerUp
                writer.Write(gameManagerComp.PU_delete);
                if (gameManagerComp.PU_delete)
                {
                    writer.Write(gameManagerComp.PU_deleteID);
                    gameManagerComp.PU_delete = false;
                }

                break;
            default:
                break;
        }

        newSocket.SendTo(serializeStream.ToArray(), serializeStream.ToArray().Length, SocketFlags.None, client);
        serializeStream.Dispose();
    }

    private void Deserialize()
    {
        BinaryReader reader = new BinaryReader(deserializeStream);
        deserializeStream.Seek(0, SeekOrigin.Begin);

        switch(state)
        {
            case State.NONE:
                break;
            case State.LOBBY:
                enemyUsername = gameManagerComp.enemyUsername = reader.ReadString();
                break;
            case State.GAME:
                gameManagerComp.enemyPosition.x = reader.ReadSingle();
                gameManagerComp.enemyPosition.y = reader.ReadSingle();
                gameManagerComp.enemyPosition.z = reader.ReadSingle();
                gameManagerComp.enemyRot = reader.ReadSingle();
                if (reader.ReadBoolean())
                {
                    ShootManager.ShootEnemy();
                }
                if (reader.ReadBoolean())
                {
                    gameManagerComp.HitEnemy();
                }

                // Delete PowerUp
                if (reader.ReadBoolean())
                {
                    gameManagerComp.EnemyCatchesPowerUp(reader.ReadInt32());
                }

                break;
            default:
                break;
        }

        deserializeStream.Dispose();
    }

    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize();
    }
}
