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
                if (Input.GetKeyDown(KeyCode.Mouse0) && !shootP)
                {
                    Shoot();
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

    private void Shoot()
    {
        shootP = true;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.up*bulletForce,ForceMode.Impulse);
        bullet.transform.parent = trash.transform;
    }

    public void ShootEnemy()
    {
        shootE = true;
    }
}
