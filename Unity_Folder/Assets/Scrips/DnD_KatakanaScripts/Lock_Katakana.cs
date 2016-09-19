using UnityEngine;
using System.Collections;

public class Lock_Katakana : MonoBehaviour
{
	public GameObject UI;
	public GameObject GM;
	public GameObject pauseButton;
	public AudioSource LockAudioSource;
	
	private GamecontrollerDnDKatakana GCDnD;

	private Color glow;
	private bool swing_r = false;
	private bool swing_b = false;
	private bool hover = false;
	
	void Awake()
	{
		GCDnD = GamecontrollerDnDKatakana.FindObjectOfType (typeof (GamecontrollerDnDKatakana)) as GamecontrollerDnDKatakana;
		glow = this.GetComponent<SpriteRenderer>().color;
		
		Debug.Log("b (blue) = " + glow.b);
		Debug.Log("g (green) = " + glow.g);
		Debug.Log("r (red) = " + glow.r);
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
		
		if (win == true)
		{
			this.gameObject.SetActive(false);
			pauseButton.SetActive(false);
			Destroy(GM.gameObject);
			LockAudioSource.Play();
			Invoke("NextLevel", 1.7f);
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
		this.GetComponent<SpriteRenderer>().color = Color.white;
		hover = false;
	}
	
	void NextLevel()
	{
		Destroy(UI.gameObject);

		PlayerPrefs.SetInt("DnD_Katakana", 1);
		
		LoadingScreen ls = GameObject.FindObjectOfType (typeof (LoadingScreen)) as LoadingScreen;
		ls.LoadScene("DnD_Katakana_Transition");
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
		
		if (glow.b < 1 && swing_b == false && hover == false)
		{
			glow.b += Time.deltaTime;
			this.GetComponent<SpriteRenderer>().color = glow;
			
			if (glow.b > 1){swing_b = true;}
		}
		else if (hover == false)
		{
			glow.b -= Time.deltaTime;
			this.GetComponent<SpriteRenderer>().color = glow;
			
			if (glow.b <= 0){swing_b = false;}
		}
	}
}
