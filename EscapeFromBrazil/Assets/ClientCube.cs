using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCube : MonoBehaviour
{
    public string Ip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            newMessage = "\n[" + userName + "]:" + messageField.text;

        messageField.text = "";

        Debug.Log("Message has been sent");
        
        byte[] data = Encoding.ASCII.GetBytes(newMessage);

        newSocket.SendTo(data, data.Length, SocketFlags.None, Server);
        }
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
