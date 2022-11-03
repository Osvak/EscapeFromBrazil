using System.Collections;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

public class ServerCube : MonoBehaviour
{
    Socket newSocket;
    IPEndPoint ipep;
    EndPoint Client;

    bool updateText;

    string newMessage,message;
    // Start is called before the first frame update
    void Start()
    {

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 8000); // un puerto para el host, IPAddress.Any
        newSocket.Bind(ipep);

        // inicializar Client
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        Client = (EndPoint)(sender);

        Thread thread = new Thread(Reciver);

        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateText)
        {
            Debug.Log("Modified text");

            Debug.Log(message);

            byte[] data = new byte[255];

            data = Encoding.ASCII.GetBytes(message);

            newSocket.SendTo(data, data.Length, SocketFlags.None, Client);

            updateText = false;

        }
    }

    private void Reciver()
    {
        while (true)
        {
            byte[] data = new byte[255];
            int recv = newSocket.ReceiveFrom(data, ref Client);

            string str = Encoding.ASCII.GetString(data);

            newMessage = "";

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != 0) { newMessage += str[i]; }
                else break;

            }

            Debug.Log("Receives a user");

            byte[] invitation;
            invitation = Encoding.ASCII.GetBytes("Can Join");
            newSocket.SendTo(invitation, invitation.Length, SocketFlags.None, Client);

            message = ">> " + newMessage;

            updateText = true;

        }
    }
}
