using UnityEngine;
using System.Collections;

public class GateOpenClose : MonoBehaviour {

	GameObject leftDoor;
	GameObject rightDoor;

	Vector3 leftClosedPosition;
	Vector3 rightClosedPosition;
	Vector3 leftOpenPosition;
	Vector3 rightOpenPosition;

	float time = 1;
	float total = 1;

	public bool open = false;
	bool wasOpen = false;

	// Use this for initialization
	void Start () {
		leftDoor = this.transform.Find("Left Door").gameObject;
		rightDoor = this.transform.Find("RightDoor").gameObject;
		leftClosedPosition = leftDoor.transform.position;
		rightClosedPosition = rightDoor.transform.position;
		leftOpenPosition = leftDoor.transform.TransformPoint(Vector3.left*2);
		rightOpenPosition = rightDoor.transform.TransformPoint(Vector3.right*2);
	}
	
	// Update is called once per frame
	void Update () {
		
		if(wasOpen && !open){
			wasOpen = false;
			time = 0;
		} else if (!wasOpen && open){
			wasOpen = true;
			time = 0;
		}

		if(open){
			if(time < total){
				time += Time.deltaTime/total;
				leftDoor.transform.position = Vector3.Lerp(leftClosedPosition, leftOpenPosition, time);
				rightDoor.transform.position = Vector3.Lerp(rightClosedPosition, rightOpenPosition, time);
			}
		} else {
			if(time < total){
				time += Time.deltaTime/total;
				leftDoor.transform.position = Vector3.Lerp(leftOpenPosition, leftClosedPosition, time);
				rightDoor.transform.position = Vector3.Lerp(rightOpenPosition, rightClosedPosition, time);
			}
		}



	}

	public void OpenDoor(){
		time = 0;
		open = true;
	}

	public void CloseDoor(){
		time = 0;
		open = false;
	}
}
