using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    Collider2D attackCol;
    public float attackDam = 5f;
    public string attackID;
    public Vector2 knockback = Vector2.zero;

	private void Awake()
    {
        attackCol = GetComponent<Collider2D>();
		if (string.IsNullOrEmpty(attackID))
		{
			attackID = System.Guid.NewGuid().ToString(); // Tạo một ID duy nhất
		}
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damagable damagable = collision.GetComponent<Damagable>();
        if (damagable != null) 
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
            bool gotHit = damagable.Hit(attackDam, deliveredKnockback);
            if(gotHit) 
                Debug.Log(collision.name + " hit for " + attackDam); 

        }
    }
}
