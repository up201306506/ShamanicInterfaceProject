using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class ImageFader : MonoBehaviour {

	Image img;
	float fadetime = 2f;

	// Use this for initialization
	void Start () {
		img = this.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(img.color.a > 0){
			var tempColor = img.color;
			tempColor.a -= Time.deltaTime/fadetime;
			if(tempColor.a < 0)
				tempColor.a = 0;
			img.color = tempColor;
		}

	}

	public void FadeImageIn(){
		var tempColor = img.color;
		tempColor.a = 1f;
		img.color = tempColor;
	}
}
