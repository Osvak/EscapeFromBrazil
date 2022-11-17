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

    private bool clientLogged = false;

    public GameObject joinChatGO;
    public GameObject backgroundGO;

    public GameObject gameManager;
    public GameObject player;

    [HideInInspector]
    public GameManager gameManagerComp;

    private void Awake()
    {
        gameManagerComp = gameManager.GetComponent<GameManager>();
    }

    void Update()
    {
        if (clientLogged) StartCoroutine(SendInfo());
    }

    public void CreateServer()
    {
        Socketing();

        username = userNameText.text;

        joinChatGO.SetActive(false);
        backgroundGO.SetActive(false);
        gameManager.SetActive(true);
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
            byte[] data = new byte[1024];
            int recv = newSocket.ReceiveFrom(data, ref client);
            deserializeStream = new MemoryStream(data);
            Deserialize();
            if (!clientLogged) clientLogged = !clientLogged;
        }
    }

    private void Serialize()
    {
        serializeStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(serializeStream);

        writer.Write(username);
        writer.Write(player.transform.position.x);
        writer.Write(player.transform.position.y);
        writer.Write(player.transform.position.z);
        writer.Write(player.transform.GetChild(0).transform.rotation.eulerAngles.y);

        newSocket.SendTo(serializeStream.ToArray(), serializeStream.ToArray().Length, SocketFlags.None, client);
        serializeStream.Dispose();
    }

    private void Deserialize()
    {
        BinaryReader reader = new BinaryReader(deserializeStream);
        deserializeStream.Seek(0, SeekOrigin.Begin);

        gameManagerComp.enemyUsername = reader.ReadString();
        if(clientLogged)
        {
            // Cuando se deserialicen cosas del enemigo, mandarlas al GameManager. Desde ahí updatear al enemigo.
            gameManagerComp.enemyPosition.x = reader.ReadSingle();
            gameManagerComp.enemyPosition.y = reader.ReadSingle();
            gameManagerComp.enemyPosition.z = reader.ReadSingle();
            gameManagerComp.enemyRot = reader.ReadSingle();

        }

        deserializeStream.Dispose();
    }

    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize();
    }
}
