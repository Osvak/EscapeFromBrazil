using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (BGAudio.Instance != null)
        {
            BGAudio.Instance.gameObject.GetComponent<AudioSource>().Stop();
        }
    }
}
