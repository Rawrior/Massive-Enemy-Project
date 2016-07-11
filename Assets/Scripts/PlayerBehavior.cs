using UnityEngine;
using System.Collections;

public class PlayerBehavior : MonoBehaviour
{
	private ControllerClass controller;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		Debug.Log(controller.LeftThumbstick);
	}
}
