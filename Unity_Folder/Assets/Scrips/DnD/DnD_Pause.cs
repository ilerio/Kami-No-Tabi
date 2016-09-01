using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DnD_Pause : MonoBehaviour 
{
	public GameObject pauseMenu;

	void Start()
	{
		pauseMenu.SetActive (false);
	}
	
	public void PauseGame()
	{
		pauseMenu.SetActive(true);
	}
	
	public void UnpauseGame()
	{
		pauseMenu.SetActive(false);
	}
	
	public void Restart()
	{
		Application.LoadLevel (Application.loadedLevel);
	}
	
	public void Quit()
	{
		Time.timeScale = 1f;
		Application.LoadLevel("MainMenue");
	}
}
