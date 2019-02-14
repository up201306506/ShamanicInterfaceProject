using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using System.Collections.Generic;
using Leap;
using System;
using System.IO;
using ShamanicInterface.Classifier;
using ShamanicInterface.State;

public class NewCharacterControllerScript : MonoBehaviour {

	//Game state (and storage for pausing) for input resolution
	//not the same as the GameStates in the NewGameScript
	private enum GameState {Movement, Pause, Interaction, Waiting};
	private GameState state;
	private GameState previousState;
	private string stateName;
	private string previousStateName;

	//Command listing for input resolution 
	private enum Commands {None, Move, Pause, Mute, PrimaryInter, SecondaryInter, NoneInter};
	private Commands current_command;
	
	//hud objects
	public GameObject hudObject;
	private Text HUDGameStateInfo;
	private Text HUDCommandListInfo;
	private Text HUDCurrentCommandInfo;
	private ImageFader HUDAuraControlScript;
	private PanelFader HUDEndPanelScript;
	private List<string> listactions;

	//Interation spaces objects
	private InteractionSpace interScript;

	//Moving Player Automatically:
	private Vector3 initialPlacement;
	private Vector3 targetPlacement;
	private Quaternion initialRotation;
	private Quaternion targetRotation;
	private float placementTime;
	private int placementTotalTime;
	private bool placementFlag = false;

	//other objects
	public HandController controller;
	private HMMClassifier classifier;

	private bool muted = false;
	
	private float horzInput;
	private float vertInput;

	private bool gameIsOver = false;


	//--------------------------------------------------------------------------------
	//---- Initialization
	//--------------------------------------------------------------------------------

	void Awake() {
		NewGame.StartCulture();
		UpdateState("Movement");
	}

	// Use this for initialization
	void Start() {
		System.IO.Directory.CreateDirectory("LOGS");
		addLog("Game started");

		state = GameState.Movement;
		current_command = Commands.None;
		if(hudObject != null)
		{
			HUDGameStateInfo = hudObject.transform.Find("GameStateInfo").GetComponent<Text>();
			HUDCommandListInfo = hudObject.transform.Find("CommandsList").GetComponent<Text>();
			HUDCurrentCommandInfo = hudObject.transform.Find("CurrentCommand").GetComponent<Text>();
			HUDAuraControlScript = hudObject.transform.Find("FadeAura").GetComponent<ImageFader>();
			HUDEndPanelScript = hudObject.transform.Find("EndGamePanel").GetComponent<PanelFader>();
			hudObject.transform.Find("EndGamePanel").gameObject.SetActive(true);
		}
		updateHUDInfo();
	}



	//--------------------------------------------------------------------------------
	//---- Update Functions
	//--------------------------------------------------------------------------------

	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	void FixedUpdate() {	
		//Direct Control
		if(!placementFlag)
		{	
			
			ResetInputVariables();
		
			//---------
			CheckGestureActions();
			CheckKeyBoardActions();

			//---------
			ResolveCommands();
		}

	}

	void Update(){
		//Automatic player movement
		if(placementFlag)
		{	
			placementTime += Time.deltaTime / placementTotalTime;
			if(placementTime >= 1)
			{
				transform.position = targetPlacement;
				transform.rotation = targetRotation;
				placementFlag = false;
			}
			else
			{
				transform.position = Vector3.Lerp(initialPlacement, targetPlacement, placementTime);
				transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, placementTime);
			}
		}

