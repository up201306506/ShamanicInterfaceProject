using UnityEngine;
using System.Collections;

public class CinematicHandler : MonoBehaviour {

	//--------------------------
	//--- To add a new state:
	//----- Add to NewGameScript.GetState()
	//----- Add to NewGameScript the state itself.
	//----- Add a new setupObjectInitial
	//----- (optional) Add to setupObjectSteps()
	//----- Add a new playCinematic(string)
	//----- (optional) Add other functions required by CinematicResolution and setupObjectSteps.
	//----- Add to CinematicResolution()
	//----- Add case in InteractionSpace.actionPerformedPrimary()
	//----- (optional) Repeat everything and add case in InteractionSpace.actionPerformedSecondary()
	//--------------------------

	//Interaction Script will see this
	public bool cinematicOver = false;

	//Cinematic vars
	private bool playingCinematic = false;
	private float cinematicTime;
	private string cinematicType;

	//Auxiliars
	private bool cinematicBoolAux;
	private int cinematicStepAux;
	private Vector3 cinematicPositionAux;
	private bool cinematicEvent1 = false;
	private bool cinematicEvent2 = false;
	private bool cinematicEvent3 = false;

	//Objects
	private GameObject cinematicObject1;
	private GameObject cinematicObject2;
	private GameObject cinematicObject3;
	private GameObject cinematicObject4;

	void Update () {
		if(playingCinematic){
			cinematicTime += Time.deltaTime;
			CinematicResolution();
		}
	}


	//------------------------------------
	// CinematicResoluton
	//------------------------------------
	
	void CinematicResolution(){
		if(cinematicType == "YesNoTest"){
			if(cinematicTime >= 2 && !cinematicEvent1){
				YesNoColorPick();
				cinematicEvent1 = true;
			}
			if(cinematicTime >= 3 && !cinematicEvent2){
				cinematicEvent2 = true;
				endCinematic();
			}
		}

		if(cinematicType == "AskHelp"){

			if(!cinematicEvent1){
				cinematicEvent1 = true;
				cinematicObject1.GetComponent<Animator>().SetBool("Attention",true);
			}

			if(cinematicEvent1 && cinematicObject1.GetComponent<Animator>().GetBool("ArrivedWindow")){
				cinematicEvent2 = true;
				endCinematic();
			}
		}

		if(cinematicType == "PointAt"){			
			if(!cinematicEvent1){
				cinematicEvent1 = true;
				cinematicObject4.GetComponent<Animator>().SetBool("BehindYou",true);
			}

			if(!cinematicEvent2 && cinematicObject4.GetComponent<Animator>().GetBool("SentKey")){
				cinematicEvent2 = true;
				cinematicTime = 0;
			}

			if(cinematicEvent2 && !cinematicEvent3 && cinematicTime > 0 && cinematicTime <= 2){
				float time = cinematicTime/2f;
				Vector3 targetpos = cinematicObject3.transform.position;
				cinematicObject1.transform.position = Vector3.Lerp(cinematicPositionAux, targetpos, time);
				float auxScale = 0.5f - time/2f;
				cinematicObject1.transform.localScale = new Vector3(auxScale, auxScale, auxScale);
			}

			//Ends movement and opens gate
			if(cinematicEvent2 && !cinematicEvent3 && cinematicTime > 2){
				cinematicEvent3 = true;
				cinematicObject1.SetActive(false);
				cinematicObject2.GetComponent<GateOpenClose>().OpenDoor();

				endCinematic();
			}
		}


		if(cinematicType == "Audio"){
			if(cinematicTime >= 2 && !cinematicEvent1){
				AudioSource music_c = cinematicObject1.GetComponent<AudioSource>();
				music_c.mute = true;

				Light light_c = cinematicObject2.GetComponent<Light>();
				light_c.enabled = false;

				cinematicEvent1 = true;
				endCinematic();
			}
		}

		if(cinematicType == "ComeHere"){

			if(cinematicTime > 1 && cinematicTime <= 3){
				float time = (cinematicTime - 1)/2f;
				Vector3 targetpos = cinematicObject3.transform.position;

				
				cinematicObject1.transform.position = Vector3.Lerp(cinematicPositionAux, targetpos, time);
				
				float auxScale = 0.5f - time/2f;
				cinematicObject1.transform.localScale = new Vector3(auxScale, auxScale, auxScale);
			}

			if(cinematicTime > 3 && !cinematicEvent1){
				cinematicEvent1 = true;

				cinematicObject1.SetActive(false);
				cinematicObject2.GetComponent<GateOpenClose>().OpenDoor();

				endCinematic();
			}

			
		}

		if(cinematicType == "GoAway"){
			if(cinematicTime >= 2 && !cinematicEvent1){
				cinematicEvent1 = true;

				for(int i = 0; i < 5; i++){
					GameObject fly = cinematicObject1.transform.Find("Fly " + i.ToString()).gameObject;
					FlyRandomFlight flyscript = fly.GetComponent<FlyRandomFlight>();
					flyscript.Goaway();
				}

				endCinematic();

			} 
			if(cinematicTime >= 4 && cinematicEvent1){
				cinematicEvent2 = true;
				endCinematic();
			}
		}

		if(cinematicType == "Victory"){
			if(cinematicTime < 3){
				float time = (cinematicTime - 1)/3f;
				float auxScale = 1 - time;
				cinematicObject1.transform.localScale = new Vector3(auxScale, auxScale, auxScale);

			}
			if(cinematicTime >= 3 && !cinematicEvent1){
				cinematicObject1.SetActive(false);
				cinematicEvent1 = true;
				endCinematic();
			}
			
		}

		if(cinematicType == "Photo"){
			if(cinematicTime >= 2 && !cinematicEvent1){
				cinematicObject1.GetComponent<SpriteFader>().FadeSpriteIn();
				cinematicEvent1 = true;
			}
			if(cinematicTime >= 5 && !cinematicEvent2){
				cinematicEvent2 = true;
				endCinematic();
			}
		}

		if(cinematicType == "Impatient"){
			if(cinematicTime >= 2 && !cinematicEvent1){
				cinematicObject1.GetComponent<GateOpenClose>().OpenDoor();
				cinematicEvent1 = true;
			}
			if(cinematicTime >= 3 && !cinematicEvent2){
				cinematicEvent2 = true;
				endCinematic();
			}
		}

		if(cinematicType == "PhoneHelp"){
			if(cinematicTime >= 2 && !cinematicEvent1){
				cinematicObject1.GetComponent<GateOpenClose>().OpenDoor();
				cinematicEvent1 = true;
			}
			if(cinematicTime >= 3 && !cinematicEvent2){
				cinematicEvent2 = true;
				AudioSource phone_ring = cinematicObject2.GetComponent<AudioSource>();
				phone_ring.mute = true;
				endCinematic();
			}
		}
	}


