using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EFBPlayerController : MonoBehaviour
{

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletDir;
    [SerializeField] private Transform trash;
    private EFBActions controls;

    private void Awake()
    {
        controls = new EFBActions();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    void Start()
    {
        controls.Player.Shoot.performed += ctx => PlayerShoot();
        controls.Player.Movement.performed += ctx => PlayerMove();
    }

    private void PlayerMove()
    {
        Vector2 movement = controls.Player.Movement.ReadValue<Vector2>();

    }

    private void PlayerShoot()
    {
        Vector2 mousePosition = controls.Player.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        GameObject g = Instantiate(bullet, bulletDir.position, bulletDir.rotation, trash);
        g.SetActive(true);
    }
}
