using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;
using System.Threading;

public class UDP_Server_Lan : MonoBehaviour
{

    Socket newSocket;
    IPEndPoint ipep;
    EndPoint Client;

    public TMP_InputField userNameText;

    [SerializeField]
    private GameObject joinChatPanel;
    [SerializeField]
    private GameObject theChatPanel;
    [SerializeField]
    private GameObject OnlineChat;

    public TMP_InputField message;

    bool updateText;

    private string userName, newMessage;

    void Update()
    {
        if (updateText)
        {
            UpdateText();
        }
    }

    private void UpdateText()
    {
        Debug.Log("Modified text");

        byte[] data = new byte[255];

        data = Encoding.ASCII.GetBytes(newMessage);

        newSocket.SendTo(data, data.Length, SocketFlags.None, Client);

        OnlineChat.GetComponent<TextMeshProUGUI>().text += newMessage;

        updateText = false;
    }

    public void SendButton()
    {
        if(message.text == "") return;
        
        newMessage = "\n[" + userName + "]:" + message.text;

        message.text = "";

        updateText = true;
    }

    public void CreateServer()
    {
        Socketing();

        userName = userNameText.text;
        //Open Chat
        joinChatPanel.SetActive(false);
        theChatPanel.SetActive(true);

    }

    private void Socketing()
    {
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 8000); // un puerto para el host, IPAddress.Any
        newSocket.Bind(ipep);

        // inicializar Client
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        Client = (EndPoint)(sender);

        Thread thread = new Thread(ReceieveClients);

        thread.Start();
    }

    private void ReceieveClients()
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

            bool newClient = true;

            for (int i = 0; i < newMessage.Length; i++)
            {
                if (newMessage[i] == '\n')
                {
                    newClient = false;
                    break;
                }
            }

            if (newClient)
            {
                Debug.Log("Receives a user");

                byte[] invitation;
                invitation = Encoding.ASCII.GetBytes("Can Join");
                newSocket.SendTo(invitation, invitation.Length, SocketFlags.None, Client);

                newMessage = "\n>> " + newMessage + " joined the chat";

            }

            updateText = true;

        }
    }

}
