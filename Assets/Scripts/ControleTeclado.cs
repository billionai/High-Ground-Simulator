﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleTeclado : MonoBehaviour {

	private Rigidbody rb;
	private float timeToShoot;
	private float timeToNextBullet;
	private int bulletCount;
	private float timeToBomb;
	private float protectionCount;
	private Animator anim;
	private PlayerStatus stat;

	public float moveSpeed = 100f;
	public GameObject bullet;
	public GameObject bomb;
	public GameObject protEffect;
	public Vector3 bulletOffset;
	public Vector3 grenadeOffset;
	public Color bulletColor;
	public float shotDelay = 1f;
	public int bulletsToShoot = 3;
	public float bulletFrequency = 0.05f;
	public float bombDelay = 2f;
	public float protectionTime = 5f;
	public AudioSource shootSFX;
	public float velocidadeRotacao = 8f;
	
	void Start(){
		timeToShoot = 0;
		timeToBomb = 0;
		protectionCount = 0;
		rb = GetComponent<Rigidbody>();
		anim = GetComponentInChildren<Animator>();
		stat = GetComponent<PlayerStatus>();
	}
	
	// Update is called once per frame
	void Update () {

		// Mover personagem
		Move();
		
		// Virar personagem
		Turn();

		// Atirar
		Shoot();

		//Granadas
		Grenade();		

		Invulnerable();
	}

	// Mover personagem
	void Move(){
		var leftX = Input.GetAxis("Horizontal");
        var leftY = Input.GetAxis("Vertical");
		var movement = new Vector3(-leftX*moveSpeed, rb.velocity.y, -leftY*moveSpeed);
		rb.velocity = movement;
		//rb.MovePosition(rb.position + movement*moveSpeed*Time.deltaTime);
		anim.SetFloat("Speed", movement.magnitude/moveSpeed);
	}

	// Virar personagem
	void Turn(){
		var rZ = Input.GetAxis("Horizontal_2");
        var rX = Input.GetAxis("Vertical_2");
		var direction = new Vector3(-rZ, 0, -rX);
		if(direction != Vector3.zero){
			//rb.transform.forward = direction;
		
			//Código adaptado de https://forum.unity.com/threads/smooth-look-at.26141/
			var targetRotation = Quaternion.LookRotation(direction);
			// Smoothly rotate towards the target point.
			rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, targetRotation, velocidadeRotacao * Time.deltaTime);
		}
	}

	// Atirar com a arma
	void Shoot(){
		// Tempo entre tiros
		if(timeToShoot > 0){
			timeToShoot -= Time.deltaTime;
			timeToNextBullet -= Time.deltaTime;
			if(timeToNextBullet < 0 && bulletCount > 0){
				shootSFX.Play();
				var shot = Instantiate(bullet, transform.position, transform.rotation);
				shot.transform.Translate(bulletOffset);
				shot.GetComponent<Bullet>().SetParentName(stat.name);
				shot.GetComponentInChildren<MeshRenderer>().material.color = bulletColor;
				shot.GetComponent<Bullet>().stat = stat;
				bulletCount--;
				timeToNextBullet = bulletFrequency;
			}
		}

		// Atirar
		if(Input.GetButtonDown("Fire1") && timeToShoot <= 0){
			shootSFX.Play();
			var shot = Instantiate(bullet, transform.position, transform.rotation);
			shot.transform.Translate(bulletOffset);
			shot.GetComponent<Bullet>().SetParentName(stat.name);
			shot.GetComponentInChildren<MeshRenderer>().material.color = bulletColor;
			shot.GetComponent<Bullet>().stat = stat;
			timeToShoot = shotDelay;
			timeToNextBullet = bulletFrequency;
			bulletCount = bulletsToShoot-1;
		}
	}

	// Arremessar Granada
	void Grenade(){
		if(timeToBomb > 0){
			timeToBomb -= Time.deltaTime;
			stat.UpdateColorPercentage((bombDelay-timeToBomb)/bombDelay);
		}

		if(Input.GetButtonDown("Fire2") && timeToBomb <= 0){
			var shot = Instantiate(bomb, transform.position, transform.rotation);
			shot.transform.Translate(grenadeOffset);
			shot.GetComponent<BulletBomb>().SetParentName(this.gameObject.name);
			timeToBomb = bombDelay;
			stat.UpdateColorPercentage((bombDelay-timeToBomb)/bombDelay);
		}	
	}

	// Invulnerabilidade
	void Invulnerable(){
		if (protectionCount > 0){
			protectionCount -= Time.deltaTime;
			if (protectionCount <= 0){
				stat.isProtected = false;
			}
		}

		if (Input.GetButtonDown("Jump") && stat.useProtection()){
			protectionCount = protectionTime;
			var effect = Instantiate(protEffect,transform.position,transform.rotation);
			effect.transform.SetParent(this.transform);
		}
	}
}
