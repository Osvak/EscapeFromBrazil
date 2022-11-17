using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    private bool UP, DOWN, RIGHT, LEFT;
    private Vector3 input;
    
    public float speed = 15.0f;
    float MaxLenght = 0f;
    //-------------

    [SerializeField]
    private GameObject gunPivot;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    // Cread aquí lo que necesitéis para que el player se mueva
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        Move();
        RotateGun();

    }

    private void RotateGun()
    {
        //Aim player at mouse
        //which direction is up
        Vector3 upAxis = new Vector3(0, 1, 0);
        Vector3 mouseScreenPosition = Input.mousePosition;
        //set mouses z to your targets
        Vector3 mouseWorldSpace = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        gunPivot.transform.LookAt(mouseWorldSpace, upAxis);
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


}
