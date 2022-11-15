using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameSetup;
    public GameObject cam;
    // Start is called before the first frame update
    void Start()
    {
        gameSetup.SetActive(true);
        cam.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        cam.transform.position = new Vector3(0, 450, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
