using System.Collections;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using System.Threading;
using System.IO;

public class UDP_Client : MonoBehaviour
{
    Socket newSocket;
    IPEndPoint ipep;
    EndPoint server;

    Thread ReceiveThread;

    public TMP_InputField userNameText;
    public TMP_InputField IpServerText;

    MemoryStream serializeStream;
    MemoryStream deserializeStream;

    private string username;
    private string enemyUsername;

    private State state;

    private bool connected = false;
    private bool firstTime = true;
    private bool updateText = false;

    public TMP_Text versusText;
    public GameObject joinChatGO;
    public GameObject backgroundGO;
    public GameObject joinGameGO;
    public GameObject score;
    public GameObject gameManager;
    public GameObject player;
    private Shooting ShootManager;


    [HideInInspector]
    public GameManager gameManagerComp;
    private PlayerMovement playerMov;

    private void Awake()
    {
        gameManagerComp = gameManager.GetComponent<GameManager>();
        gameManagerComp.side = Side.CLIENT;
        ShootManager = player.GetComponent<Shooting>();
        playerMov = player.GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        state = State.NONE;
    }

    void Update()
    {

        if (connected) StartCoroutine(SendInfo());
        if(state == State.GAME && firstTime)
        {
            joinGameGO.SetActive(false);
            backgroundGO.SetActive(false);
            score.SetActive(true);
            gameManager.SetActive(true);
            gameManagerComp.SetState(state);
            firstTime = !firstTime;
        }
        if(state == State.LOBBY && updateText)
        {
            versusText.text = "Waiting for " + enemyUsername + " to start the game.";
            updateText = false;
        }
    }

    public void EnterServer()
    {
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 6799); // IPAddress.Any, puerto del cliente
        newSocket.Bind(ipep);
        server = new IPEndPoint(IPAddress.Parse(IpServerText.text), 8000);
        newSocket.Connect(server);

        if(newSocket.Connected) 
        {
            connected = !connected;
            state = State.LOBBY;
            gameManagerComp.SetState(state);
        }

        username = userNameText.text;
        playerMov.tagName.text = username;

        joinChatGO.SetActive(false);
        joinGameGO.SetActive(true);

        Serialize();

        ReceiveThread = new Thread(Receiver);
        ReceiveThread.Start();

    }

    public void AutoEnterServer()
    {
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 6799); // IPAddress.Any, puerto del cliente
        newSocket.Bind(ipep);
        server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
        newSocket.Connect(server);

        if (newSocket.Connected)
        {
            connected = !connected;
            state = State.LOBBY;
            gameManagerComp.SetState(state);
        }

        username = "ClientUser";

        joinChatGO.SetActive(false);
        joinGameGO.SetActive(true);

        Serialize();

        ReceiveThread = new Thread(Receiver);
        ReceiveThread.Start();
    }

    private void Receiver()
    {
        while(true)
        {
            byte[] data = new byte[2048];
            int recv = newSocket.ReceiveFrom(data, ref server);
            deserializeStream = new MemoryStream(data);
            Deserialize();
        }
    }

    private void Serialize()
    {
        serializeStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(serializeStream);
        
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

        newSocket.SendTo(serializeStream.ToArray(), serializeStream.ToArray().Length, SocketFlags.None, server);
        serializeStream.Dispose();
    }

    private void Deserialize()
    {
        BinaryReader reader = new BinaryReader(deserializeStream);
        deserializeStream.Seek(0, SeekOrigin.Begin);

        state = (State)reader.ReadInt32();
        switch(state)
        {
            case State.NONE:
                break;
            case State.LOBBY:
                enemyUsername = gameManagerComp.enemyUsername = reader.ReadString();
                updateText = true;
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

                // PowerUp
                if (reader.ReadBoolean())
                {
                    gameManagerComp.PU_pos.x = reader.ReadSingle();
                    gameManagerComp.PU_pos.y = reader.ReadSingle();
                    gameManagerComp.PU_pos.z = reader.ReadSingle();
                    gameManagerComp.PU_type = reader.ReadInt32();
                    gameManagerComp.PU_activeID = reader.ReadInt32();
                    gameManagerComp.PowerUpAppears();
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
