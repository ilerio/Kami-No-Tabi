using UnityEngine;
using System.Collections;

public class NextScene : MonoBehaviour 
{
	public GameObject ui;
	public string sceneChoice;

	void Awake()
	{
		ui.SetActive(true);
	}

	public void LoadSceneChoice()
	{
		PlayerPrefs.SetInt("Level_01",1);

		ui.SetActive (false);

		LoadingScreen ls = GameObject.FindObjectOfType (typeof (LoadingScreen)) as LoadingScreen;
		ls.LoadScene(sceneChoice);
	}

	/*public void LoadNextScene()
	{
		Application.LoadLevel(Application.loadedLevel + 1);
	}*/

	public void Credits()
	{
		Application.LoadLevel("Credits");
	}
}
