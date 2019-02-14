using UnityEngine;
using System.Collections;

public class TurnMe : MonoBehaviour {

	Animator ani;
	public GameObject playerpos;

	// Use this for initialization
	void Start () {
		ani = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(ani.GetBool("TurnMe")){
			ani.SetBool("TurnMe", false);
			transform.LookAt(playerpos.transform.position);
		}
	}
}
