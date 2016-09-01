using UnityEngine;
using System.Collections;

public class LevelSelection : MonoBehaviour 
{
	public GameObject lsButton;
	public GameObject lsPanel;

	public GameObject level_01;
	public GameObject level_02;
	public GameObject level_03;
	public GameObject dnd_Hiragana;
	public GameObject dnd_Katakana;

	private bool isDisplayed; 

	void Awake()
	{
		int levels = 0;

		lsPanel.SetActive (false);
		isDisplayed = false;

		//Not yet made
		level_03.SetActive(false);

		if (PlayerPrefs.GetInt("Level_01") == 1)
		{
			level_01.SetActive(true); 
			levels++;
		}
		else
		{
			level_01.SetActive(false);
		}

		if (PlayerPrefs.GetInt("Level_02") == 1)
		{
			level_02.SetActive(true); 
			levels++;
		}
		else
		{
			level_02.SetActive(false);
		}

		if (PlayerPrefs.GetInt("Level_03") == 1)
		{
			level_03.SetActive(true); 
			levels++;
		}
		else
		{
			level_03.SetActive(false);
		}

		if (PlayerPrefs.GetInt("DnD_Hiragana") == 1)
		{
			dnd_Hiragana.SetActive(true);
			levels++;
		}
		else
		{
			dnd_Hiragana.SetActive(false);
		}

		if (PlayerPrefs.GetInt("DnD_Katakana") == 1)
		{
			dnd_Katakana.SetActive(true);
			levels++;
		}
		else
		{
			dnd_Katakana.SetActive(false);
		}

		if (levels < 1)
			lsButton.SetActive(false);
	}

	public void ToggleLevelSelectionPanel()
	{
		if(!isDisplayed)
		{
			isDisplayed = true;
			lsPanel.SetActive(true);
			lsButton.SetActive(false);
		}
		else
		{
			isDisplayed = false;
			lsPanel.SetActive(false);
			lsButton.SetActive(true);
		}
	}
}
