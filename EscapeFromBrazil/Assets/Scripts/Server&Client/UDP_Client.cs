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
    public GameObject gameManager;
    public GameObject player;

    [HideInInspector]
    public GameManager gameManagerComp;

    private void Awake()
    {
        gameManagerComp = gameManager.GetComponent<GameManager>();
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
            gameManager.SetActive(true);
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
        }

        username = userNameText.text;

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
