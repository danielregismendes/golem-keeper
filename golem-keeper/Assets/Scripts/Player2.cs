using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTeste : MonoBehaviour
{
    public float currentSpeed, maxSpeed, arrasto, rotationSpeed, jumpForce;
    public LayerMask layerGround;
    public Rigidbody rb;
    public Transform groundCheck;

    private GameObject cam;
    private bool direita, esquerda, frente, atras;
    private bool grounded, jump;


    private void Start()
    {
 
        cam = GameObject.FindGameObjectWithTag("MainCamera");

    }

    private void Update()
    {

        CheckInputs();
        Velocidade();
        Arrasto();
        CheckGrounded();

    }

    private void FixedUpdate()
    {

        Movimentacao();
        Rotacao();

    }

    void CheckGrounded()
    {

        grounded = Physics.Raycast(groundCheck.transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f, layerGround);

    }

    void Velocidade()
    {

        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if(horizontalVelocity.magnitude > maxSpeed)
        {

            Vector3 limitedVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);

        }

    }

    void Movimentacao()
    {

        Quaternion dir = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);

        if (esquerda)
        {
            rb.AddForce(dir * Vector3.left * currentSpeed);
            esquerda = false;
        }
        if (frente)
        {
            rb.AddForce(dir * Vector3.forward * currentSpeed);
            frente = false;
        }
        if (atras)
        {
            rb.AddForce(dir * Vector3.back * currentSpeed);
            atras = false;
        }
        if (direita)
        {
            rb.AddForce(dir * Vector3.right * currentSpeed);
            direita = false;
        }

        if(jump && grounded)
        {
            transform.position += Vector3.up * .1f;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jump = false;
        }

    }

    void Arrasto()
    {

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z) / (1 + arrasto / 100) + new Vector3(0, rb.velocity.y, 0);

    }

    void Rotacao()
    {

        if((new Vector2(rb.velocity.x, rb.velocity.z)).magnitude > .1f)
        {
            Vector3 horizontalDir = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            Quaternion rotation = Quaternion.LookRotation(horizontalDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed);
        }

    }

    void CheckInputs()
    {

        if (Input.GetKey(KeyCode.A))
            esquerda = true;
        if (Input.GetKey(KeyCode.W))
            frente = true;
        if (Input.GetKey(KeyCode.S))
            atras = true;
        if (Input.GetKey(KeyCode.D))
            direita = true;
        if(Input.GetKeyDown(KeyCode.Space) && grounded)
            jump = true;


    }

}