	//------------------------------------
	//---- Setup
	//------------------------------------

	
	public void setupObjectsInitial(string stateType){
		switch(stateType){
			case "YesNoTest":
				setupObjectsInitialYesNo();
				break;
			case "AskHelp":
				setupObjectsInitialAskHelp();
				break;
			case "PointAt":
				setupObjectsInitialPointAt();
				break;
			case "Audio":
				setupObjectsInitialAudio();
				break;
			case "GoAway":
				setupObjectsInitialGoAway();
				break;
			case "ComeHere":
				setupObjectsInitialComeHere();
				break;
			case "Victory":
				setupObjectsInitialVictory();
				break;
			case "Photo":
				setupObjectsInitialPhoto();
				break;
			case "Impatient":
				setupObjectsInitialImpatient();
				break;
			case "PhoneHelp":
				setupObjectsInitialPhoneHelp();
				break;

				

			default:
				break;
		}
	}

	public void setupObjectSteps(string stateType, int step){
		switch(stateType){
			case "YesNoTest":
				YesNoChangeToCyan(step);
				break;
			case "PointAt":
				setupObjectsInitialPointAt();
				break;
			default:
				break;
		}
	}


	//------------------------------------
	//---- YesNoTest
	//------------------------------------

	void setupObjectsInitialYesNo(){
		cinematicObject1 = transform.Find("Object1").gameObject;
		cinematicObject2 = transform.Find("Object2").gameObject;
		cinematicObject3 = transform.Find("Object3").gameObject;
	}

	public void playCinematicYesNo(int step, bool answer){
		cinematicBoolAux = answer;
		cinematicStepAux = step;
		startCinematic("YesNoTest");
	}

	void YesNoChangeToCyan(int step){
		Material cyanMaterial = (Material)Resources.Load("Materials/Test Spheres/CyanSphereObject", typeof(Material));
		if(step == 0)
			cinematicObject1.GetComponent<MeshRenderer>().material = cyanMaterial;
		if(step == 1)
			cinematicObject2.GetComponent<MeshRenderer>().material = cyanMaterial;
		if(step == 2)
			cinematicObject3.GetComponent<MeshRenderer>().material = cyanMaterial;
	}

