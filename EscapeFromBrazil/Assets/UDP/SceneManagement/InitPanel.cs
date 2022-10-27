using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject JoinChat;
    [SerializeField]
    private GameObject Chat;

    // Start is called before the first frame update
    void Start()
    {
        JoinChat.SetActive(true);    
        Chat.SetActive(false);    
    }
}