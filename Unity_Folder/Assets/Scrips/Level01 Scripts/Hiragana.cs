using UnityEngine;
using System.Collections;

public class Hiragana : MonoBehaviour 
{
	public Sprite[] hiragana;
	public SpriteRenderer hiraganaDisplayObject;

	private int randomNumber;

	// Initializes each clone Oni to a hiragana sprite...
	// > also checks to make sure the sprite has not already been used and updates its used array.
	void Start ()
	{
		ChooseSprite ();
	}
	
	public void ChooseSprite()
	{
		GameController gameController = GameObject.FindObjectOfType (typeof (GameController)) as GameController;
		int num = gameController.HiraganaSetUp(); // Will get the spesific sprite to be rendered on a spesific instance oni -> From GameController.cs
		hiraganaDisplayObject.sprite = hiragana [num];

		int oniNum = gameController.GetOniNum();
		gameObject.name = "Oni" + oniNum;
	}
}
