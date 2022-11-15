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

    private string username, hostUsername;

    private bool connected = false;
    private bool firstTime = true;

    public GameObject joinChatGO;
    public GameObject backgroundGO;
    public GameObject gameManager;

    private Vector3 enemyPos;

    void Update()
    {
        if (connected) StartCoroutine(SendInfo());
        Debug.Log(hostUsername);
        Debug.Log("Enemy position: " + enemyPos);
    }

    public void EnterServer()
    {
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 6799); // IPAddress.Any, puerto del cliente
        newSocket.Bind(ipep);
        server = new IPEndPoint(IPAddress.Parse(IpServerText.text), 8000);
        newSocket.Connect(server);

        if(newSocket.Connected) connected = !connected;

        username = userNameText.text;

        joinChatGO.SetActive(false);
        backgroundGO.SetActive(false);
        gameManager.SetActive(true);

        Serialize();
        firstTime = false;

        ReceiveThread = new Thread(Receiver);
        ReceiveThread.Start();

    }

    private void Receiver()
    {
        while(true)
        {
            byte[] data = new byte[1024];
            int recv = newSocket.ReceiveFrom(data, ref server);
            deserializeStream = new MemoryStream(data);
            Deserialize();
        }
    }

    private void Serialize()
    {
        serializeStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(serializeStream);
        
        writer.Write(username);
        if(!firstTime)
        {
            writer.Write(GameObject.Find("Player").transform.position.x);
            writer.Write(GameObject.Find("Player").transform.position.y);
            writer.Write(GameObject.Find("Player").transform.position.z);
        }

        newSocket.SendTo(serializeStream.ToArray(), serializeStream.ToArray().Length, SocketFlags.None, server);
        serializeStream.Dispose();
    }

    private void Deserialize()
    {
        BinaryReader reader = new BinaryReader(deserializeStream);
        deserializeStream.Seek(0, SeekOrigin.Begin);

        hostUsername = reader.ReadString();
        enemyPos.x = reader.ReadSingle();
        enemyPos.y = reader.ReadSingle();
        enemyPos.z = reader.ReadSingle();

        deserializeStream.Dispose();
    }

    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize();
    }
}
