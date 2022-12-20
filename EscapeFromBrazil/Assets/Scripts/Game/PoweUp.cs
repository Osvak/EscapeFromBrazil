using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoweUp : MonoBehaviour
{

    private PowerUpManager powerUpManager;

    public int type = 0;
    private bool delete = false;
    private int ID;
    public void StartSpawn(Vector3 pos, int _type,int id)
    {
        powerUpManager = this.GetComponentInParent<PowerUpManager>();
        transform.position = pos;
        type = _type;
        ID = id;

    }

    public void SetType(int _type)
    {
        type = _type;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !delete)
        {
            delete = true;
            powerUpManager.ActivePowerUp((PowerUpManager.PuType)type,ID);
        }

    }

}
