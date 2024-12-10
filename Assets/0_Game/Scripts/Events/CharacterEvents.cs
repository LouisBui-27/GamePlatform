using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class CharacterEvents
{
	public static UnityAction<GameObject, float> characterDamaged;
	public static UnityAction<GameObject, float> characterHealed;
	
	public static UnityAction<GameObject, float> staminaHealed;
	public static UnityAction<GameObject, float, float> staminaChanged;

	public static UnityAction<GameObject, float> manaUsed;
	public static UnityAction<GameObject, float , float> manaChanged;
	
	public static UnityAction<GameObject, int> expUsed;
	public static UnityAction<GameObject, int , int> expChanged;


}

