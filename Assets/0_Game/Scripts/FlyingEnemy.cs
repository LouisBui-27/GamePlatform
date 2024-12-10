using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
	public float speed = 2f;
	public DetectionZone biteDetectionZone;
	public Collider2D deathCol;
	public List<Transform> wayPoints;
	public float waypointReachedDistance = 0f;

	Animator animator;
	Rigidbody2D rb;
	Damagable damagable;
	ExpManage ExpManage;
	PlayerController playerController;
	public int ExpReward = 50;

	Transform nextWaypoint;
	int waypointNum = 0;

	public bool _hasTarget = false;
	public bool HasTarget
	{
		get { return _hasTarget; }
		private set
		{
			_hasTarget = value;
			animator.SetBool(AnimationStrings.hasTarget, value);
		}
	}

	public bool CanMove
	{
		get
		{
			return animator.GetBool(AnimationStrings.canMove);
		}
	}

	public float AttackCooldown
	{
		get
		{
			return animator.GetFloat(AnimationStrings.attackCooldown);
		}
		private set
		{
			animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
		}
	}
	private void Awake()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		damagable = GetComponent<Damagable>();
		ExpManage = FindObjectOfType<ExpManage>();
		playerController = FindObjectOfType<PlayerController>();
	}

	private void Start()
	{
		nextWaypoint = wayPoints[waypointNum];
	}

	private void OnEnable()
	{
		damagable.damagebleDeath.AddListener(OnDeath);
	}
	private void Update()
	{
		HasTarget = biteDetectionZone.detectedColliders.Count > 0 && playerController.IsAlive == true;
		if (AttackCooldown > 0)
		{
			AttackCooldown -= Time.deltaTime;
		}

	}
	private void FixedUpdate()
	{
		if (damagable.IsAlive)
		{
			if (CanMove)
			{
				Flight();
			}
			else
			{
				rb.velocity = Vector3.zero;
			}
		}
	}

	private void Flight()
	{
		Vector2 dircetionToWaypoint = (nextWaypoint.position - transform.position).normalized;
		
		float distance = Vector2.Distance(nextWaypoint.position, transform.position);
		
		rb.velocity = dircetionToWaypoint * speed;
		UpdateDir();
		if(distance <= waypointReachedDistance)
		{
			waypointNum++;

			if(waypointNum>= wayPoints.Count)
			{
				waypointNum = 0;
			}
			nextWaypoint = wayPoints[waypointNum];
		}
	}

	private void UpdateDir()
	{
		Vector3 localScale = transform.localScale;
		if(transform.localScale.x > 0)
		{
			if (rb.velocity.x < 0)
			{
				transform.localScale = new Vector3(-1 * localScale.x, localScale.y,localScale.z);
			}
		}
		else
		{
			if (rb.velocity.x > 0)
			{
				transform.localScale = new Vector3(-1 * localScale.x, localScale.y, localScale.z);
			}
		}
	}
	private bool isDead = false;
	public void OnDeath()
	{
		if (isDead) return;
		isDead = true;
		rb.gravityScale = 2f;
		rb.velocity = new Vector2(0, rb.velocity.y);
		deathCol.enabled = true;
	
		ExpManage.AddExp(ExpReward);
		Debug.Log("Đã nhận được " + ExpReward);
		
	}
}
