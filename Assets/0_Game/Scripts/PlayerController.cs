using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
	private float runSpeed;
	public float airWalkSpeed = 3f;
	public float jumpImpulse = 10f;

    [SerializeField]
    private int maxJumpCount = 1; // Số lần nhảy tối đa (double jump)
    private int jumpCount = 0;

    [SerializeField] private float coyoteTimeDuration = 0.2f;
    private float coyoteTimeCounter;

    public float JumpStamina = 3f;
	public float AttackStamina = 2f;

	public float ManaForU = 20f;
	public float ManaForI = 3f;
	public float ManaForO = 40f;

	TouchingDirection touchingDirection;
    Rigidbody2D rb;
    Animator animator;
    Damagable damagable;
	Attack attack;
	StaminaManage stamina;
	ManaManage mana;
	ExpManage exp;
	SkillManage skill;

	private Vector3 respawnPoint;
	private Vector3 checkpoint;

	public dialogueManager dialogueManager;
	public List<Dialogue> bossDialogue;
	public float CurrentMoveSpeed
	{
		get
		{
			if (CanMove)
			{
				if (IsMoving && !touchingDirection.IsOnWall)
				{
					if (touchingDirection.IsGrounded)
					{
						if (isRunning)
						{
							runSpeed = (float)(speed *  1.5);
							return runSpeed;
						}
						else
						{
							return speed;
						}
					}
					else
					{
						return airWalkSpeed;
					}
				}
				else
				{
					return 0;
				}
			}
			else
			{
				return 0;
			}
			
		}
	}

	Vector2 moveInput;

	[SerializeField]
	private bool _isMoving = false;
	public bool IsMoving { get
		{
			return _isMoving;
		}private set 
		{ 
			_isMoving = value;
			animator.SetBool(AnimationStrings.isMoving, value);
		} 
	}

	[SerializeField]
	private bool _isRunning = false;
	public bool isRunning
	{
		get
		{
			return _isRunning;
		}
		private set
		{
			_isRunning = value;
			animator.SetBool(AnimationStrings.isRunning, value);
		}
	}

	public float RunSpeed { get => runSpeed; set => runSpeed = value; }
			
	public bool _isFacingRight = true;
	public bool IsFacingRight { get { return _isFacingRight; } private set {
			if (_isFacingRight != value)
			{
				transform.localScale *= new Vector2(-1, 1);
			}
			_isFacingRight = value;
		} }
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
	public bool IsAlive
	{
		get
		{
			return animator.GetBool(AnimationStrings.isAlive);
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		touchingDirection = GetComponent<TouchingDirection>();
		damagable = GetComponent<Damagable>();
		stamina = GetComponent<StaminaManage>();
		mana = GetComponent<ManaManage>();
		exp = GetComponent<ExpManage>();	
		skill = GetComponent<SkillManage>();
		respawnPoint = transform.position;

		PlayerData.instance.LoadData();
		
		LoadAttackData();
		LoadHealthData();
	}
	
	private void FixedUpdate()
	{
		if(!damagable.LockVelocity)
			rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed  , rb.velocity.y );
		animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y );

        if (touchingDirection.IsGrounded)
        {
            jumpCount = 0; // Đặt lại số lần nhảy khi chạm đất
			coyoteTimeCounter = coyoteTimeDuration;
		}
		else
		{
			coyoteTimeCounter -= Time.deltaTime;
		}

        // Giảm stamina khi chạy
        if (isRunning && IsMoving && !stamina.UseStamina(stamina.staminaCostPerSecondRunning * Time.fixedDeltaTime))
		{
			isRunning = false; // Dừng chạy nếu không còn stamina
			stamina.SetRunning(false);
		}
        
    }
	public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

		if (IsAlive)
		{
			IsMoving = moveInput != Vector2.zero;
			stamina.SetMoving(IsMoving);
			if (isRunning && !IsMoving)
			{
				// Nếu Shift đang nhấn nhưng không di chuyển, hãy tiếp tục hồi phục stamina
				stamina.SetRunning(false); // Bạn có thể cập nhật trạng thái ở đây nếu cần
			}
			Debug.Log("Move Input: " + moveInput); // Kiểm tra xem input có nhận được hay không
			SetFacingDir(moveInput);
		}
		else
		{
			IsMoving = false;
			stamina.SetMoving(IsMoving);
		}
		
	}

	private void SetFacingDir(Vector2 moveInput)
	{
		if(moveInput.x >0 && !IsFacingRight)
		{
			IsFacingRight = true;
		}
		else if(moveInput.x < 0 && IsFacingRight)
		{
			IsFacingRight = false;
		}
	}

	public void OnRun(InputAction.CallbackContext context)
	{
		
		if (context.started)
		{
			isRunning = true;
			stamina.SetRunning(isRunning);

		}
		else if(context.canceled)
        {
			isRunning = false;
			stamina.SetRunning(isRunning);
        }
	}
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && (jumpCount < maxJumpCount || coyoteTimeCounter > 0f ) && stamina.UseStamina(JumpStamina)) // Khi giữ phím
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            jumpCount++;
			coyoteTimeCounter = 0;
        }
        else if (context.canceled) // Khi thả phím
        {
            rb.velocity = new Vector2(rb.velocity.x,rb.velocity.y * 0.5f);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
	{
		if (context.started && stamina.UseStamina(AttackStamina))
		{
			animator.SetTrigger(AnimationStrings.attackTrigger);
			stamina.SetAttacking(true);
		}
		stamina.SetAttacking(false);
	}

	public void OnRangeAtatck(InputAction.CallbackContext context)
	{

		if (context.started  && !skill.isRangeAttackCooldown && mana.UseMana(ManaForI))
		{
			animator.SetTrigger(AnimationStrings.rangeAttackTrigger);
			StartCoroutine(skill.ActiveRangeAttack());
		}

    }
	public void OnBuffAttack(InputAction.CallbackContext context)
	{

		if (context.started && !skill.isBuffOnCooldown && mana.UseMana(ManaForU))
		{
			animator.SetTrigger(AnimationStrings.buffAttackTrigger);
			StartCoroutine(skill.ActivateBuff());
		}
	}
	public void OnBuffForAll(InputAction.CallbackContext context)
	{
		if (context.started && !skill.isbuffForAllCooldown && mana.UseMana(ManaForO))
		{
			animator.SetTrigger(AnimationStrings.buffTrigger);
			StartCoroutine(skill.ActiveBuffForAll());
		}
	}
	
	public void OnHit(float damage,Vector2 knockback)
	{
		rb.velocity = new Vector2(knockback.x,rb.velocity.y + knockback.y);
	}
	public void IncreaseAttack(float amount)
	{
		Attack[] attackComponents = GetComponentsInChildren<Attack>();
		foreach (Attack attack in attackComponents)
		{
			attack.attackDam += amount; // Tăng damage
			PlayerPrefs.SetFloat($"attackDam_{attack.attackID}", attack.attackDam); // Lưu với ID
		}

		PlayerPrefs.SetInt("AttackCount", attackComponents.Length); // Lưu số lượng Attack components
		PlayerPrefs.Save();
	}
	public void SaveHealthData()
	{
		PlayerPrefs.SetFloat("MaxHealth",damagable.MaxHealth);
		PlayerPrefs.SetFloat("Health", damagable.Health);
		PlayerPrefs.Save();
	}
	public void LoadHealthData()
	{
		damagable.MaxHealth = PlayerPrefs.GetFloat("MaxHealth", 100f);
		damagable.Health = PlayerPrefs.GetFloat("MaxHealth", 100f);
	}
	public void LoadAttackData()
	{
		Attack[] attackComponents = FindObjectsOfType<Attack>();

		foreach (Attack attack in attackComponents)
		{
			if (PlayerPrefs.HasKey($"attackDam_{attack.attackID}"))
			{
				attack.attackDam = PlayerPrefs.GetFloat($"attackDam_{attack.attackID}");
			}
		}

	}
	public void SetCheckpoint(Vector3 checkpointPosition)
	{
		checkpoint = checkpointPosition;
	
	}
	public void AddHealth(float amount)
	{
		damagable.AddHeal(amount);
		SaveHealthData();
	}

	public void Respawn()
	{
		transform.position = checkpoint != Vector3.zero ? checkpoint : respawnPoint;
		damagable.Respawn();
		RestoreStats();
		HealBossOnRespawn();
	}
	private void RestoreStats()
	{
		 stamina.currentStamina = stamina.maxStamina;
		 mana.currentMana = mana.maxMana;
		// attackDamage = originalAttackDamage;
	}
	private void HealBossOnRespawn()
	{
		// Tìm boss trong scene và gọi phương thức hồi máu
		BossController bossController = FindObjectOfType<BossController>();
		Boss2Controller boss2Controller = FindObjectOfType<Boss2Controller>();
		Boss3Controller boss3Controller = FindObjectOfType<Boss3Controller>();
		Boss4Controller boss4Controller = FindObjectOfType<Boss4Controller>();
		if (bossController != null)
		{
			bossController.HealBoss();  // Hồi máu đầy cho boss
		}else if(boss2Controller != null)
		{
			boss2Controller.HealBoss();
		}else if(boss3Controller != null)
		{
			boss3Controller.HealBoss();
		}else if(boss4Controller != null)
		{
			boss4Controller.HealBoss();
		}
	}

}
