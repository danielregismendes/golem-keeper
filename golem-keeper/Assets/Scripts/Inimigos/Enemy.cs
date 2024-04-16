using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Enemy : MonoBehaviour {

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

	public float distanceAttackX = 1.5f;
    public float distanceAttackZ = 1.5f;


    void Start () {

		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		target = FindFirstObjectByType<Player>().transform;
		currentHealth = maxHealth;
        gameManager = FindFirstObjectByType<GameManager>();

    }
	

	void Update () {

            CheckGrounded();

			walkTimer += Time.deltaTime;
		
	}

	private void FixedUpdate()
	{

			if (!isDead)
			{
				Vector3 targetDitance = target.position - transform.position;
				float hForce = targetDitance.x / Mathf.Abs(targetDitance.x);

				if (walkTimer >= UnityEngine.Random.Range(1f, 2f))
				{
					zForce = targetDitance.z / Mathf.Abs(targetDitance.z);
                walkTimer = 0;
				}

				if (Mathf.Abs(targetDitance.x) < 1.5f)
				{
					hForce = 0;
				}

				if (!damaged)
					rb.velocity = new Vector3(hForce * currentSpeed, 0, zForce * currentSpeed);

				anim.SetFloat("Speed", Mathf.Abs(currentSpeed));

				if (Mathf.Abs(targetDitance.x) < distanceAttackX && Mathf.Abs(targetDitance.z) < distanceAttackZ && Time.time > nextAttack)
				{
					anim.SetTrigger("Attack");
					attack.SetEnemyAttack(enemyHit);
					nextAttack = Time.time + attackRate;
				}

            rb.position = new Vector3(rb.position.x, rb.position.y, rb.position.z);

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
