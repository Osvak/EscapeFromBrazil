using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonActions : MonoBehaviour
{
    public void UDPClient()
    {
        SceneManager.LoadScene("Client");
    }

    public void UDPServer()
    {
        SceneManager.LoadScene("Server");
    }

    public void BackSelector()
    {
        SceneManager.LoadScene("Selector");
    }

}
