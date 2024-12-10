﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			PlayerController player = collision.GetComponent<PlayerController>();
			if (player != null)
			{
				player.SetCheckpoint(transform.position);
			}
		}
	}
}