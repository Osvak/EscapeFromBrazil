using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float bulletLifespan = 3f;
    private void Start()
    {
        Invoke("DestroyBullet", bulletLifespan);
    }
    // Start is called before the first frame update
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

}
