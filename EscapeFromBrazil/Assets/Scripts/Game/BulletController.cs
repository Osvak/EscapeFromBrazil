using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float bulletLifespan = 3f;
    private void Start()
    {
        Invoke("DestroyBullet",1);
    }
    // Start is called before the first frame update
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
