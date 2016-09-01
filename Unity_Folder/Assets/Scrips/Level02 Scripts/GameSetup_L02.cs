using UnityEngine;
using System.Collections;

public class GameSetup_L02 : MonoBehaviour 
{
	public Transform katakana;
	public GameObject pauseMenu;
	public GameObject katakanaChart;
	public GameObject gameOverScreen;
	public GameObject pauseButton;
	public GameObject katakanaChartButton;
	public GameObject hearts;
	public GameObject hiraganaDisplay;
	public GameObject UI;
	
	void Start ()
	{
		// disableing in the way UI elements
		pauseMenu.SetActive(false);
		katakanaChart.SetActive(false);
		gameOverScreen.SetActive(false);
		pauseButton.SetActive (true);
		katakanaChartButton.SetActive (true);
		hearts.SetActive (true);
		hiraganaDisplay.SetActive (true);
		UI.SetActive (true);
		
		DrawKatakana ();
	}
	
	public void DrawKatakana()
	{
		// Instantiate Oni: here check dificulty and set up appropriate amount of oni <- are we still doing this?
		Instantiate (katakana, new Vector2(-9.9f,0f), Quaternion.identity);
		Instantiate (katakana, new Vector2(0f,2f), Quaternion.identity);
		Instantiate (katakana, new Vector2(9.9f,0f), Quaternion.identity);
	}
	
	public void DisableAll()
	{
		//pauseMenu.SetActive(false);
		katakanaChart.SetActive(false);
		pauseButton.SetActive (false);
		katakanaChartButton.SetActive (false);
		hiraganaDisplay.SetActive (false);
		hearts.SetActive (false);
	}
}
