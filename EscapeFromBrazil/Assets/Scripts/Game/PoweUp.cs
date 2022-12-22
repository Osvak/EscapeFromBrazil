using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoweUp : MonoBehaviour
{

    private PowerUpManager powerUpManager;

    public int type = 0;
    private bool delete = false;
    private int ID;
    [Header("\nPowerUp list:\nLive + 1\nFire rate")]
    public List<Sprite> sprites = new List<Sprite> ();
    public void StartSpawn(Vector3 pos, int _type,int id)
    {
        powerUpManager = this.GetComponentInParent<PowerUpManager>();
        this.GetComponent<SpriteRenderer>().sprite = sprites[_type - 1];
        transform.position = pos;
        type = _type;
        ID = id;

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
