using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class Level3_Transition : MonoBehaviour 
{
	public Sprite[] slides;
	public Image display;
	public AudioSource soundEffect;
	
	private int currentSlide;
	private Fading fade;
	private float quit = 2f;
	private bool fadeBegun = false;
	private bool exit;
	
	void Awake()
	{
		fade = this.GetComponent<Fading>();
		currentSlide = 0;
		StartCoroutine (Play());
	}

	void Update()
	{
		if(exit)
		{
			soundEffect.volume = soundEffect.volume - (Time.deltaTime);
			quit = quit - (Time.deltaTime);
			beginFade();
			if(soundEffect.volume <= 0 && quit <= 0)
			{
				Debug.Log("Exit Called.");
				Invoke("LoadNextLevel",0.001f);
			}
		}
	}

	IEnumerator Play () 
	{
		yield return new WaitForSeconds(2f);
		
		for (int i = 0; i < 2; i++)
		{
			if(i == 1)
				soundEffect.Play();

			yield return new WaitForSeconds(1f);
			currentSlide ++;
			display.sprite = slides [currentSlide];
			yield return new WaitForSeconds(1f);
		}

		// Pause for soundeffect
		yield return new WaitForSeconds(7f);
		fade.BeginFade(1);
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

	public void LoadNextLevel()
	{
		Application.LoadLevel("Story_End");
	}
}