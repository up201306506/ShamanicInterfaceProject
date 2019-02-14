using UnityEngine;
using System.Collections;

public class SpriteFader : MonoBehaviour {

	SpriteRenderer img;
	float fadetime = 5f;
	public bool fade = false;

	// Use this for initialization
	void Start () {
		img = this.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(fade){
			if(img.color.a < 1){
				var tempColor = img.color;
				tempColor.a += Time.deltaTime/fadetime;
				if(tempColor.a > 1)
					tempColor.a = 1;
				img.color = tempColor;
			}
		}
		

	}

	public void FadeSpriteIn(){
		fade = true;
	}
}
