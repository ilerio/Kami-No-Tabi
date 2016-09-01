using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour 
{
	public GameObject pauseMenu;
	public GameObject hiraganaChart;
	public GameObject romanjiDisplay;
	public GameObject voiceAndRomanjiButton;
	public GameObject romanjiPromptButton;
	public GameObject gameOverScreen;

	private bool showing;
	private bool romanjiShowing;

	void Start ()
	{
		showing = false;
		romanjiShowing = false;
	}
	
	public void PauseGame ()
	{
		if (showing == true)
		{
			hiraganaChart.SetActive(false);
			showing = false;


			if(romanjiShowing)
			{
				romanjiDisplay.SetActive(false);
				romanjiPromptButton.SetActive (true);
			}
			else
				voiceAndRomanjiButton.SetActive (true);
		}
		else if (romanjiShowing)
		{
			romanjiDisplay.SetActive (false);
			romanjiPromptButton.SetActive (true);
		}

		pauseMenu.SetActive(true);
		DisableOni ();
		Time.timeScale = 0f;
	}
	
	public void UnpauseGame ()
	{
		if(romanjiShowing)
		{
			romanjiDisplay.SetActive(true);
			romanjiPromptButton.SetActive (true);
		}
		else
			voiceAndRomanjiButton.SetActive (true);

		pauseMenu.SetActive(false);
		EnableOni ();
		Time.timeScale = 1f;
	}
	
	public void Restart ()
	{
		Time.timeScale = 1f;

		Application.LoadLevel (Application.loadedLevel);
	}

	public void Quit()
	{
		Time.timeScale = 1f;
		Application.LoadLevel("MainMenue");
	}

	public void NextLevel()
	{
		gameOverScreen.SetActive(false);

		LoadingScreen ls = GameObject.FindObjectOfType (typeof (LoadingScreen)) as LoadingScreen;
		ls.LoadScene("Level_01_Transition");
	}

	public void HiraganaToggle () // used to display Hirragana Chart
	{
		if (showing == false) 
		{
			showing = true;
			hiraganaChart.SetActive(true);

			DisableOni();

			if(romanjiShowing)
			{
				romanjiDisplay.SetActive(false);
				romanjiPromptButton.SetActive(false);
			} 
			else
				voiceAndRomanjiButton.SetActive (false);
		}
		else if (showing == true)
		{
			showing = false;
			hiraganaChart.SetActive(false);

			EnableOni();

			if (romanjiShowing)
			{
				romanjiDisplay.SetActive(true);
				romanjiPromptButton.SetActive(true);
			}
			else 
				voiceAndRomanjiButton.SetActive(true);
		}
	}

	public void RomanjiOn ()
	{
		romanjiDisplay.SetActive(true);
		voiceAndRomanjiButton.SetActive (false);
		romanjiPromptButton.SetActive (true);
		romanjiShowing = true;
	}

	public void RomanjiOff ()
	{
		romanjiDisplay.SetActive(false);
		voiceAndRomanjiButton.SetActive (true);
		romanjiPromptButton.SetActive (false);
		romanjiShowing = false;
	}

	void DisableOni ()
	{
		GameObject[] OniGOs = GameObject.FindGameObjectsWithTag ("Oni");
		foreach(GameObject go in OniGOs)
		{
			PolygonCollider2D p = go.GetComponentInChildren<PolygonCollider2D>();
			p.enabled = false;
		}
	}

	void EnableOni ()
	{
		GameObject[] OniGOs = GameObject.FindGameObjectsWithTag ("Oni");
		foreach(GameObject go in OniGOs)
		{
			PolygonCollider2D p = go.GetComponentInChildren<PolygonCollider2D>();
			p.enabled = true;
		}
	}
}
