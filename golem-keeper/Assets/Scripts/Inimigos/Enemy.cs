using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Enemy : MonoBehaviour {

    enum estadoDaAI
    {
        patrulha, seguindo, procurandoAlvoPerdido
    };

    estadoDaAI estadoAI;

    public Transform[] rota;
	public bool useRota = true;
    public FOVEnemys cabeca;

    public float maxSpeed;
	public float damageTime = 0.5f;
	public int damage;
	public int maxHealth;
	public float attackRate = 1f;
	public AudioClip collisionSound, deathSound;
	public EnemyHit enemyHit;
    public Attack attack;
    public float maxSlope;
    public LayerMask layerGround;
    public Transform groundCheck;
    public int currentHealth;

	private NavMeshAgent navMesh;
	private Transform alvo;
    private Vector3 posicInicialDaAI;
    private Vector3 ultimaPosicConhecida;
    private float timerProcura;


    private float currentSpeed;
	private Rigidbody rb;
	protected Animator anim;
	private bool grounded;
	protected bool facingRight = false;
	private Transform target;
	protected bool isDead = false;
	private float zForce;
	private float walkTimer;
	private bool damaged = false;
	private float damageTimer;
	private float nextAttack;
	private AudioSource audioS;
	private GameManager	gameManager;
	private float distanceAttackX = 1.5f;
    private float distanceAttackZ = 1.5f;


    void Start () {

        navMesh = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		target = FindFirstObjectByType<Player>().transform;
		currentHealth = maxHealth;
        gameManager = FindFirstObjectByType<GameManager>();
        alvo = null;
        ultimaPosicConhecida = Vector3.zero;
        estadoAI = estadoDaAI.patrulha;
        posicInicialDaAI = transform.position;
        timerProcura = 0;

    }
	

	void Update () 
	{

		//CheckGrounded();
				
	}

	private void FixedUpdate()
	{

		AiPatrol();
					
	}

	void AiPatrol()
	{
        
        if (cabeca)
        {
            switch (estadoAI)
            {
                case estadoDaAI.patrulha:
                    if (useRota && navMesh.remainingDistance < 1)
                    {
                        navMesh.SetDestination(rota[UnityEngine.Random.Range(0, rota.Length - 1)].position);
                    }
                    if (cabeca.inimigosVisiveis.Count > 0)
                    {
                        alvo = cabeca.inimigosVisiveis[0];
                        ultimaPosicConhecida = alvo.position;
                        estadoAI = estadoDaAI.seguindo;
                    }
                    break;
                case estadoDaAI.seguindo:
                    navMesh.SetDestination(alvo.position);
                    if (!cabeca.inimigosVisiveis.Contains(alvo))
                    {
                        ultimaPosicConhecida = alvo.position;
                        estadoAI = estadoDaAI.procurandoAlvoPerdido;
                    }
                    break;
                case estadoDaAI.procurandoAlvoPerdido:
                    navMesh.SetDestination(ultimaPosicConhecida);
                    timerProcura += Time.deltaTime;
                    if (timerProcura > 5)
                    {
                        timerProcura = 0;
                        estadoAI = estadoDaAI.patrulha;
                        break;
                    }
                    if (cabeca.inimigosVisiveis.Count > 0)
                    {
                        alvo = cabeca.inimigosVisiveis[0];
                        ultimaPosicConhecida = alvo.position;
                        estadoAI = estadoDaAI.seguindo;
                    }
                    break;
            }
        }

    }

    void CheckGrounded()
    {

        RaycastHit hit;
        if (Physics.Raycast(groundCheck.transform.position + Vector3.up * .1f, Vector3.down, out hit, Mathf.Infinity, layerGround))
        {
            float slopeAngle = Mathf.Deg2Rad * Vector3.Angle(Vector3.up, hit.normal);
            float sec = 1 / Mathf.Cos(slopeAngle);
            float yDiff = .5f * sec - .5f;

            if ((groundCheck.transform.position.y - yDiff) - hit.point.y < .05f)
            {
                if (Vector3.Angle(Vector3.up, hit.normal) <= maxSlope)
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

    public void TookDamage(int damage)
	{
		if (!isDead)
		{
			damaged = true;
			currentHealth -= damage;
			anim.SetTrigger("HitDamage");
			if(currentHealth <= 0)
			{
				isDead = true;
				rb.AddRelativeForce(new Vector3(3, 5, 0), ForceMode.Impulse);
				DisableEnemy();

            }
		}
	}
	
	public void DisableEnemy()
	{
		Destroy(gameObject);
	}

	void ResetSpeed()
	{
		currentSpeed = maxSpeed;
	}
    void ZeroSpeed()
    {
        currentSpeed = 0;
    }

    [Serializable]
    public class EnemyHit
	{
		public int damage = 1;
		public AudioClip collisionSound;
    }

    void PlayEnemyHit(EnemyHit enemyHit)
    {
        attack.SetEnemyAttack(enemyHit);
    }

	public int GetHealth()
	{
		return currentHealth;
	}

	public bool GetIsDead()
	{
		return isDead;

    }

}
