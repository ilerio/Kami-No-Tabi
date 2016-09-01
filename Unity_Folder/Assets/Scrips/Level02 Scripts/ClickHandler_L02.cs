using UnityEngine;
using System.Collections;

public class ClickHandler_L02 : MonoBehaviour 
{
	void OnMouseUp()
	{
		//Debug.Log ("Katakana Clicked!");
		GameController_L02 gameController = GameObject.FindObjectOfType (typeof (GameController_L02)) as GameController_L02;
		gameController.CheckCorrect(gameObject.name, gameObject); // passes the katakana name to GameController to check if selection is correct
	}
}
