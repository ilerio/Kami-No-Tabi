using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour 
{
	public string levelToLoad;

	public GameObject backgroung;
	public GameObject text;
	public GameObject progressBar;

	private int loadProgress = 0;
	
	// Use this for initialization
	void Start () 
	{
		backgroung.SetActive (false);
		text.SetActive (false);
		progressBar.SetActive (false);

		//DisplayLoadScreen();
	}

	public void LoadScene(string level)
	{
		StartCoroutine (DisplayLoadScreen(level));
	}

	IEnumerator DisplayLoadScreen(string level)
	{
		backgroung.SetActive (true);
		text.SetActive (true);
		progressBar.SetActive (true);

		progressBar.transform.localScale = new Vector3(loadProgress,progressBar.transform.localScale.y, progressBar.transform.localScale.z);

		text.GetComponent<GUIText>().text = "Loading... " + loadProgress + "%";

		AsyncOperation async = Application.LoadLevelAsync (level);

		while(!async.isDone)
		{
			loadProgress = (int) (async.progress  * 100);
			text.GetComponent<GUIText>().text = "Loading Progress " + loadProgress + "%";
			progressBar.transform.localScale = new Vector3(async.progress,progressBar.transform.localScale.y, progressBar.transform.localScale.z);

			yield return null;
		}
	}
}
