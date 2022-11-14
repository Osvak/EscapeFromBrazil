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

public class UDP_Client_Lan : MonoBehaviour
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

    void Update()
    {
        if (connected) StartCoroutine(SendInfo());
        Debug.Log(hostUsername);
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

        Serialize();

        ReceiveThread = new Thread(Receiver);
        ReceiveThread.Start();

        GameObject.Find("Join Chat").SetActive(false);
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
        newSocket.SendTo(serializeStream.ToArray(), serializeStream.ToArray().Length, SocketFlags.None, server);
        serializeStream.Dispose();
    }

    private void Deserialize()
    {
        BinaryReader reader = new BinaryReader(deserializeStream);
        deserializeStream.Seek(0, SeekOrigin.Begin);
        hostUsername = reader.ReadString();
        deserializeStream.Dispose();
    }

    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize();
    }
}
