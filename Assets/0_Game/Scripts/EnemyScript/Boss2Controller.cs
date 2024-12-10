using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Controller : MonoBehaviour
{
	public float speed = 2f;
	public DetectionZone detectionZone;
	public DetectionZone hitBoxdetectionZone;
	public DetectionZone arrowDetectionZone;
	public float chaseSpeed = 4f;
	public float rangeAttackCooldown = 2f; // Thời gian cooldown mặc định cho RangeAttack
	private float rangeAttackCooldownTimer = 0f; // Bộ đếm thời gian cho RangeAttack

	//Nghỉ sau khi di chuyển vài giây
	public float restTime = 2f;
	private float restCooldown = 0f;
	private float moveCooldown = 0f;
	private bool isResting = false;
	private float moveDuration = 5f;

	public List<Transform> patrolPoints;
	public float waypointReachedDistance = 0.5f;
	public int expReward = 100;

	private Animator animator;
	private Rigidbody2D rb;
	private Damagable damagable;
	private ExpManage expManage;
	private PlayerController playerController;
	public GameObject finishPointObject;
	public Dialogue dialogue;
	private dialogueManager manager;

	private Transform currentWaypoint;
	private int waypointIndex = 0;
	//private bool hasTarget = false;
	private bool isChasing = false;
	[SerializeField] private Transform target;

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
	[SerializeField]
	private bool _isMoving = true;
	public bool IsMoving
	{
		get
		{
			return _isMoving;
		}
		set
		{
			_isMoving = value;
			animator.SetBool(AnimationStrings.isMoving, value);
		}
	}
	private bool _canMove;
	public bool CanMove
	{
		get
		{
			return animator.GetBool(AnimationStrings.canMove);
		}
		set
		{
			_canMove = value;
			animator.SetBool(AnimationStrings.canMove, value);
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
		expManage = FindObjectOfType<ExpManage>();
		playerController = FindObjectOfType<PlayerController>();
		finishPointObject = FindObjectOfType<FinishPoint>().gameObject;

		finishPointObject.SetActive(false);
	}

	private void Start()
	{
		currentWaypoint = patrolPoints[waypointIndex];
		manager = FindObjectOfType<dialogueManager>();
	}

	private void OnEnable()
	{
		damagable.damagebleDeath.AddListener(OnDeath);
	}
	private void Update()
	{
		HasTarget = hitBoxdetectionZone.detectedColliders.Count > 0 && playerController.IsAlive == true;
		if(rangeAttackCooldownTimer > 0)
		{
			rangeAttackCooldownTimer -= Time.deltaTime;
		}

		// Kiểm tra nếu có kẻ thù trong phạm vi tầm xa (arrowDetectionZone)
		if (arrowDetectionZone.detectedColliders.Count > 0 && rangeAttackCooldownTimer <= 0)
		{
			// Nếu có kẻ thù và cooldown đã kết thúc, thực hiện tấn công từ xa
			RangeAttack();
		}

		if (AttackCooldown > 0)
		{
			AttackCooldown -= Time.deltaTime;
		}

		if (detectionZone.detectedColliders.Count > 0)
		{
			target = detectionZone.detectedColliders[0].transform;
			isChasing = true;
		}
		else
		{
			isChasing = false;
			target = null;
		}
	}
	private void FixedUpdate()
	{
		if (damagable.IsAlive)
		{
			if (CanMove)
			{
				if (isChasing && target != null)
				{
					ChaseTarget();
				}
				else
				{
					Patrol();
				}
			}
			else
			{
				Attack();
			}
		}
	}

	private void Patrol()
	{
		if (isResting)
		{
			animator.SetBool(AnimationStrings.isMoving, false);
			// Nếu boss đang nghỉ, chỉ cần giảm thời gian nghỉ
			restCooldown -= Time.deltaTime;

			if (restCooldown <= 0f)
			{
				// Sau khi hết thời gian nghỉ, tiếp tục tuần tra
				isResting = false;
				animator.SetBool(AnimationStrings.isMoving, true);
				moveCooldown = 0f;  // Reset thời gian di chuyển
			}
			else
			{
				// Boss đang nghỉ, giữ nguyên vận tốc
				rb.velocity = Vector2.zero;
				return;
			}
		}
		Vector2 directionToWaypoint = new Vector2((currentWaypoint.position.x - transform.position.x), 0).normalized;
		float distance = Mathf.Abs(currentWaypoint.position.x - transform.position.x);

		rb.velocity = new Vector2(directionToWaypoint.x * speed, rb.velocity.y);
		moveCooldown += Time.deltaTime;
		UpdateDirection(directionToWaypoint.x);

		// Nếu đã di chuyển đủ lâu, cho boss nghỉ
		if (moveCooldown >= moveDuration)
		{
			isResting = true;
			restCooldown = restTime; // Đặt thời gian nghỉ
			moveCooldown = 0f; // Reset thời gian di chuyển
		}

		if (distance <= waypointReachedDistance)
		{
			waypointIndex = (waypointIndex + 1) % patrolPoints.Count;
			currentWaypoint = patrolPoints[waypointIndex];
		}
	}
	private void ChaseTarget()
	{
		if (isResting)
		{
			// Nếu boss đang nghỉ, giữ nguyên vận tốc
			animator.SetBool(AnimationStrings.isMoving, false);
			restCooldown -= Time.deltaTime;

			if (restCooldown <= 0f)
			{
				// Sau khi hết thời gian nghỉ, tiếp tục đuổi theo mục tiêu
				isResting = false;
				animator.SetBool(AnimationStrings.isMoving, true);
				moveCooldown = 0f;
			}
			else
			{
				// Boss đang nghỉ, giữ nguyên vận tốc
				rb.velocity = Vector2.zero;
				return;
			}
		}
		if (target != null)
		{
			Vector2 direction = (target.position - transform.position).normalized;
			rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);

			UpdateDirection(direction.x);

			moveCooldown += Time.deltaTime;

			if (moveCooldown >= moveDuration)
			{
				isResting = true;
				restCooldown = restTime; // Đặt thời gian nghỉ
				moveCooldown = 0f; // Reset thời gian di chuyển
			}

		}

	}
	private void UpdateDirection(float direction)
	{
		if (Mathf.Sign(direction) != Mathf.Sign(transform.localScale.x))  // Kiểm tra nếu hướng di chuyển thay đổi
		{
			Vector3 localScale = transform.localScale;
			localScale.x *= -1;  // Đảo chiều
			transform.localScale = localScale;
		}
	}

	private void Attack()
	{
		animator.SetTrigger(AnimationStrings.attackTrigger); // Trigger combo attack animation
	}
	private void RangeAttack()
	{
		animator.SetTrigger(AnimationStrings.rangeAttackTrigger);

		// Reset cooldown
		rangeAttackCooldownTimer = rangeAttackCooldown;
	}

	private bool isDead = false;
	public void HealBoss()
	{
		if (damagable != null && damagable.Health < damagable.MaxHealth)
		{
			damagable.Health = damagable.MaxHealth;  // Hồi máu đầy cho boss
		}
	}
	public void OnDeath()
	{
		if (isDead) return;
		isDead = true;
		expManage.AddExp(expReward);
		Debug.Log("Player gained " + expReward + " EXP from Boss");
		finishPointObject.SetActive(true);
		manager.StartDialogue(dialogue);
	}
}
