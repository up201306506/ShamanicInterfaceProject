using UnityEngine;
using System.Collections;

public class properturnangle : StateMachineBehaviour {

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool("TurnMe", true);
	}

}
