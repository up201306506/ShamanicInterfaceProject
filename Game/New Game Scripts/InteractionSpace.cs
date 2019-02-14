using UnityEngine;
using System;
using System.IO;
using System.Collections;
using ShamanicInterface.State;

public class InteractionSpace : MonoBehaviour {

	//--------------------------
	//--- Attach this behavior onto an object with a collider
	//--- The object's tag must be "Interaction"
	//--- This will interface with a player object with a "NewCharacterControllerScript" script
	//--- Multiple steps for actions supported. Make absolute sure to define them on the editor.
	//--- If you want more than one valid action on each step, or even different behaviors, toggle the useSecondary flag
	//--- Make sure to define action states on "newGameScript"
	//--- If you do not do the above, the Interaction object will self-break to avoid undefined behaviour
	//--- Cinematic transitions between interaction actions should be defined on a case by case standard, and best defined on separate scripts
	//--- As should secondary action effects. Primary actions should ssume linear pogression, used secondary to do weird things.
	//--------------------------

	//Make sure to change this on editor
	//Valid StateNames can be found on NewGame.GetState
	public int actionsUsed = 1;

	public string[] storedStateName = {"","",""};
	private string[] primaryAction = {"","",""};
	private string[] secondaryAction = {"","",""};
	private bool[] useSecondary = {true,true,true};
	
	private int step = 0; //Number of actions performed
	
	//Player's Controller Script
	NewCharacterControllerScript playerscript;

	//Action resolution related objects
	private float actionTimeLast = 0f;
	private float actionTimeSent = 0f;
	private bool receivingActions = false;

	//Cinematic related objects
	private CinematicHandler cinematicScript;
	private bool playingCinematic = false;

	//Other
	private GameObject playerPlacement;  
	private bool gameIsOver = false;

	//----------------------------------
	//--- Setup
	//----------------------------------

	void Start() {

		playerPlacement = transform.Find("PlayerPlacement").gameObject;
		cinematicScript = GetComponent<CinematicHandler>();

		for(int i = 0; i < actionsUsed; i++){
			Actions state = NewGame.GetState(storedStateName[i]);
			if(state == null){
				Debug.Log("One of the interaction spaces is broken. Cannot recognize state: " + storedStateName[i]);
				closeThis();
			} else {
				if(primaryAction[i] == "")
					assumeActionsFromStateConstruction(state, i);
			}
		}
		
		if(this.gameObject.tag != "ClosedInteraction")
			cinematicScript.setupObjectsInitial(storedStateName[0]);
	}
	
	void Update() {
		//Handle Time Counting for cancelling input
		countTimeSinceAction();

		//Check if Cinematic Over
		checkCinematicEnd();
	}

	public void setPlayerScript(NewCharacterControllerScript script){
		playerscript = script;
	}

	void checkCinematicEnd(){
		if(playingCinematic){
			if(cinematicScript.cinematicOver){
				playingCinematic = false;

				if(storedStateName[step] == "Victory"){
					gameIsOver = true;
				}

				endCinematic();
			}
		}
	}

	//----------------------------------
	//--- Getters used by Player script 
	//----------------------------------

	// CharacterContoller allows access to: 
	// -> setCinematicMode()
	// -> setInteractionMode()
	// -> setMovementMode()


	public string getCurrentState(){
		return storedStateName[step];
	}
	public string getCurrentPrimaryAction(){
		return primaryAction[step];
	}
	public string getCurrentSecondaryAction(){
		return secondaryAction[step];
	}
	public bool getCurrentSecondaryUsageFlag(){
		return useSecondary[step];
	}
	public GameObject getPlayerPlacement(){
		return playerPlacement;
	}

	public bool wasSendingAction(){
		return receivingActions;
	}

	public void additionalSetup(){
		cinematicScript.setupObjectSteps(storedStateName[step], step);
	}

	public bool isGameOver(){
		return gameIsOver;
	}

	//----------------------------------
	//--- Action Input
	//----------------------------------

