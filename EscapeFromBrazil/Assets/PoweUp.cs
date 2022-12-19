using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoweUp : MonoBehaviour
{

    private PowerUpManager powerUpManager;

    public enum PuType
    {
        Live1,
        FiringRate
    } 

    public PuType type;
    private bool delete = false;


    PoweUp(PuType type)
    {
        this.type = type;
    }

    // Start is called before the first frame update
    void Start()
    {
        powerUpManager = this.GetComponentInParent<PowerUpManager>();
        type = (PuType)Random.Range(0, 2);

    }

    public void SetType(int _type)
    {
        type = (PuType)_type;
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !delete)
        {
            delete = true;
            powerUpManager.ActivePowerUp(type);
            Destroy(gameObject);
        }

    }

}
