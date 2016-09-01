using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenue_L02 : MonoBehaviour 
{
	public GameObject pauseMenu;
	public GameObject katakanaChart;
	public GameObject gameOverScreen;
	
	private bool showing;
	
	void Start()
	{
		showing = false;
	}
	
	public void PauseGame()
	{
		if (showing == true)
		{
			katakanaChart.SetActive(false);
			showing = false;
		}
		
		pauseMenu.SetActive(true);
		DisableKatakana();
	}
	
	public void UnpauseGame()
	{
		pauseMenu.SetActive(false);
		EnableKatakana();
	}
	
	public void Restart()
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
		ls.LoadScene("Level_02_Transition");
	}
	
	public void KatakanaToggle() // used to display Katakana Chart
	{
		if (showing == false) 
		{
			showing = true;
			katakanaChart.SetActive(true);
			
			DisableKatakana();
		}
		else if (showing == true)
		{
			showing = false;
			katakanaChart.SetActive(false);
			
			EnableKatakana();
		}
	}
	
	void DisableKatakana()
	{
		GameObject[] KatakanaGOs = GameObject.FindGameObjectsWithTag ("Katakana");
		foreach(GameObject go in KatakanaGOs)
		{
			BoxCollider2D b = go.GetComponentInChildren<BoxCollider2D>();
			b.enabled = false;
		}
	}
	
	void EnableKatakana()
	{
		GameObject[] KatakanaGOs = GameObject.FindGameObjectsWithTag ("Katakana");
		foreach(GameObject go in KatakanaGOs)
		{
			BoxCollider2D b = go.GetComponentInChildren<BoxCollider2D>();
			b.enabled = true;
		}
	}
}
