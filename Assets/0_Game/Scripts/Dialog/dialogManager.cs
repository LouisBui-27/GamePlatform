using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dialogueManager : MonoBehaviour
{
	public Text nameText;
	public Text dialogueText;

	public Animator animator;
	private PlayerController playerController;
	private BossController bossController;
	private Boss2Controller boss2Controller;
	private Boss3Controller boss3Controller;
	private Boss4Controller boss4Controller;
	private Queue<string> sentences;
	private void Awake()
	{
		playerController = FindObjectOfType<PlayerController>();
		bossController = FindObjectOfType<BossController>();
		boss2Controller = FindObjectOfType<Boss2Controller>();
		boss3Controller = FindObjectOfType<Boss3Controller>();
		boss4Controller = FindObjectOfType<Boss4Controller>();
	}

	void Start()
	{
		sentences = new Queue<string>();
	}

	public void StartDialogue(Dialogue dialogue)
	{
		
		animator.SetBool("IsOpen", true);
		playerController.CanMove = false;
		if (bossController != null)
		{
			bossController.CanMove = false;
			bossController.IsMoving = false;
		}
		else if (boss2Controller != null)
		{
			boss2Controller.CanMove = false;
			boss2Controller.IsMoving = false;
		}
		else if (boss3Controller != null)
		{
			boss3Controller.CanMove = false;
			boss3Controller.IsMoving = false;
		}else if (boss4Controller != null)
		{
			boss4Controller.CanMove = false;
			boss4Controller.IsMoving = false;
		}
		

		nameText.text = dialogue.name;

		sentences.Clear();

		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}

		string sentence = sentences.Dequeue();
		StopAllCoroutines();
		
		StartCoroutine(TypeSentence(sentence));
		
	}

	IEnumerator TypeSentence(string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
	}

	void EndDialogue()
	{
		animator.SetBool("IsOpen", false);
		playerController.CanMove = true;
		if (bossController != null)
		{
			bossController.CanMove = true;
			bossController.IsMoving = true;
		}
		else if (boss2Controller != null)
		{
			boss2Controller.CanMove = true;
			boss2Controller.IsMoving = true;
		}
		else if (boss3Controller != null)
		{
			boss3Controller.CanMove = true;
			boss3Controller.IsMoving = true;
		}
		else if (boss4Controller != null)
		{
			boss4Controller.CanMove = true;
			boss4Controller.IsMoving = true;
		}
		
	}

}
