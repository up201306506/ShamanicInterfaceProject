using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using System.IO;
using Leap;
using UnityEngine.UI;
using System;
using ShamanicInterface.Culture;
using ShamanicInterface.Utils;
using ShamanicInterface.State;
using ShamanicInterface.Classifier;

public class NewGame {

	
	//private static Dictionary<string,float> actionsBuffer = new Dictionary<string, float>();
	//private static float bufferStartTime = 0;
	//private static float minActionTime = 0.5f;
	//private static float timeToStartRead = 1; 

	public static string culture = "PT";
	//public static int bufferSize = 2;

	public static bool alreadyStarted = false;


	//--------------------------------------------------------------------------------
	//---- Initialization
	//--------------------------------------------------------------------------------

	public static CulturalLayer culturalLayer = new CulturalLayer();

	public static void StartCulture() {
		if(!alreadyStarted) {
			InitAllModels();
			InitCulturalLayer();
			alreadyStarted = true;
		}
	}


	//--------------------------------------------------------------------------------
	//---- Models
	//--------------------------------------------------------------------------------

	public static Dictionary<string, HiddenMarkovModel<MultivariateNormalDistribution>> allModels =
		new Dictionary<string, HiddenMarkovModel<MultivariateNormalDistribution>> ();

	public static List<HiddenMarkovModel<MultivariateNormalDistribution>> GetModels(Actions state) 
	{
		return ShamanicInterface.Utils.Utils.GetModelsWithCulture(allModels, state.GetActions(),
		                                                     culturalLayer, culture);
	}

