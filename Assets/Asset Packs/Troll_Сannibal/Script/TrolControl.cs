using UnityEngine;

public class TrolControl : MonoBehaviour {
	
	private Animator anim;
	private CharacterController controller;


	// Use this for initialization
	void Start () {

		anim = GetComponent<Animator>();
		controller = GetComponent<CharacterController> ();
	}
}