		//Game Over
		if(interScript != null){
			if(interScript.isGameOver())
			{
				if(!gameIsOver){
					gameIsOver = true;
					HUDEndPanelScript.endGame();
				} else {
					if(HUDEndPanelScript.endTheGame)
					{
						Debug.Log("Application quit!");
						Application.Quit();
					}
				}
			}
		}
	}


	//--------------------------------------------------------------------------------
	//----- Classifier Usage
	//--------------------------------------------------------------------------------

	void UpdateState(string name){
		Actions gamestate = NewGame.GetState(name);
		if(gamestate == null)
		{
			updateText(HUDGameStateInfo, "Serious Warning: Tried to change to a GameState that doesn't exist");
		}
		else
		{
			classifier = NewGame.GetClassifier(gamestate);
			stateName = name;
			listactions = gamestate.GetActions();
			Debug.Log("State changed to: " + name);
		}
	}

	//--------------------------------------------------------------------------------
	//---- Input Handling
	//--------------------------------------------------------------------------------

	void ResetInputVariables() {
		vertInput = 0;
		horzInput = 0;
		current_command = Commands.None;
	}

	void CheckGestureActions() {
		List<string> allActions = controller.GetGestures(classifier);
		//List<string> actions = NewGame.UpdateActionBuffer(allActions);

		//allActions	--> instant, good for movement.
		//actions 		--> buffered, may be good for actions you don't want to perform by mistake

		updateHUDCurrentCommand(allActions);
		
		switch(state) {
		case GameState.Movement:
			CheckMove(allActions);
			break;
		case GameState.Pause:
			break;
		case GameState.Interaction:
			CheckInteractionActions(allActions);
			break;
		default:
			break;
		}
		
	}

	void CheckKeyBoardActions() {
		//----
		if(state == GameState.Movement)
		{
			float keyH = Input.GetAxis("Horizontal");
			float keyV = Input.GetAxis("Vertical");
			
			if(keyH != 0) {horzInput = keyH; current_command = Commands.Move;}
			if(keyV != 0) {vertInput = keyV; current_command = Commands.Move;}

		}

		//----
		if(state == GameState.Interaction)
		{
			if(current_command == Commands.None){
				if(Input.GetButton("PrimaryInteraction")) {
					current_command = Commands.PrimaryInter;
				} else if (Input.GetButton("SecondaryInteraction") && interScript.getCurrentSecondaryUsageFlag()) {
					current_command = Commands.SecondaryInter;
				} else if(interScript.wasSendingAction()){
					current_command = Commands.NoneInter;
				}
			}
			
		}
		
		//----
		if(state != GameState.Waiting)
		{
			if(Input.GetButtonDown("Pause")) {
				current_command = Commands.Pause;
			}
		}
		if(Input.GetButtonDown("Mute")) {
			current_command = Commands.Mute;
		}

		//----
		/*if(Input.GetButtonDown("Quit")) {
			Debug.Log("Application quit!");
			Application.Quit();
		}*/
			
		
	}

	void CheckMove(List<string> actions) {
		for (int i = 0; i< actions.Count; i++) {
			switch(actions[i]) {
			case "FRONT":
				current_command = Commands.Move;
				vertInput = 1;
				break;
			case "BACK":
				current_command = Commands.Move;
				vertInput = -1;
				break;
			case "RIGHT":
				current_command = Commands.Move;
				horzInput = 1;
				break;
			case "LEFT":
				current_command = Commands.Move;
				horzInput = -1;
				break;
			default:
				break;
			}
		}
	}

	void CheckInteractionActions(List<string> actions){
		for (int i = 0; i< actions.Count; i++) {

			if(actions[i] == interScript.getCurrentPrimaryAction()){
				current_command = Commands.PrimaryInter;
			} else if(actions[i] == interScript.getCurrentSecondaryAction()) {
				if(interScript.getCurrentSecondaryUsageFlag()){
					current_command = Commands.SecondaryInter;
				}
			} else if(interScript.wasSendingAction()) {
				current_command = Commands.NoneInter;
			}

		}
	}

	void ResolveCommands() {
		switch(current_command) {
			case Commands.Move:
				MoveAndRotate();
				break;

			case Commands.Pause:
				if(state == GameState.Pause)
					Resume(); 
				else
					Pause(); 
				break;

			case Commands.Mute:
				if(muted) 
					Unmute();
				else 
					Mute();
				break;
			
			case Commands.PrimaryInter:
				DoPrimaryAction();
				break;
			
			case Commands.SecondaryInter:
				DoSecondaryAction();
				break;

			case Commands.NoneInter:
				DoStopSendingAction();
				break;
			default: 
				break;
		}

	}

	//--------------------------------------------------------------------------------
	//----- Movement
	//--------------------------------------------------------------------------------

	void MoveAndRotate() {
		if(vertInput != 0) { Move (vertInput); }
		if(horzInput != 0) {  Rotate(horzInput*(float)1.5); }
	}

	void Rotate (float value) {
		if(state != GameState.Movement) {return;}
		transform.RotateAround(transform.position, Vector3.up, value);
	}

	void Move (float value) {
		if(state != GameState.Movement) {return;}

		Vector3 target = transform.position + transform.forward*value;
		target.y = transform.position.y;
		transform.position = Vector3.MoveTowards(transform.position, target, 0.1f);
	}

	//--------------------------------------------------------------------------------
	//---- Pausing and Muting
	//--------------------------------------------------------------------------------

	void Pause() {
		previousState = state;
		previousStateName = stateName;

		state = GameState.Pause;
		UpdateState("Paused");

		//NewGame.StartActionBuffer();
		
		updateHUDInfo();
	}

	void Resume() {
		state = previousState;		
		UpdateState(previousStateName);

		updateHUDInfo();
	}

	void Mute() {
		muted = true;
		AudioListener.pause = true;
	}

	void Unmute() {
		muted = false;
		AudioListener.pause = false;
	}


	//--------------------------------------------------------------------------------
	//---- Interaction Space
	//--------------------------------------------------------------------------------

	void DoPrimaryAction(){
		interScript.receivePrimaryInput();
	}

	void DoSecondaryAction(){
		interScript.receiveSecondaryInput();
	}

	void DoStopSendingAction(){
		interScript.resettingInput();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "ClosedInteraction")
			Debug.Log("This interaction space is closed");

		if(other.tag == "Interaction" && state == GameState.Movement)
		{
			interScript = other.gameObject.GetComponent<InteractionSpace>();
			if (interScript != null)
			{
				interScript.setPlayerScript(this);
				interScript.additionalSetup();
				setInteractionMode();
				movePlayerTo(interScript.getPlayerPlacement());
				addLog("Player approached an interaction zone of type: " + interScript.getCurrentState());
			}
		}
			
	}

	public void setCinematicMode(){
		UpdateState("Waiting");
		updateHUDInfo();
		state = GameState.Waiting;
	}

	public void setInteractionMode(){
		UpdateState(interScript.getCurrentState());
		updateHUDInfo();
		state = GameState.Interaction;
	}

	public void setMovementMode(){
		UpdateState("Movement");
		updateHUDInfo();
		state = GameState.Movement;
	}

	void movePlayerTo(GameObject placer){
		initialPlacement = transform.position;
		targetPlacement = placer.transform.position;
		initialRotation = transform.rotation;
		targetRotation = placer.transform.rotation;
		placementTime = 0f;
		placementFlag = true;
		placementTotalTime = 3;
		
	}

	//--------------------------------------------------------------------------------
	//----- Ending the Game
	//--------------------------------------------------------------------------------

	public void endGame(){
		
	}

	//--------------------------------------------------------------------------------
	//----- HUD Stuff
	//--------------------------------------------------------------------------------

	void updateHUDInfo() {
		updateHUDGameState();
		updateHUDCommandList();
		updateHUDCurrentCommand();
	}

	void updateHUDGameState(){
		if(HUDGameStateInfo != null)
			updateText(HUDGameStateInfo, "Game State: " + stateName);
	}
	void updateHUDCommandList(){
		string content = "";
		foreach (string item in listactions)
            content += item + "\n";

		if(HUDCommandListInfo != null)
			updateText(HUDCommandListInfo, content);
	}
	void updateHUDCurrentCommand(){
		updateText(HUDCurrentCommandInfo, "None Detected.");
	}
	void updateHUDCurrentCommand(List<string> actions){
		if(HUDCurrentCommandInfo != null)
		{
			if(actions.Count == 0)
				updateHUDCurrentCommand();
			else {
				string result = "";
				for(int i = 0; i < actions.Count; i++) 
				{
					string action = actions[i]; 
					if(result != "" && action != "") 
						result += " + " + action;
					else
						result += action;
				}
				updateText(HUDCurrentCommandInfo, result);
			}
			
		}
			
	}
	
	void updateText(Text obj, string value) {
		obj.text = value;
	}

	public void flashAura(){
		if(HUDAuraControlScript != null){
			HUDAuraControlScript.FadeImageIn();
		}
	}
	
	void addLog(string message){
		DateTime dt = DateTime.Now;
		string now = dt.ToLongTimeString();
		File.AppendAllText("LOGS/logs.txt",  now + " - " + message + "\n");
	}

}