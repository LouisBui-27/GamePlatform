using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowEnemy : MonoBehaviour
{
	public DetectionZone detectionZone;

	Rigidbody2D rb;
	Damagable damagable;
	Animator animator;
	ExpManage ExpManage;
	PlayerController playerController;
	public int ExpReward = 30;

	

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
		animator = GetComponent<Animator>();
		damagable = GetComponent<Damagable>();
		ExpManage = FindObjectOfType<ExpManage>();
		playerController = FindObjectOfType<PlayerController>();
	}

	private void Update()
	{
		HasTarget = detectionZone.detectedColliders.Count > 0 && playerController.IsAlive == true;
		
		if (AttackCooldown > 0)
		{
			AttackCooldown -= Time.deltaTime;
		}
		if (HasTarget)
		{
			FlipDir();
		}

	}
	
	private void FlipDir()
	{
		// Xác định hướng nhân vật
		Vector3 directionToPlayer = playerController.transform.position - transform.position;

		// Kiểm tra hướng và lật hình
		if (directionToPlayer.x > 0 && transform.localScale.x < 0)
		{
			// Quay mặt sang phải
			transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
		}
		else if (directionToPlayer.x < 0 && transform.localScale.x > 0)
		{
			// Quay mặt sang trái
			transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
		}
	}
	public void OnHit(float damage, Vector2 knockback)
	{

		rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
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
