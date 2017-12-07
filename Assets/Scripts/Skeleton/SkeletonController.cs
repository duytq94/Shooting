﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkeletonController : MonoBehaviour
{
	public float maxhealth = 50f;
	public float currenthealth = 50f;
	public float speed = 1f;
	public GameObject healthBar;
	public GameObject skeletonCanvas;

	private Animator animator;
	private GameObject knight;
	private bool isAllive = true;

	void Start ()
	{
		animator = GetComponent<Animator> ();
		knight = GameObject.FindGameObjectWithTag ("Knight");
		healthBar.GetComponent<Image> ().fillAmount = currenthealth / maxhealth;
	}

	void Update ()
	{
		if (currenthealth <= 0) {
			return;
		}

		Vector3 direction = knight.transform.position - this.transform.position;
		if (knight.GetComponent<KnightController> ().GetIsAllive ()) {
			direction.y = 0;
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), 0.1f);

			animator.SetBool ("isIdle", false);

			if (direction.magnitude > 2f) {
				if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Damage")) {
					this.transform.Translate (0, 0, speed / 10f);
				}
				animator.SetBool ("isWalking", true);
				animator.SetBool ("isAttacking", false);
			} else {
				animator.SetBool ("isAttacking", true);
				animator.SetBool ("isWalking", false);
				FindObjectOfType<AudioManager> ().PlayDelayed ("SlashEnemy", 1.2f);
			}

		} else {
			animator.SetBool ("isIdle", true);
			animator.SetBool ("isWalking", false);
			animator.SetBool ("isAttacking", false);
		}
	}

	public void BeAttack (float damAttack)
	{
		ProcessAttack (damAttack);
	}

	public void BeGunAttack (float damAttack)
	{
		ProcessAttack (damAttack);
		StartCoroutine (Wait ());
	}

	public void ProcessAttack (float damAttack)
	{
		currenthealth -= damAttack;
		healthBar.GetComponent<Image> ().fillAmount = currenthealth / maxhealth;
		animator.SetBool ("isDamage", true);
		if (currenthealth <= 0) {
			animator.SetBool ("isDead", true);
			skeletonCanvas.SetActive (false);

			isAllive = false;
			Destroy (gameObject, 4f);
		}
	}

	IEnumerator Wait ()
	{
		yield return new WaitForSeconds (0.25f);
		animator.SetBool ("isDamage", false);
	}

	public void ExitAttack ()
	{
		animator.SetBool ("isDamage", false);
	}

	public bool GetIsAllive ()
	{
		return isAllive;
	}
}
