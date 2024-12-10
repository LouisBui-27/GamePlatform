using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{
    public UnityEvent<float, Vector2> damagebleHit;
    public UnityEvent damagebleDeath;
    public UnityEvent<float, float> healthChange;
    private Collider2D Collider2D;
    Rigidbody2D rigidbody2D;


	Animator animator;
    [SerializeField]
    private float _maxHealth = 100;
    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }
    [SerializeField]
    private float _health = 100;

    public float Health
    {
        get
        {
            return _health;
        }
        set 
        { 
            _health = value;
            healthChange?.Invoke(_health, MaxHealth);
            if(_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

	[SerializeField] private bool _isAlive = true;
	[SerializeField] private bool isInvincible = false;
	[SerializeField] private float timeSinceHit = 0;
	[SerializeField] private float invincibilityTimer = 0.25f;
 

	public bool IsAlive { 
        get 
        {
            return _isAlive;   
        }  
        set {
            _isAlive = value; 
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set " +value);

            if (value == false)
            {
                damagebleDeath.Invoke();
				PlayerController playerController = GetComponent<PlayerController>();
				if (playerController != null)
				{
					StartCoroutine(WaitAndRespawn(2f));
				}

			}
        } 
    }
	public bool LockVelocity
	{
		get
		{
			return animator.GetBool(AnimationStrings.lockVelocity);
		}
		set
		{
			animator.SetBool(AnimationStrings.lockVelocity, value);
		}

	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
        Collider2D = GetComponent<Collider2D>();
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
        if (isInvincible)
        {
            if(timeSinceHit > invincibilityTimer)
            {
                isInvincible=false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }
       
	}
	public bool Hit(float damage, Vector2 knockback)
    {
        if(IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            animator.SetTrigger(AnimationStrings.hit);
            LockVelocity = true;
            damagebleHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged.Invoke(gameObject,damage);

            return true;
        }
        return false;
    }

    public bool Heal(float healthRestore)
    {
        if (IsAlive && Health < MaxHealth)
        {
            float maxHeal = Mathf.Max(MaxHealth - Health, 0);
            float actualHeal = Mathf.Min(maxHeal, healthRestore);
			Health += actualHeal;
            CharacterEvents.characterHealed(gameObject, actualHeal);
            return true;
        }
        return false;
    }
    public void AddHeal(float amount)
    {
        MaxHealth += amount;
        Health = MaxHealth;
        healthChange?.Invoke(Health, MaxHealth);
    }
    public void Respawn()
    {
        Health = MaxHealth;
        IsAlive = true;
        animator.SetTrigger(AnimationStrings.RespawnTrigger);
    }
	private IEnumerator WaitAndRespawn(float waitTime)
	{
		// Chờ trong một khoảng thời gian
		yield return new WaitForSeconds(waitTime);

		// Sau khi chờ xong, gọi Respawn
		PlayerController playerController = GetComponent<PlayerController>();
		if (playerController != null)
		{
			playerController.Respawn();
		}
	}
}
