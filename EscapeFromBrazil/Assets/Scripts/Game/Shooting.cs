using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    [SerializeField] private float bulletForce = 20f;

    public bool shootP= false;
    public bool shootE= false;

    private Transform enemyGun;

    private void Awake()
    {
        enemyGun = GameObject.Find("Enemy").gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
        if (shootE)
        {
            GameObject bullet = Instantiate(bulletPrefab, enemyGun.position, enemyGun.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(enemyGun.up * bulletForce, ForceMode.Impulse);
            shootE = false;
        }

    }

    private void Shoot()
    {
        shootP = true;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.up*bulletForce,ForceMode.Impulse);
    }

    public void ShootEnemy()
    {
        shootE = true;
    }
}
