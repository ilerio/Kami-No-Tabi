using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour
{
	void Start () 
	{
		StartCoroutine (ChangeLevel());
	}

	IEnumerator ChangeLevel()
	{
		// fade out the game and load a new level
		yield return new WaitForSeconds(2f);	// waits for animation to stop playing
		LoadingScreen ls = LoadingScreen.FindObjectOfType (typeof (LoadingScreen)) as LoadingScreen;
		ls.LoadScene("Story");
	}
}