	void YesNoColorPick(){
		if(cinematicBoolAux)
			YesNoChangeToGreen(cinematicStepAux);
		else
			YesNoChangeToRed(cinematicStepAux);
	}
	void YesNoChangeToRed(int step){
		Material redMaterial = (Material)Resources.Load("Materials/Test Spheres/RedSphereObject", typeof(Material));
		if(step == 0)
			cinematicObject1.GetComponent<MeshRenderer>().material = redMaterial;
		if(step == 1)
			cinematicObject2.GetComponent<MeshRenderer>().material = redMaterial;
		if(step == 2)
			cinematicObject3.GetComponent<MeshRenderer>().material = redMaterial;
	}
	void YesNoChangeToGreen(int step){
		Material greenMaterial = (Material)Resources.Load("Materials/Test Spheres/GreenSphereObject", typeof(Material));
		if(step == 0)
			cinematicObject1.GetComponent<MeshRenderer>().material = greenMaterial;
		if(step == 1)
			cinematicObject2.GetComponent<MeshRenderer>().material = greenMaterial;
		if(step == 2)
			cinematicObject3.GetComponent<MeshRenderer>().material = greenMaterial;
	}


	//------------------------------------
	//---- AskHelp
	//------------------------------------

	void setupObjectsInitialAskHelp(){
		cinematicObject1 = transform.Find("Person").gameObject;
		
	}

	public void playCinematicAskHelp(){
		startCinematic("AskHelp");
	}

	void setupObjectsInitialPointAt(){
		cinematicObject1 = transform.Find("Blue Key").gameObject;
		cinematicPositionAux = cinematicObject1.transform.position;
		cinematicObject2 = transform.Find("BlueGate").gameObject;
		cinematicObject3 = transform.Find("PlayerPlacement").gameObject;
		cinematicObject4 = transform.Find("Person").gameObject;
	}

	public void playCinematicPointAt(){
		startCinematic("PointAt");
	}

	//------------------------------------
	//---- Audio
	//------------------------------------
	
	void setupObjectsInitialAudio(){
		cinematicObject1 = transform.Find("Music Source").gameObject;
		cinematicObject2 = transform.Find("Light").gameObject;
	}
	
	public void playCinematicAudio(){
		startCinematic("Audio");
	}   

	//------------------------------------
	//---- ComeHere
	//------------------------------------

	void setupObjectsInitialComeHere(){
		cinematicObject1 = transform.Find("Key").gameObject;
		cinematicObject2 = transform.Find("Gate").gameObject;
		cinematicObject3 = transform.Find("PlayerPlacement").gameObject;
		cinematicPositionAux = cinematicObject1.transform.position;
	}
	
	public void playCinematicComeHere(){
		startCinematic("ComeHere");
	}

	//------------------------------------
	//---- GoAway
	//------------------------------------

	void setupObjectsInitialGoAway(){
		cinematicObject1 = transform.Find("Flies").gameObject;
	}
	public void playCinematicGoAway(){
		startCinematic("GoAway");
	}

	//------------------------------------
	//---- Victory
	//------------------------------------

	void setupObjectsInitialVictory(){
		cinematicObject1 = transform.Find("End Cube").gameObject;
	}
	
	public void playCinematicVictory(){
		startCinematic("Victory");
	}

	//------------------------------------
	//---- Photo
	//------------------------------------

	
	void setupObjectsInitialPhoto() {
		cinematicObject1 = transform.Find("Image").gameObject;
	}
	
	public void playCinematicPhoto(){
		startCinematic("Photo");
	}

	//------------------------------------
	//---- Phone for Help
	//------------------------------------

	
	void setupObjectsInitialPhoneHelp() {
		cinematicObject1 = transform.Find("RedGate").gameObject;
		cinematicObject2 = transform.Find("Ringing").gameObject;
	}
	
	public void playCinematicPhoneHelp(){
		startCinematic("PhoneHelp");
	}

	//------------------------------------
	//---- Impatient
	//------------------------------------
	
	void setupObjectsInitialImpatient(){
		cinematicObject1 = transform.Find("GreenGate").gameObject;
	}
	public void playCinematicImpatient(){
		startCinematic("Impatient");
	}


	//------------------------------------
	//---- Auxiliary code
	//------------------------------------

	void startCinematic(string type){
		cinematicOver = false;
		cinematicTime = 0;
		playingCinematic = true;
		cinematicType = type;

		cinematicEvent1 = false;
		cinematicEvent2 = false;
		cinematicEvent3 = false;
	}

	void endCinematic(){
		playingCinematic = false;
		cinematicOver = true;
	}


}
