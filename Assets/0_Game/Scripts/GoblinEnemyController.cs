using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection))]
public class GoblinEnemyController : MonoBehaviour
{
	public float walkAcceleration = 3f;
	public float maxSpeed = 3f;
	public float walkStopRate = 0.6f;
	public DetectionZone attackZone;
	public DetectionZone cliffDetectionZone;

	Rigidbody2D rb;
	TouchingDirection touchingDirection;
	Damagable damagable;
	Animator animator;
	ExpManage ExpManage;
	PlayerController playerController;
	public int ExpReward = 30;
	public enum WalkableDirection { Right, Left }

	private WalkableDirection _walkDirecttion;
	private Vector2 walkDirectionVector = Vector2.right;

	public WalkableDirection WalkDirection
	{
		get { return _walkDirecttion; }
		set
		{
			if (_walkDirecttion != value)
			{
				gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
				if (value == WalkableDirection.Right)
				{
					walkDirectionVector = Vector2.right;
				}
				else if (value == WalkableDirection.Left)
				{
					walkDirectionVector = Vector2.left;
				}
			}
			_walkDirecttion = value;
		}

	}

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
		rb = GetComponent<Rigidbody2D>();
		touchingDirection = GetComponent<TouchingDirection>();
		animator = GetComponent<Animator>();
		damagable = GetComponent<Damagable>();
		ExpManage = FindObjectOfType<ExpManage>();
		playerController = FindObjectOfType<PlayerController>();
	}

	private void Update()
	{
		HasTarget = attackZone.detectedColliders.Count > 0 && playerController.IsAlive == true;
		if (AttackCooldown > 0)
		{
			AttackCooldown -= Time.deltaTime;
		}

	}
	private void FixedUpdate()
	{
		if (touchingDirection.IsGrounded && touchingDirection.IsOnWall)
		{
			FlipDir();
		}

		if (!damagable.LockVelocity)
		{
			if (CanMove && touchingDirection.IsGrounded)
				rb.velocity = new Vector2(
					Mathf.Clamp(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed),
					rb.velocity.y);
			else
				rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
		}

	}

	private void FlipDir()
	{
		if (WalkDirection == WalkableDirection.Right)
		{
			WalkDirection = WalkableDirection.Left;
		}
		else if (WalkDirection == WalkableDirection.Left)
		{
			WalkDirection = WalkableDirection.Right;
		}
		else
		{
			Debug.Log("CURRENT WALKDIRERCTION IS NOT SET TO LEGAL VALUE (RIGHT OR LEFT)");
		}
	}
	public void OnHit(float damage, Vector2 knockback)
	{

		rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
	}
	public void OnCliffDetected()
	{
		if (touchingDirection.IsGrounded)
		{
			FlipDir();
		}
	}
	private bool isDead = false;
	public void OnDeath()
	{
		if (isDead) return;
		isDead = true;
		if (ExpManage != null)
		{
			ExpManage.AddExp(ExpReward);
			Debug.Log("Đã nhận được " + ExpReward);
		}
	}
}