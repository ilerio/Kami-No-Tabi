using UnityEngine;
using System.Collections;

public class ClickHandler : MonoBehaviour 
{
	void OnMouseUp()
	{
		GameController gameController = GameObject.FindObjectOfType (typeof (GameController)) as GameController;
		gameController.CheckCorrect(gameObject.name, gameObject); // passes the oni name to Game controller to check if selection is correct
	}
}
