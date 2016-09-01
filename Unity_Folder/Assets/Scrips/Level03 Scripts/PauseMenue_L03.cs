using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenue_L03 : MonoBehaviour 
{
	public GameObject pauseMenu;
	public GameObject hiraganaButton;
	public GameObject KatakanaButton;
	public GameObject gameOverScreen;
	public GameObject hiraganaChars;
	public GameObject katakanaChars;

	private bool hiraganaCharsShowing;
	
	void Start ()
	{
		hiraganaCharsShowing = true;
		hiraganaChars.SetActive(true);
		katakanaChars.SetActive(false);
		hiraganaButton.SetActive(false); // If hiraganaCharShowing = true, then hiraganaButton.isActive = false
		KatakanaButton.SetActive(true);
	}
	
	public void PauseGame ()
	{
		pauseMenu.SetActive(true);
		DisableDragable ();
		Time.timeScale = 0f;
	}
	
	public void UnpauseGame ()
	{	
		pauseMenu.SetActive(false);
		EnableDragable ();
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
	
	public void ToggleCharacters ()
	{
		if (hiraganaCharsShowing)
		{
			hiraganaChars.SetActive(false);
			katakanaChars.SetActive(true);

			hiraganaButton.SetActive(true);
			KatakanaButton.SetActive(false);

			hiraganaCharsShowing = false;
		}
		else
		{
			hiraganaChars.SetActive(true);
			katakanaChars.SetActive(false);

			hiraganaButton.SetActive(false);
			KatakanaButton.SetActive(true);

			hiraganaCharsShowing = true;
		}
	}

	void DisableDragable ()
	{
		GameObject[] HiraganaGOs = GameObject.FindGameObjectsWithTag ("Hiragana");
		GameObject[] KatakanaGOs = GameObject.FindGameObjectsWithTag ("Katakana");
		GameObject[] TentenMaruGOs = GameObject.FindGameObjectsWithTag ("TentenMaru");

		if (HiraganaGOs.Length > 0)
			foreach(GameObject go in HiraganaGOs)
			{
				CanvasGroup cg = go.GetComponent<CanvasGroup>();
				cg.blocksRaycasts = false;
			}

		if (KatakanaGOs.Length > 0)
			foreach(GameObject go in KatakanaGOs)
			{
				CanvasGroup cg = go.GetComponent<CanvasGroup>();
				cg.blocksRaycasts = false;
			}

		if (TentenMaruGOs.Length > 0)
			foreach(GameObject go in TentenMaruGOs)
			{
				CanvasGroup cg = go.GetComponent<CanvasGroup>();
				cg.blocksRaycasts = false;
			}
	}
	
	void EnableDragable ()
	{
		GameObject[] HiraganaGOs = GameObject.FindGameObjectsWithTag ("Hiragana");
		GameObject[] KatakanaGOs = GameObject.FindGameObjectsWithTag ("Katakana");
		GameObject[] TentenMaruGOs = GameObject.FindGameObjectsWithTag ("TentenMaru");
		
		if (HiraganaGOs.Length > 0)
			foreach(GameObject go in HiraganaGOs)
			{
				CanvasGroup cg = go.GetComponent<CanvasGroup>();
				cg.blocksRaycasts = true;
			}
		
		if (KatakanaGOs.Length > 0)
				foreach(GameObject go in KatakanaGOs)
			{
				CanvasGroup cg = go.GetComponent<CanvasGroup>();
				cg.blocksRaycasts = true;
			}
		
		if (TentenMaruGOs.Length > 0)
			foreach(GameObject go in TentenMaruGOs)
			{
				CanvasGroup cg = go.GetComponent<CanvasGroup>();
				cg.blocksRaycasts = true;
			}
	}
}
