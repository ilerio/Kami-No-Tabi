using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour 
{
	public int speed = 1;
	public int endpos; 
	public AudioSource musicPlayer;

	private float quit = 1f;
	private bool fadeBegun = false;

	void Update () 
	{
		this.transform.Translate(Vector3.up * Time.deltaTime * speed);

		if(this.transform.position.y > endpos)
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
