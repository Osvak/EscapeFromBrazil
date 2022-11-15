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

    private string username, clientUsername;

    private bool clientLogged = false;

    public GameObject joinChatGO;
    public GameObject backgroundGO;
    public GameObject gameManager;

    private Vector3 enemyPos;

    void Update()
    {
        if (clientLogged) StartCoroutine(SendInfo());
        Debug.Log(clientUsername);
        Debug.Log("Enemy position: " + enemyPos);
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
        writer.Write(GameObject.Find("Player").transform.position.x);
        writer.Write(GameObject.Find("Player").transform.position.y);
        writer.Write(GameObject.Find("Player").transform.position.z);

        newSocket.SendTo(serializeStream.ToArray(), serializeStream.ToArray().Length, SocketFlags.None, client);
        serializeStream.Dispose();
    }

    private void Deserialize()
    {
        BinaryReader reader = new BinaryReader(deserializeStream);
        deserializeStream.Seek(0, SeekOrigin.Begin);

        clientUsername = reader.ReadString();
        if(clientLogged)
        {
            enemyPos.x = reader.ReadSingle();
            enemyPos.y = reader.ReadSingle();
            enemyPos.z = reader.ReadSingle();
        }

        deserializeStream.Dispose();
    }

    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize();
    }
}
