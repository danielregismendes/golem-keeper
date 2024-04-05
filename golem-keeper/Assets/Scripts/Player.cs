using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxSpeed = 4;
    public float jumpForce = 400;
    public int maxHealth = 10;

    private int currentHealth;
    private float currentSpeed;
    private Rigidbody rb;
    private Animator anim;
    private Transform groundCheck;
    private bool onGround;
    private bool isDead = false;
    private bool Jump = false;
    private bool canJump = true;

    void Start()
    {
        groundCheck = gameObject.transform.Find("GroundCheck");
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        currentSpeed = maxSpeed;

    }

    void Update()
    {

        onGround = Physics.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButtonDown("Jump") && onGround && canJump)
        {

            Jump = true;

        } 

    }

    private void FixedUpdate()
    {

        if (!isDead)
        {
            float h = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            rb.velocity = new Vector3(h * currentSpeed, rb.velocity.y, z * currentSpeed);

            if (onGround)
                anim.SetFloat("Speed", Mathf.Abs(rb.velocity.magnitude));

           if (Jump)
            {
                Jump = false;
                rb.AddForce(Vector3.up * jumpForce);
            }
        }

        rb.position = new Vector3(rb.position.x, rb.position.y, rb.position.z);

    }

}
