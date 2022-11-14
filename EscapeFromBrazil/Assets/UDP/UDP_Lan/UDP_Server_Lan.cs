using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;
using System.Threading;
using System.IO;

public class UDP_Server_Lan : MonoBehaviour
{ 
    Socket newSocket;
    IPEndPoint ipep;
    EndPoint client;

    public TMP_InputField userNameText;

    MemoryStream serializeStream;
    MemoryStream deserializeStream;

    private string username, clientUsername;

    private bool clientLogged = false;

    void Update()
    {
        if (clientLogged) StartCoroutine(SendInfo());
        Debug.Log(clientUsername);
    }

    public void CreateServer()
    {
        Socketing();

        username = userNameText.text;
        GameObject.Find("Join Chat").SetActive(false);
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
            if (!clientLogged) clientLogged = !clientLogged;
            deserializeStream = new MemoryStream(data);
            Deserialize();
        }
    }

    private void Serialize()
    {
        serializeStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(serializeStream);
        writer.Write(username);
        newSocket.SendTo(serializeStream.ToArray(), serializeStream.ToArray().Length, SocketFlags.None, client);
        serializeStream.Dispose();
    }

    private void Deserialize()
    {
        BinaryReader reader = new BinaryReader(deserializeStream);
        deserializeStream.Seek(0, SeekOrigin.Begin);
        clientUsername = reader.ReadString();
        deserializeStream.Dispose();
    }

    IEnumerator SendInfo()
    {
        yield return new WaitForSeconds(0.16f);
        Serialize();
    }
}
