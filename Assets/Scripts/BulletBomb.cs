﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBomb : MonoBehaviour {

	private Rigidbody rb;
	private GameObject detect;
	private string parentName;
	public float speed = 300f;
	public float verticalSpeed = 5f;
	public float gravity = 20f;
	public float size = 10f;
	public float lifeTime = 10f;
	public GameObject bomb;
	public GameObject bulletDetector;
	
	void Start(){
		rb = GetComponent<Rigidbody>();
		transform.localScale = new Vector3(size,size,size);
		//detect = Instantiate(bulletDetector, transform.position, transform.rotation, transform);
		//detect.transform.parent = this.gameObject.transform;
		rb.velocity = new Vector3(0f,verticalSpeed,0f) + transform.forward*speed;
	}

	// Mover a bala
	void Update () {
		if(lifeTime < 0) Destroy(this.gameObject);
		lifeTime -= Time.deltaTime;

		// Mover a bala em direção e velocidade constante com aplicação de gravidade
		//verticalSpeed -= gravity*Time.deltaTime;
		//rb.MovePosition(transform.position + transform.forward * Time.deltaTime * speed + 
		//(new Vector3(0,verticalSpeed,0)));
		//detect.transform.position = this.transform.position;
	}

	// Colisão
	void OnTriggerEnter(Collider other){
		if(other.gameObject.name != parentName && other.gameObject.tag != "Detector" && other.gameObject.tag != "Region"
												 && other.gameObject.tag != "MapLimit" && other.gameObject.tag != "Turret"){
			Explode();
		}
	}

	public void Explode(){
		speed = 0f;
		Instantiate(bomb, transform.position, Quaternion.identity);
		Destroy(this.gameObject);
	}

	public void SetParentName(string name){
		parentName = name;
	}
}
