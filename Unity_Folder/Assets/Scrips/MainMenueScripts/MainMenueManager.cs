using UnityEngine;
using System.Collections;

public class MainMenueManager : MonoBehaviour 
{
	public GameObject ui;

	private string sceneChoice;

	void Awake()
	{
		ui.SetActive(true);

		if (PlayerPrefs.GetInt("Done") == 1)
		{
			sceneChoice = "Level_01";
		} 
		else if (PlayerPrefs.GetInt("Level_03") == 1)
		{
			sceneChoice = "Level_03";
		} 
		else if (PlayerPrefs.GetInt("DnD_Katakana") == 1)
		{
			sceneChoice = "DnD_Katakana";
		} 
		else if (PlayerPrefs.GetInt("Level_02") == 1)
		{
			sceneChoice = "Level_02";
		} 
		else if (PlayerPrefs.GetInt("DnD_Hiragana") == 1)
		{
			sceneChoice = "DnD_Hiragana";
		} 
		else if (PlayerPrefs.GetInt("Level_01") == 1)
		{
			sceneChoice = "Level_01";
		}
		else
		{
			sceneChoice = "Level_01";
		}
	}

	public void LoadSceneChoice()
	{
		PlayerPrefs.SetInt("Level_01",1);

		ui.SetActive (false);

		LoadingScreen ls = GameObject.FindObjectOfType (typeof (LoadingScreen)) as LoadingScreen;
		ls.LoadScene(sceneChoice);
	}

	public void Credits()
	{
		Application.LoadLevel("Credits");
	}
}
