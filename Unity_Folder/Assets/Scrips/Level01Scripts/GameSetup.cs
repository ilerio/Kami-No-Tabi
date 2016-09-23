using UnityEngine;
using System.Collections;

public class GameSetup : MonoBehaviour 
{
	public Camera mainCamera;
	public Transform oni;
	public GameObject pauseMenu;
	public GameObject hiraganaChart;
	public GameObject gameOverScreen;
	public GameObject romanjiPromptButton;
	public GameObject voiceAndRomanjiPromptButton;
	public GameObject pauseButton;
	public GameObject hiraganaChartButton;
	public GameObject timer;
	public GameObject UI;
	public GameObject romanji;
	public GameObject incDec;
	
	void Start ()
	{
		// disableing in the way UI elements
		pauseMenu.SetActive(false);
		hiraganaChart.SetActive(false);
		gameOverScreen.SetActive(false);
		romanjiPromptButton.SetActive(false);
		voiceAndRomanjiPromptButton.SetActive (true);
		pauseButton.SetActive (true);
		hiraganaChartButton.SetActive (true);
		timer.SetActive (true);
		UI.SetActive (true);
		romanji.SetActive (false);
		incDec.SetActive (false);

		DrawOni ();
	}

	public void DrawOni()
	{
		// Instantiate Oni
		Instantiate (oni, new Vector2(-22f,-3f), Quaternion.identity);
		Instantiate (oni, new Vector2(-2f,-3f), Quaternion.identity);
		Instantiate (oni, new Vector2(19f,-3f), Quaternion.identity);
	}

	public void DisableAll()
	{
		pauseMenu.SetActive(false);
		hiraganaChart.SetActive(false);
		romanjiPromptButton.SetActive(false);
		voiceAndRomanjiPromptButton.SetActive (false);
		pauseButton.SetActive (false);
		hiraganaChartButton.SetActive (false);
		timer.SetActive (false);
		romanji.SetActive(false);
		incDec.SetActive (false);
	}

}