using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public TMP_Text tagName;

    [SerializeField]
    private GameManager gameManager;

    Rigidbody rb;
    private bool UP, DOWN, RIGHT, LEFT;
    private Vector3 input;
    
    public float speed = 15.0f;
    float MaxLenght = 0f;
    //-------------

    private Vector3 startPos;

    [SerializeField]
    private GameObject gunPivot;

    public bool hit = false;
    private State state;

    private Shooting shooting;

    private void Awake()
    {
        state = State.NONE;
        Application.targetFrameRate = 60;
        startPos = transform.position;
        shooting = this.GetComponent<Shooting>();
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {

        switch (state)
        {
            case State.NONE:
                break;
            case State.LOBBY:
                break;
            case State.GAME:
                Move();
                RotateGun();
                break;
            default:
                break;
        }
    }

    private void RotateGun()
    {
        //Aim player at mouse
        //which direction is up
        Vector3 upAxis = new Vector3(0, 1, 0);
        Vector3 mouseScreenPosition = Input.mousePosition;
        //set mouses z to your targets
        //Vector3 mouseWorldSpace = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            gunPivot.transform.LookAt(hit.point, upAxis);
        }
        //zero out all rotations except the axis I want
        gunPivot.transform.rotation = Quaternion.Euler(0, gunPivot.transform.eulerAngles.y, 0);
    }

    private void Move()
    {
        UP = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        DOWN = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        RIGHT = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        LEFT = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);

        //Reset Pos
        if(Input.GetKey(KeyCode.R))
        {
            rb.gameObject.transform.position = new Vector3(0f, 0f, 0f);
        }

        //Move Diagonal
        if (UP && RIGHT)
        {
            input.z = speed * (Mathf.Sqrt(2) / 2);
            input.x = speed * (Mathf.Sqrt(2) / 2);
        }
        if (UP && LEFT)
        {
            input.z = speed * (Mathf.Sqrt(2) / 2);
            input.x = -speed * (Mathf.Sqrt(2) / 2);
        }
        if (DOWN && RIGHT)
        {
            input.z = -speed * (Mathf.Sqrt(2) / 2);
            input.x = speed * (Mathf.Sqrt(2) / 2);
        }
        if (DOWN && LEFT)
        {
            input.z = -speed * (Mathf.Sqrt(2) / 2);
            input.x = -speed * (Mathf.Sqrt(2) / 2);
        }

        //Move Hor/Ver
        if (UP && !RIGHT && !LEFT)
            input.z = speed;

        if (DOWN && !RIGHT && !LEFT)
            input.z = -speed;

        if (RIGHT && !UP && !DOWN)
            input.x = speed;

        if (LEFT && !UP && !DOWN)
            input.x = -speed;


        rb.AddForce(Vector3.right * 1000 * input.x);
        rb.AddForce(Vector3.forward * 1000 * input.z);
        
        if (input.x != 0 && input.z != 0)
            MaxLenght = 1f;
        else
            MaxLenght = 0f;

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxLenght);

        input.x = input.z = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet") && !hit)
        {
            gameManager.HitPlayer();
            other.gameObject.GetComponent<BulletController>().DestroyBullet();
            hit = true;
        }
    }

    public void SetState(State newState)
    {
        state = newState;
    }

    public State GetState()
    {
        return state;
    }

    public void PlayerReset()
    {
        transform.position = startPos;
        shooting.ResetFireRate();
    }

    public void PowerUpFireRatePlayer()
    {
        shooting.PowerUpFireRate();
    }
}
