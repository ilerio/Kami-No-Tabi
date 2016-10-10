using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour 
{
	public float speed = 1f;
	public AudioSource musicPlayer;

	private float quit = 1f;
	private bool fadeBegun = false;
	private bool exit = false;

	void Start()
	{
		// This is to adress a slowdown in speen when ported to Android
		#if UNITY_ANDROID
		speed = 100f;
		#endif
		this.GetComponent<Rigidbody2D>().velocity = new Vector2(0,speed);
	}

	void Update () 
	{
		//this.transform.Translate(Vector3.up * Time.deltaTime * speed); // This seems to have a variable speed depending on screen size

		if(exit)
		{
			musicPlayer.volume = musicPlayer.volume - (Time.deltaTime);
			quit = quit - (Time.deltaTime);
			beginFade();
			if(musicPlayer.volume <= 0 && quit <= 0)
			{
				exit = false; // to prevent a double call
				Debug.Log("Exit Called.");
				Invoke("BackToMainMenu",0.01f);
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