	public static void InitAllModels() {
		allModels.Add("OPEN_HAND",
						HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/NOTHING_NEW20.bin"));
		              //HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/OPEN_HAND.bin"));
		allModels.Add("THUMBS_DOWN",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/THUMBS_DOWN5.bin"));
		allModels.Add("THUMBS_UP",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/THUMBS_UP10.bin"));
		allModels.Add("WAVE",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/WAVE10.bin"));
		allModels.Add("POINT_FRONT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/POINT_FRONT.bin"));
		allModels.Add("POINT_BACK",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/POINT_BACK.bin"));
		allModels.Add("POINT_RIGHT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/POINT_RIGHT.bin"));
		allModels.Add("POINT_LEFT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/POINT_LEFT.bin"));
		allModels.Add("QUIET_NEW",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/QUIET_NEW14.bin"));
		allModels.Add("IMPATIENT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/IMPATIENT14.bin"));
		//allModels.Add("SHAKA_UP",
		//              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/SHAKA_UP14.bin"));
		allModels.Add("SHAKA_DOWN",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/SHAKA_DOWN20.bin"));
		allModels.Add("POINT_AT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/POINT_TO_OBJECT_RIGHT14.bin"));
		allModels.Add("ATTENTION",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/POINT_UP12.bin"));
		allModels.Add("COME_HERE",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/COME_HERE20.bin"));
		allModels.Add("GO_AWAY",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/GO_AWAY14.bin"));
		allModels.Add("V_VICTORY",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/V_VICTORY12.bin"));
		allModels.Add("FIST_PUMP",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/RAISED_FIST6.bin"));
		allModels.Add("PHOTO_FRAME",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/PHOTO_FRAME14.bin"));
					  
		}
	
	//----------------------------------
	//---- Cultural Layer and classifier
	//----------------------------------

	public static void InitCulturalLayer() {

		//Make list of default gestures here.
		culturalLayer.AddDefaultGesture("NOTHING", "OPEN_HAND");
		culturalLayer.AddDefaultGesture("CANCEL", "WAVE");
		culturalLayer.AddDefaultGesture("WAVE", "WAVE");
		culturalLayer.AddDefaultGesture("ATTENTION", "ATTENTION");
		culturalLayer.AddDefaultGesture("NO", "THUMBS_DOWN");
		culturalLayer.AddDefaultGesture("YES", "THUMBS_UP");
		culturalLayer.AddDefaultGesture("FRONT", "POINT_FRONT");
		culturalLayer.AddDefaultGesture("BACK", "POINT_BACK");
		culturalLayer.AddDefaultGesture("RIGHT", "POINT_RIGHT");
		culturalLayer.AddDefaultGesture("LEFT", "POINT_LEFT");
		culturalLayer.AddDefaultGesture("QUIET", "QUIET_NEW");
		culturalLayer.AddDefaultGesture("IMPATIENT", "IMPATIENT");
		//culturalLayer.AddDefaultGesture("PHONE_UP", "SHAKA_UP");
		culturalLayer.AddDefaultGesture("PHONE", "SHAKA_DOWN");
		culturalLayer.AddDefaultGesture("POINT_AT", "POINT_AT");
		culturalLayer.AddDefaultGesture("COME_HERE", "COME_HERE");
		culturalLayer.AddDefaultGesture("GO_AWAY", "GO_AWAY");
		culturalLayer.AddDefaultGesture("FIST_PUMP", "FIST_PUMP");
		culturalLayer.AddDefaultGesture("V_VICTORY", "V_VICTORY");
		culturalLayer.AddDefaultGesture("PHOTO_FRAME", "PHOTO_FRAME");

		//Make list of a different culture gestures here
		//culturalLayer.AddCultureGesture("", "OTHER", "");
	}

	public static HMMClassifier GetClassifier(Actions state) {
		HMMClassifier classifier = new HMMClassifier(GetModels(state), state.GetActions());
		classifier.StartClassifier();
		return classifier;
	}


	//----------------------------------
	//---- List of state actions (use get-state)
	//----------------------------------

	public static Actions GetState(string name){
		switch (name)
		{
			case "Movement":
				return MovementState();
			case "Paused":
				return PauseState();
			case "YesNoTest":
				return YesNoState();
			case "Waiting":
				return NothingState();
			case "Audio":
				return AudioShushState();

			//States in need of new models
			case "ComeHere":
				return ComeHereState();
			case "GoAway":
				return GoAwayState();
			case "Victory":
				return VictoryState();
			case "Photo":
				return PhotoState();
			case "AskHelp":
				return AskAttentionState();
			case "PointAt":
				return PointAtState();
			case "PhoneHelp":
				return PhoneState();
			case "Impatient":
				return ImpatientState();
				

			default:
				return null;
		}
	}

	//NOTE: Order matters when creating these states. First AddAction should always be "NOTHING" 
	//(so the classifier has a neutral state), and the first two actions afterwards should be the
	//primary and secondary actions, in order.


	public static Actions MovementState() {
		Actions state = new Actions("Game State");
		state.AddAction("NOTHING");
		state.AddAction("FRONT");
		state.AddAction("BACK");
		state.AddAction("LEFT");
		state.AddAction("RIGHT");
		return state;
	}
	
	public static Actions PauseState() {
		Actions state = new Actions("Pause State");
		state.AddAction("NOTHING");
		state.AddAction("CANCEL");
		state.AddAction("NO");
		state.AddAction("YES");
		return state;
	}

	public static Actions YesNoState() {
		Actions state = new Actions("Test YesNo State");
		state.AddAction("NOTHING");
		state.AddAction("YES");
		state.AddAction("NO");
		return state;
	}
	
	public static Actions NothingState() {
		Actions state = new Actions("Nothing State");
		state.AddAction("NOTHING");
		return state;
	}

	public static Actions AudioShushState() {
		Actions state = new Actions("Shush State");
		state.AddAction("NOTHING");
		state.AddAction("QUIET");
		return state;
	}

	public static Actions ComeHereState() {
		Actions state = new Actions("Come Here State");
		state.AddAction("NOTHING");
		state.AddAction("COME_HERE");
		return state;
	}

	public static Actions GoAwayState() {
		Actions state = new Actions("Get Out State");
		state.AddAction("NOTHING");
		state.AddAction("GO_AWAY");
		return state;
	}

	public static Actions VictoryState() {
		Actions state = new Actions("Victory State");
		state.AddAction("NOTHING");
		state.AddAction("FIST_PUMP");
		state.AddAction("V_VICTORY");
		return state;
	}

	public static Actions PhotoState() {
		Actions state = new Actions("Photo State");
		state.AddAction("NOTHING");
		state.AddAction("PHOTO_FRAME");
		return state;
	}

	public static Actions AskAttentionState() {
		Actions state = new Actions("Ask for Attention State");
		state.AddAction("NOTHING");
		state.AddAction("ATTENTION");
		state.AddAction("WAVE");
		return state;
	}

	public static Actions PointAtState() {
		Actions state = new Actions("Point At Something State");
		state.AddAction("NOTHING");
		state.AddAction("POINT_AT");
		return state;
	}

	public static Actions PhoneState() {
		Actions state = new Actions("Phone for Help State");
		state.AddAction("NOTHING");
		//state.AddAction("PHONE_UP");
		state.AddAction("PHONE");
		return state;
	}

	public static Actions ImpatientState() {
		Actions state = new Actions("Being Impatient State");
		state.AddAction("NOTHING");
		state.AddAction("IMPATIENT");
		return state;
	}

	//----------------------------------
	//---- Action Buffer
	//----------------------------------

    /*
	public static void StartActionBuffer() {
		bufferStartTime = Time.time;
		actionsBuffer.Clear();
	}

	public static List<string> UpdateActionBuffer(List<string> actions) {
		List<string> actionsToCheck = new List<string> (actionsBuffer.Keys);
		List<string> returnActions = new List<string>();

		if((bufferStartTime + timeToStartRead) > Time.time) {
			return returnActions;
		}

		foreach(string action in actions) {
			actionsToCheck.Remove(action);
			if(!actionsBuffer.ContainsKey(action)) {
				actionsBuffer[action] = Time.time;
			}

			if(actionsBuffer[action] + minActionTime < Time.time) {
				returnActions.Add (action);
			}
		}

		foreach (string actionToCheck in actionsToCheck) {
			actionsBuffer.Remove(actionToCheck);
		}

		return returnActions;
	}
	*/

	//----------------------------------
	//----------------------------------
	//----------------------------------

}