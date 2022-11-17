using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameSetup;
    public GameObject cam;

    public string enemyUsername;
    public Vector3 enemyPosition;

    private GameObject Enemy;
    // Start is called before the first frame update
    void Start()
    {
        gameSetup.SetActive(true);
        cam.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        cam.transform.position = new Vector3(0, 100f, 0);
        Enemy = GameObject.Find("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        // Espacio para updatear al enemigo, de este modo separamos player de enemigo y podemos manejar correctamente los dos sin pisar el código del servidor.
        Debug.Log(enemyUsername);
        Debug.Log("Enemy Position: " + enemyPosition);

        Enemy.transform.position = enemyPosition;
    }
}
