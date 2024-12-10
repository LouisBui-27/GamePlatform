using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Controller : MonoBehaviour
{
    public float speed = 2f;
	public DetectionZone detectionZone;
	public DetectionZone hitBoxdetectionZone;
	public float chaseSpeed = 4f;
	

	//Nghỉ sau khi di chuyển vài giây
	public float restTime = 2f;
	private float restCooldown = 0f;
	private float moveCooldown = 0f;
	private bool isResting = false;
	private float moveDuration = 5f;

	private float lastHealthCheck; // Điểm máu cuối cùng để theo dõi thay đổi
	private const float HEALTH_THRESHOLD_PERCENT = 0.15f; // 15% máu tối đa
	private const float DAMAGE_BUFF_PERCENT = 0.20f; // 20% sát thương

	public List<Transform> patrolPoints;
	public float waypointReachedDistance = 0.5f;
	public int expReward = 100;

	private Animator animator;
	private Rigidbody2D rb;
	private Damagable damagable;
	private ExpManage expManage;
	private PlayerController playerController;
	private Attack attack;
	public GameObject finishPointObject;
	private dialogueManager manager;
	public Dialogue dialogue;


	private Transform currentWaypoint;
	private int waypointIndex = 0;
	private bool hasTarget = false;
	//[SerializeField] private bool isBuffed = false;
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
		attack = GetComponentInChildren<Attack>();
		finishPointObject = FindObjectOfType<FinishPoint>().gameObject;
		manager = FindObjectOfType<dialogueManager>();

		finishPointObject.SetActive(false);
	}

	private void Start()
	{
		currentWaypoint = patrolPoints[waypointIndex];
		lastHealthCheck= damagable.MaxHealth;
	}

	private void OnEnable()
	{
		damagable.damagebleDeath.AddListener(OnDeath);
	}
	private void Update()
	{
		HasTarget = hitBoxdetectionZone.detectedColliders.Count > 0 && playerController.IsAlive == true;
		

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
		CheckHealthAndBuff();
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
	private void CheckHealthAndBuff()
	{
		if (damagable != null)
		{
			float currentHealth = damagable.Health;
			float maxHealth = damagable.MaxHealth;

			// Kiểm tra nếu Boss đã mất thêm 15% máu
			if (lastHealthCheck- currentHealth >= maxHealth * HEALTH_THRESHOLD_PERCENT)
			{
				lastHealthCheck= currentHealth; // Cập nhật điểm máu tiếp theo
				animator.SetTrigger(AnimationStrings.buffAttackTrigger);
				attack.attackDam *= (1 + DAMAGE_BUFF_PERCENT);
				
			}
		}
	}
	public void HealBoss()
	{
		if (damagable != null && damagable.Health < damagable.MaxHealth)
		{
			damagable.Health = damagable.MaxHealth;  // Hồi máu đầy cho boss
		}
	}

	private bool isDead = false;
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
