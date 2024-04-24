using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Enemy : MonoBehaviour {

    enum estadoDaAI
    {
        patrulha, seguindo, ataque, procurandoAlvoPerdido
    };

    estadoDaAI estadoAI;

    public Transform[] rota;
	public bool useRota = true;
    public FOVEnemys cabeca;
    public int stopDistancePatrol;
    public int stopDistancePlayer;

    public float velocidade;
    public float aceleracao;
    public float velInvestida;
    public float aceInvestida;
    public float timerAtaque;
    public float maxSpeed;
	public float damageTime = 0.5f;
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

    private float distancePlayer;
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
        velocidade = navMesh.speed;
        aceleracao = navMesh.acceleration;


    }
	

	void Update () 
	{
        anim.SetFloat("Velocidade", navMesh.velocity.magnitude);

        distancePlayer = Vector3.Distance(transform.position, target.position);
               
        //CheckGrounded();
        /*
        Debug.Log("Estado: " + estadoAI);
        Debug.Log("Distancia Restante: " + navMesh.remainingDistance);
        Debug.Log("Distancia Alvo: " + navMesh.stoppingDistance);
        */

        AiPatrol();

    }

	private void FixedUpdate()
	{

        

    }

	void AiPatrol()
	{
        
        if (cabeca)
        {
            switch (estadoAI)
            {
                case estadoDaAI.patrulha:
                    navMesh.stoppingDistance = stopDistancePatrol;
                    if (useRota && navMesh.remainingDistance < stopDistancePatrol + 1)
                    {
                        navMesh.stoppingDistance = stopDistancePatrol;
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
                    navMesh.stoppingDistance = stopDistancePlayer;
                    navMesh.SetDestination(alvo.position);
                    LookTargert(alvo.transform);
                    if (navMesh.remainingDistance < stopDistancePlayer + 1)
                    {
                        ultimaPosicConhecida = alvo.position;
                        LookTargert(alvo.transform);
                        estadoAI = estadoDaAI.ataque;
                    }
                    if (!cabeca.inimigosVisiveis.Contains(alvo))
                    {
                        ultimaPosicConhecida = alvo.position;
                        LookTargert(alvo.transform);
                        estadoAI = estadoDaAI.procurandoAlvoPerdido;
                    }
                    break;

                case estadoDaAI.ataque:
                    AtaqueBoi();
                    break;

                case estadoDaAI.procurandoAlvoPerdido:
                    navMesh.stoppingDistance = stopDistancePlayer;
                    navMesh.SetDestination(ultimaPosicConhecida);
                    timerProcura += Time.deltaTime;
                    if (timerAtaque < timerProcura)
                    {
                        timerProcura = 0;
                        estadoAI = estadoDaAI.patrulha;
                        break;
                    }
                    if (cabeca.inimigosVisiveis.Count > 0)
                    {
                        alvo = cabeca.inimigosVisiveis[0];
                        ultimaPosicConhecida = alvo.position;
                        LookTargert(alvo.transform);
                        estadoAI = estadoDaAI.seguindo;
                    }
                    break;
            }
        }

    }

    void AtaqueBoi()
    {
        navMesh.stoppingDistance = stopDistancePatrol;
        navMesh.speed = velInvestida;
        navMesh.acceleration = aceInvestida;

        LookTargert(alvo.transform);

        if (navMesh.remainingDistance > stopDistancePatrol) navMesh.SetDestination(ultimaPosicConhecida);

        if (navMesh.remainingDistance <= stopDistancePatrol + 1)
        {
            if(timerProcura == 0)
            {
                anim.SetTrigger("Ataque");
                attack.SetEnemyAttack(enemyHit);
            }

            timerProcura += Time.deltaTime;
            if (timerAtaque < timerProcura)
            {                
                timerProcura = 0;
                navMesh.speed = velocidade;
                navMesh.acceleration = aceleracao;
                estadoAI = estadoDaAI.patrulha;
                               
            }
        }            

    }



    void LookTargert(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * navMesh.angularSpeed);
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
			//anim.SetTrigger("HitDamage");
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
        public float knockback;
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
