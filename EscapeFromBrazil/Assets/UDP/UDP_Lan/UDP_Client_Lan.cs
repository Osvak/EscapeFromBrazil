using System.Collections;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using System.Threading;

public class UDP_Client_Lan : MonoBehaviour
{

    Socket newSocket;
    IPEndPoint ipep;
    EndPoint Server;

    Thread CurrentThread;
    Thread ReceiveThread;

    public TMP_InputField userNameText;
    public TMP_InputField IpServerText;

    //Panels
    [SerializeField]
    private GameObject joinChatPanel;
    [SerializeField]
    private GameObject theChatPanel;
    [SerializeField]
    private GameObject OnlineChat;

    public TMP_InputField messageField;

    bool openChat = false;
    bool updateText = false;

    private string userName, newMessage;

    // Start is called before the first frame update
    void Start()
    {
        newMessage = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (openChat)
        {
            OpenChat();
        }
        if(updateText){
            UpdateText();
        }

    }

    private void UpdateText(){

        Debug.Log("Modified text");

        OnlineChat.GetComponent<TextMeshProUGUI>().text += newMessage;

        updateText = false;
    }

    public void EnterServer()
    {
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, 6799); // IPAddress.Any, puerto del cliente
        newSocket.Bind(ipep);
        Server = new IPEndPoint(IPAddress.Parse(IpServerText.text), 8000);
        newSocket.Connect(Server);

        userName = userNameText.text;

        byte[] data = Encoding.ASCII.GetBytes(userName);

        newSocket.SendTo(data, data.Length, SocketFlags.None, Server);

        ReceiveThread = new Thread(Receiver);
        ReceiveThread.Start();
    }

    private void OpenChat()
    {
        joinChatPanel.SetActive(false);
        theChatPanel.SetActive(true);
        ReceiveThread.Abort();
        CurrentThread = new Thread(InChat);
        CurrentThread.Start();
        openChat = false;
    }

    private void Receiver()
    {
        byte[] recieve = new byte[255];
        int rev = newSocket.Receive(recieve);
        openChat = true;
    }

    public void SendButton()
    {
        if(messageField.text == "") return;

        newMessage = "\n[" + userName + "]:" + messageField.text;

        messageField.text = "";

        Debug.Log("Message has been sent");
        
        byte[] data = Encoding.ASCII.GetBytes(newMessage);

        newSocket.SendTo(data, data.Length, SocketFlags.None, Server);
    }

    private void InChat()
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
