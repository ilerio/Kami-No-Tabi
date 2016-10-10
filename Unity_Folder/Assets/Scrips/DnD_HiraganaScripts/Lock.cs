using UnityEngine;
using System.Collections;

public class Lock : MonoBehaviour
{
	public GameObject UI;
	public GameObject GM;
	public GameObject pauseButton;
	public AudioSource LockAudioSource;

	private GameControllerDnDHiragana GCDnD;

	private Color glow;
	private bool swing_r = false;
	private bool swing_g = false;
	private bool hover = false;

	void Awake()
	{
		GCDnD = GameControllerDnDHiragana.FindObjectOfType (typeof (GameControllerDnDHiragana)) as GameControllerDnDHiragana;
		glow = this.GetComponent<SpriteRenderer>().color;
	}

	void Update()
	{
		if (GCDnD.AllFilledIn == true)
		{
			Glow();
		}
	}

	void OnMouseDown()
	{
		bool win = GCDnD.CheckWinState();

		if (win)
		{
			this.gameObject.SetActive(false);
			Destroy(GM.gameObject);
			pauseButton.SetActive(false);
			LockAudioSource.Play();
			Invoke("NextLevel", 8f);
		}
		else
			GCDnD.WrongAnswer();
	}

	void OnMouseEnter()
	{
		hover = true;
		this.GetComponent<SpriteRenderer>().color = Color.green;
	}

	void OnMouseExit()
	{
		this.GetComponent<SpriteRenderer>().color = Color.yellow;
		hover = false;
	}

	void NextLevel()
	{
		Destroy(UI.gameObject);

		PlayerPrefs.SetInt("Level_02",1);

		LoadingScreen ls = GameObject.FindObjectOfType (typeof (LoadingScreen)) as LoadingScreen;
		ls.LoadScene("DnD_Hiragana_Transition");
	}

	void Glow()
	{
		if (glow.r < 1 && swing_r == false && hover == false)
		{
			glow.r += Time.deltaTime;
			this.GetComponent<SpriteRenderer>().color = glow;
			
			if (glow.r > 1){swing_r = true;}
		}
		else if (hover == false)
		{
			glow.r -= Time.deltaTime;
			this.GetComponent<SpriteRenderer>().color = glow;
			
			if (glow.r <= 0){swing_r = false;}
		}
		
		if (glow.g < 1 && swing_g == false && hover == false)
		{
			glow.g += Time.deltaTime;
			this.GetComponent<SpriteRenderer>().color = glow;
			
			if (glow.b > 1){swing_g = true;}
		}
		else if (hover == false)
		{
			glow.g -= Time.deltaTime;
			this.GetComponent<SpriteRenderer>().color = glow;
			
			if (glow.b <= 0){swing_g = false;}
		}
	}
}
