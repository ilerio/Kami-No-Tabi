using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour 
{
	public float speed = 1f;
	public AudioSource musicPlayer;

	private float quit = 1f;
	private bool fadeBegun = false;
	private bool exit = false;

	void Update () 
	{
		this.transform.Translate(Vector3.up * Time.deltaTime * speed);

		if(exit)
		{
			musicPlayer.volume = musicPlayer.volume - (Time.deltaTime);
			quit = quit - (Time.deltaTime);
			beginFade();
			if(musicPlayer.volume <= 0 && quit <= 0)
			{
				Debug.Log("Exit Called.");
				Invoke("BackToMainMenu",0.001f);
			}
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		exit = true;
	}

	public void beginFade()
	{
		if(!fadeBegun)
		{
			fadeBegun = true;
			Fading fade = GetComponent<Fading>();
			fade.BeginFade(1);
		}
	}

	public void BackToMainMenu()
	{
		Application.LoadLevel("MainMenue");
	}
}