	public void receivePrimaryInput(){
		if(countActionTime()){
			receivingActions = false;
			actionPerformedPrimary();
		}

	}

	public void receiveSecondaryInput(){
		if(countActionTime()){
			//Intentional lack of action parse cancelation
			actionPerformedSecondary();
		}
	}
	
	public void resettingInput() {
		if(actionTimeLast > 0.3){
			actionTimeLast = 0f;
			actionTimeSent = 0f;
			receivingActions = false;
		}
	}

	bool countActionTime(){
		actionTimeLast = 0f;
		if(!receivingActions){
			receivingActions = true;
			actionTimeSent = 0f;
				
		} else {
			actionTimeSent += Time.deltaTime;

			if(actionTimeSent > 2){
				actionTimeLast = 0f;
				actionTimeSent = 0f;
				receivingActions = false;
				return true;
			}
		}
		return false;
	}

	void countTimeSinceAction(){
		if(receivingActions){
			actionTimeLast += Time.deltaTime;
		}
	}

	//----------------------------------
	//--- Action Resolution
	//----------------------------------

	void actionPerformedPrimary(){
		addLog("Player Performed a primary action of type: "+ storedStateName[step]);
		switch(storedStateName[step]){
			case "YesNoTest":
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicYesNo(step, true);
				break;

			case "AskHelp":
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicAskHelp();
				break;

			case "PointAt":
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicPointAt();
				break;

			case "Audio":
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicAudio();
				break;

			case "GoAway":
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicGoAway();
				break;

			case "ComeHere":
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicComeHere();
				break;

			case "Victory":
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicVictory();
				break;

			case "Photo":
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicPhoto();
				break;

			case "Impatient":
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicImpatient();
				break;

			case "PhoneHelp":
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicPhoneHelp();
				break;

			default:
				Debug.Log("Primary Action Completed on an incompleted state");
				playerscript.flashAura();
				endInteraction();
				break;
		}
	}

	void actionPerformedSecondary(){
		//Do "receivingActions = false;" on commands that are meant to do something
		addLog("Player Performed a secondary action of type: "+ storedStateName[step]);
		switch(storedStateName[step]){
			case "YesNoTest":
				receivingActions = false;
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicYesNo(step, false);
				break;

			case "AskHelp":
				receivingActions = false;
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicAskHelp();
				break;

			//case "PhoneHelp":
			//	receivingActions = false;
			//	playingCinematic = true;
			//	playerscript.setCinematicMode();
			//	playerscript.flashAura();
			//	cinematicScript.playCinematicPhoneHelp();
			//	break;

			case "Victory":
				receivingActions = false;
				playingCinematic = true;
				playerscript.setCinematicMode();
				playerscript.flashAura();
				cinematicScript.playCinematicVictory();
				break;

			default:
				Debug.Log("By default Secondary action does nothing");
				break;
		}
	}

	//----------------------------------
	//--- Useful Extras
	//----------------------------------

	void endCinematic(){

		step++;
		if(step >= actionsUsed){
			endInteraction();
		} else {
			playerscript.setInteractionMode();
			additionalSetup();
		}
			
	}

	void endInteraction(){
		addLog("Player Received Ability to Move");
		playerscript.setMovementMode();
		closeThis();
	}

	void closeThis(){
		Material gray = (Material)Resources.Load("Materials/Radiuses/Gray Radius", typeof(Material));
		GameObject radius = transform.Find("Radius").gameObject;
		radius.GetComponent<MeshRenderer>().material = gray;

		this.gameObject.tag = "ClosedInteraction";
	}

	void assumeActionsFromStateConstruction(Actions state, int i){
		foreach(string item in state.GetActions()){
			if(item != "NOTHING"){
				if(primaryAction[i] == "")
					primaryAction[i] = item;
				else if(secondaryAction[i] == "" && useSecondary[i])
					secondaryAction[i] = item;
			}
		}
	}

	void addLog(string message){
		DateTime dt = DateTime.Now;
		string now = dt.ToLongTimeString();
		File.AppendAllText("LOGS/logs.txt",  now + " - " + message + "\n");
	}
}
