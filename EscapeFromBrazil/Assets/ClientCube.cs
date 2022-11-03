using System.Collections;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

public class ClientCube : MonoBehaviour
{
    Socket newSocket;
    IPEndPoint ipep;
    EndPoint Server;

    Thread CurrentThread;

    public string IP, message;
    private string newMessage;

    bool updateText = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)){
            startConexion();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("send:" + message);
        
            byte[] data = Encoding.ASCII.GetBytes(message);

            newSocket.SendTo(data, data.Length, SocketFlags.None, Server);
        }

        if (updateText)
        {

            Debug.Log("New Message: " + newMessage);

            updateText = false;

        }
    }

    private void startConexion(){
        Debug.Log("Conectado al servidor");

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 6799); // IPAddress.Any, puerto del cliente
        newSocket.Bind(ipep);
        Server = new IPEndPoint(IPAddress.Parse(IP), 8000);
        newSocket.Connect(Server);
        
        CurrentThread = new Thread(Conexion);
        CurrentThread.Start();
    }

    private void Conexion()
    {

        while (true)
        {

            byte[] data = new byte[255];

            int rev = newSocket.ReceiveFrom(data,ref Server);

            newMessage = "";

            string reciveMessage = Encoding.ASCII.GetString(data);

            for (int i = 0; i < reciveMessage.Length; i++)
            {
                if (reciveMessage[i] != 0) { newMessage += reciveMessage[i]; }
                else break;
            }

            updateText = true;
        }
    }
}
