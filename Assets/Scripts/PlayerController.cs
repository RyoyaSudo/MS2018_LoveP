using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed;

	private Rigidbody rb;
	private int count;

	void Start(){
		rb = GetComponent<Rigidbody> ();

	}

	// Update is called once per frame
	void FixedUpdate () {
		float MoveH = Input.GetAxis ("Horizontal");
		float MoveV = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3 (MoveH, 0.0f, MoveV);

		rb.AddForce (movement * speed);
	}
}