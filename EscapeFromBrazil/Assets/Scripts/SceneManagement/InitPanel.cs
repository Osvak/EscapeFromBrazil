using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject JoinChat;

    // Start is called before the first frame update
    void Start()
    {
        JoinChat.SetActive(true);
    }
}