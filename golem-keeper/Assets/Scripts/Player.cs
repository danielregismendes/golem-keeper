using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public float currentSpeed, maxSpeed, arrasto, rotationSpeed, jumpForce;
    public LayerMask layerGround;
    public Rigidbody rb;
    public Transform groundCheck;
    public Transform interactor;
    public float maxSlope;
    public GameObject menuFarming;
    public int maxHealth;
    public string seedSelect = "";
    public Combo[] combos;
    public Attack attack;
    public List<string> currentCombo;

    private GameObject cam;
    private bool direita, esquerda, frente, atras;
    private bool grounded, jump;
    private Farming selectedLand = null;
    private GameManager gameManager;
    private int currentHealth;
    private bool canHit = true;
    private bool resetCombo;
    public UnityEvent OnStartCombo, OnFinishCombo;
    private Hit currentHit, nextHit;
    private bool startCombo;
    private float comboTimer;
    private Animator anim;
    private bool isDead;
    private bool canJump = true;

    private void Start()
    {
        currentHealth = maxHealth;
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        gameManager = FindFirstObjectByType<GameManager>();
        anim = GetComponent<Animator>();

    }

    private void Update()
    {

        CheckInputs();
        Arrasto();
        CheckGrounded();
        CheckFarmGround();
        CheckAttackInputs();

    }

    private void FixedUpdate()
    {

        Velocidade();
        Movimentacao();
        Rotacao();

    }

    void CheckGrounded()
    {

        RaycastHit hit;
        if (Physics.Raycast(groundCheck.transform.position + Vector3.up * .1f, Vector3.down, out hit, Mathf.Infinity, layerGround))
        {
            float slopeAngle = Mathf.Deg2Rad * Vector3.Angle(Vector3.up, hit.normal);
            float sec = 1 / Mathf.Cos(slopeAngle);
            float yDiff = .5f * sec - .5f;

            if((groundCheck.transform.position.y - yDiff) - hit.point.y < .05f)
            {
                if(Vector3.Angle(Vector3.up, hit.normal) <= maxSlope) 
                {
                    grounded = true;
                    rb.useGravity = false;
                    return;
                }
                rb.AddForce(Vector3.down * 300f);
            }
        }

        grounded = false;
        rb.useGravity = true;

    }

    void CheckFarmGround()
    {
        RaycastHit hit;
        Collider farmLand;

        if (Physics.Raycast(interactor.transform.position, Vector3.down, out hit, 2.0f))
        {
            farmLand = hit.collider;

            if (farmLand.tag == "FarmGround")
            {

                Farming farming = farmLand.GetComponent<Farming>();
                SelectedLand(farming);
                return;

            }

            if(selectedLand != null)
            {
                selectedLand.Select(false);
                selectedLand = null;
            }

        }

    }

    void SelectedLand(Farming farming)
    {

        if(selectedLand != null)
        {
            selectedLand.Select(false);
        }

        selectedLand = farming;
        farming.Select(true);

    }

    public void InteractFarm()
    {    
        if (selectedLand.state == FARMSTATE.VAZIO)
        {
            Debug.Log(seedSelect != "");

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            menuFarming.SetActive(true);
            Time.timeScale = 0;

            if (seedSelect != "")
            {
                selectedLand.Seeding(seedSelect);
                menuFarming.SetActive(false);
                seedSelect = "";
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
            }
        }
        if (selectedLand.state == FARMSTATE.COLHEITA)
        {

            gameManager.SetInventario(selectedLand.plantData.name, 1, 0);
            selectedLand.state = FARMSTATE.SECO;

        }

    }

    void Velocidade()
    {
        if (!grounded)
        {
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (horizontalVelocity.magnitude > maxSpeed)
            {

                Vector3 limitedVelocity = horizontalVelocity.normalized * maxSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);

            }
        }
        else
        {

            if (rb.velocity.magnitude > maxSpeed)
            {

                Vector3 limitedVelocity = rb.velocity.normalized * maxSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);


            }
        }

    }

    void Movimentacao()
    {

        if (esquerda)
        {
            MoveDir(Vector3.left);
            esquerda = false;
        }
        if (frente)
        {
            MoveDir(Vector3.forward);
            frente = false;
        }
        if (atras)
        {
            MoveDir(Vector3.back);
            atras = false;
        }
        if (direita)
        {
            MoveDir(Vector3.right);
            direita = false;
        }

        if(jump && grounded)
        {
            transform.position += Vector3.up * .5f;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jump = false;
        }

    }

    void Arrasto()
    {

        if(!grounded)
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z) / (1 + arrasto / 100) + new Vector3(0, rb.velocity.y, 0);
        else
            rb.velocity /= (1 + arrasto / 100);
    }

    void Rotacao()
    {
        Vector3 dir = rb.GetAccumulatedForce();
        if((new Vector2(dir.x, dir.z)).magnitude > .1f)
        {
            Vector3 horizontalDir = new Vector3(dir.x, 0, dir.z);
            Quaternion rotation = Quaternion.LookRotation(horizontalDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed);
        }

    }

    void MoveDir(Vector3 moveDir)
    {

        Quaternion dir = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);

        Vector3 planeNormal = Vector3.up;

        if(grounded)
            if (Physics.Raycast(groundCheck.transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerGround))
                planeNormal = hit.normal;

        Vector3 force = Vector3.ProjectOnPlane(dir * moveDir, planeNormal) * currentSpeed;

        rb.AddForce(force);

    }

    void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded && canJump)
            jump = true;
        if (Input.GetKey(KeyCode.A))
            esquerda = true;
        if (Input.GetKey(KeyCode.W))
            frente = true;
        if (Input.GetKey(KeyCode.S))
            atras = true;
        if (Input.GetKey(KeyCode.D))
            direita = true;        
        if (Input.GetKeyDown(KeyCode.E) && selectedLand)
            InteractFarm();

    }

    void CheckAttackInputs()
    {

        if (grounded)
        {
            if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")) && !canHit)
            {
                resetCombo = true;
            }

            for (int i = 0; i < combos.Length; i++)
            {
                if (combos[i].hits.Length > currentCombo.Count)
                {
                    if (Input.GetButtonDown(combos[i].hits[currentCombo.Count].inputButton))
                    {
                        if (currentCombo.Count == 0)
                        {
                            OnStartCombo.Invoke();
                            Debug.Log("Primeiro hit foi adicionado");
                            PlayHit(combos[i].hits[currentCombo.Count]);
                            break;
                        }
                        else
                        {
                            bool comboMatch = false;
                            for (int y = 0; y < currentCombo.Count; y++)
                            {
                                if (currentCombo[y] != combos[i].hits[y].inputButton)
                                {
                                    Debug.Log("Input não pertence ao hit atual");
                                    comboMatch = false;
                                    break;
                                }
                                else
                                {
                                    comboMatch = true;
                                }
                            }

                            if (comboMatch && canHit)
                            {
                                Debug.Log("Hit adicionado ao combo");
                                nextHit = combos[i].hits[currentCombo.Count];
                                canHit = false;
                                break;
                            }
                        }

                    }
                }


            }

            if (startCombo)
            {
                comboTimer += Time.deltaTime;
                if (comboTimer >= currentHit.animationTime && !canHit)
                {
                    PlayHit(nextHit);
                    if (resetCombo)
                    {
                        canHit = false;
                        CancelInvoke();
                        //Invoke("ResetCombo", currentHit.animationTime);
                        ResetCombo();
                    }
                }

                if (comboTimer >= currentHit.resetTime)
                {
                    ResetCombo();
                }

            }
        }
    }

    void PlayHit(Hit hit)
    {
        comboTimer = 0;
        attack.SetAttack(hit);
        anim.Play(hit.animation);
        startCombo = true;
        currentCombo.Add(hit.inputButton);
        currentHit = hit;
        canHit = true;
    }

    void ResetCombo()
    {
        resetCombo = false;
        OnFinishCombo.Invoke();
        startCombo = false;
        comboTimer = 0;
        currentCombo.Clear();
        anim.Rebind();
        canHit = true;
    }

    public void TookDamage(int damage)
    {

        if (!isDead)
        {
            currentHealth -= damage;
            anim.SetTrigger("HitDamage");
            if (currentHealth <= 0)
            {
                isDead = true;
            }
        }

    }

    public int GetHealth()
    {
        return currentHealth;
    }

    void ZeroSpeed()
    {

        currentSpeed = 0;
        canJump = false;
    }

    void resetSpeed()
    {

        currentSpeed = maxSpeed;
        canJump = true;
    }

}
