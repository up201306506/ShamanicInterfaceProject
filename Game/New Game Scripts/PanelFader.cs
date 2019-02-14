using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class PanelFader : MonoBehaviour {

	public bool endTheGame = false;

	Image img;
	Text tex;

	bool activated = false;
	float timer = 0;

	// Use this for initialization
	void Start () {
		img = GetComponent<Image>();
		tex = this.transform.Find("Text").gameObject.GetComponent<Text>();

		var tempColor = img.color;
		tempColor.a = 0;
		img.color = tempColor;

		tempColor = tex.color;
		tempColor.a = 0;
		tex.color = tempColor;

	}
	
	// Update is called once per frame
	void Update () {
		if(activated){
			timer += Time.deltaTime/5f;

			if(img.color.a < 1){
				var tempColor = img.color;
				tempColor.a = timer;
				img.color = tempColor;
			}

			if(img.color.a < 1){
				var tempColor = tex.color;
				tempColor.a = timer;
				tex.color = tempColor;
			}
			
			if(timer > 2)
			{
				endTheGame = true;
			}
		}
	}


	public void endGame(){
		activated = true;
	}
}
