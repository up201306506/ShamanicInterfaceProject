using UnityEngine;
using System.Collections;

public class SpinInPlace : MonoBehaviour {

	public float speed = 50f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(transform.position, transform.up, Time.deltaTime * speed);
	}
}
