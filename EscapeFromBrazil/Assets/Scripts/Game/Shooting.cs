using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    [SerializeField] 
    private float bulletForce = 20f;

    private PlayerMovement player;

    public bool shootP= false;
    private bool shootE= false;

    private bool canShoot = true;

    [SerializeField]
    public float defaultFireCooldown = 0.5f;
    public float actualFireCooldown;

    //private float fireRate = 3;

    private Transform enemyGun;

    private State state;

    private GameObject trash;
    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        enemyGun = GameObject.Find("Enemy").gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform;
        trash = GameObject.Find("Trash");
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        switch (state)
        {
            case State.NONE:
                break;
            case State.LOBBY:
                break;
            case State.GAME:
                if ((Input.GetKey(KeyCode.Mouse0) && !shootP) && canShoot)
                {
                    StartCoroutine(Shoot());
                }
                if (shootE)
                {
                    GameObject bullet = Instantiate(bulletPrefab, enemyGun.position, enemyGun.rotation);
                    bullet.GetComponent<Rigidbody>().AddForce(enemyGun.up * bulletForce, ForceMode.Impulse);
                    bullet.transform.parent = trash.transform;
                    shootE = false;
                }
                return;
            default:
                break;
        }
        state = player.GetState();
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        shootP = true;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.up*bulletForce,ForceMode.Impulse);
        bullet.transform.parent = trash.transform;
        // Start Delay
        StartCoroutine(FireRateHandler());
        yield return null;
    }

    public void ShootEnemy()
    {
        shootE = true;
    }
    IEnumerator FireRateHandler()
    {
        //float timeToNextShot = 1 / fireRate;
        //yield return new WaitForSeconds(fireRate);
        yield return new WaitForSeconds(actualFireCooldown);
        canShoot = true;
    }

    public void PowerUpFireRate()
    {
        actualFireCooldown *= 0.5f;
    }

    public void ResetFireRate()
    {
        actualFireCooldown = defaultFireCooldown;
    }
}
