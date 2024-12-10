using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	public Dialogue dialogue;
	private dialogueManager manager;
	

	private void Start()
	{
		manager = FindObjectOfType<dialogueManager>();
		
		
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			manager.StartDialogue(dialogue); 
			gameObject.SetActive(false);
		}
	}
}
