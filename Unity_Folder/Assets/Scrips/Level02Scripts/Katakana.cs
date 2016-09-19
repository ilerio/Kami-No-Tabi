using UnityEngine;
using System.Collections;

public class Katakana : MonoBehaviour 
{
	public Sprite[] katakana;
	public SpriteRenderer katakanaDisplayObject;
	
	// Initializes each clone Katakana object to a katakana sprite...
	// > also checks to make sure the sprite has not already been used and updates its used array.
	void Start ()
	{
		GetSprite ();
	}
	
	public void GetSprite()
	{
		GameController_L02 gameController = GameObject.FindObjectOfType (typeof (GameController_L02)) as GameController_L02;
		int num = gameController.KatakanaSetUp(); // Will get the spesific sprite to be rendered -> From GameController_L02.cs
		katakanaDisplayObject.sprite = katakana [num];
		
		int kataknaNum = gameController.GetNum();
		gameObject.name = "Katakana" + kataknaNum;
	}
}
