using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EFBPlayerController : MonoBehaviour
{

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletDir;
    [SerializeField] private Transform trash;
    [SerializeField] private Camera cam;
    [SerializeField] private float movementVel = 2f;
    private EFBActions controls;
    private bool canShoot = true;
    [SerializeField] LayerMask ignore;


    private void Awake()
    {
        controls = new EFBActions();
        ignore = LayerMask.NameToLayer("Player");
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

    void Update()
    {
        PlayerAim();
        PlayerMove();
    }

    private void PlayerAim()
    {
        Vector3 mouseScreenPos = controls.Player.MousePosition.ReadValue<Vector2>();
        mouseScreenPos.z = 100f;
        mouseScreenPos = cam.ScreenToWorldPoint(mouseScreenPos);
        Ray ray = cam.ScreenPointToRay(new Vector3(controls.Player.MousePosition.ReadValue<Vector2>().x, controls.Player.MousePosition.ReadValue<Vector2>().y, 100f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, ~ignore))
        {
            transform.LookAt(new Vector3(hit.point.x, 1.31f, hit.point.z));
        }

        
    }

    private void PlayerMove()
    {
        Vector3 movement = new Vector3(controls.Player.Movement.ReadValue<Vector2>().x, 0f, controls.Player.Movement.ReadValue<Vector2>().y) * movementVel;
        transform.position += movement * Time.deltaTime;
    }

    private void PlayerShoot()
    {
        if (!canShoot) return;;
        GameObject g = Instantiate(bullet, bulletDir.position, bulletDir.rotation, trash);
        g.SetActive(true);
        StartCoroutine(CanShoot());
    }

    IEnumerator CanShoot()
    {
        canShoot = false;
            yield return new WaitForSeconds(.5f);
        canShoot = true;
    }
}
