using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonActions : MonoBehaviour
{
    public void UDPClient()
    {
        SceneManager.LoadScene("UDP_Join Game");
    }

    public void UDPServer()
    {
        SceneManager.LoadScene("UDP_Create Game");
    }

    public void TCPServer()
    {
        SceneManager.LoadScene("TCP_Create Game");
    }

    public void TCPClient()
    {
        SceneManager.LoadScene("TCP_Join Game");
    }

    public void BackSelector()
    {
        SceneManager.LoadScene("Selector");
    }

}
