using UnityEngine;
using System.Collections;

public class FlyRandomFlight : MonoBehaviour {

	Vector3 originalCenter;
	Vector3 previousPosition;
	Vector3 nextPosition;
	float currenttime;
	float curenttotal;
	
	float rangemin = 0.1f;
	float rangemax = 0.4f;

	// Use this for initialization
	void Start () {
		originalCenter = transform.position;
		previousPosition = transform.position;
		currenttime = 1f;
		curenttotal = 1f;
	
	}
	
	// Update is called once per frame
	void Update () {
		if(currenttime >= curenttotal){
			curenttotal = Random.Range(rangemin, rangemax);
			currenttime = 0;
			nextPosition = originalCenter +  new Vector3(Random.Range(-2f, 2f),Random.Range(-1f, 1f),Random.Range(-2f, 2f)); //Random.insideUnitSphere*2;
			previousPosition = transform.position;
			transform.LookAt(nextPosition);

		} else {
			currenttime += Time.deltaTime/curenttotal;
			transform.position = Vector3.Lerp(previousPosition, nextPosition, currenttime);
		}
	
	}

	public void Goaway(){
		originalCenter = originalCenter + new Vector3(0f, 30f, 0f);
		rangemin = 1f;
		rangemax = 2f;
	}
}
