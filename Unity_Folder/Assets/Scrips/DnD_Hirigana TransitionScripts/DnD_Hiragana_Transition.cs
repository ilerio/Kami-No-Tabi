﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class DnD_Hiragana_Transition : MonoBehaviour 
{
	public Sprite[] slides;
	public Image display;
	public Text textOutput;
	public Canvas textCanvas;
	public Text storyTextObject;
	public static LoadingScreen ls;
	public GameObject storyCanvasObject;
	public GameObject textCanvasObject;
	
	public float textScrollSpeed;
	
	private string[] storyArray_Hiragana;
	private string[] storyArray_Romanji;
	private string[] storyArray_English;
	
	private int currentLine;
	private int currentSlide;
	private bool playing; 
	private bool textIsScrolling;
	private bool english;
	private bool romanji;
	private bool romanjiStoryTurn;
	private bool englishStroryTurn;
	private AudioSource audioSource;
	private bool first;
	
	void Awake()
	{
		// Prep
		string temp = storyTextObject.text;
		string[] storyArray = temp.Split('*');
		
		storyArray_Hiragana = storyArray[0].Split ('-');
		storyArray_Romanji = storyArray[1].Split ('-');
		storyArray_English = storyArray[2].Split ('-');

		ls = LoadingScreen.FindObjectOfType (typeof (LoadingScreen)) as LoadingScreen;
		first = true;
		Invoke("StartGame",0.1f);
	}
	
	public void StartGame()
	{
		if (PlayerPrefs.GetInt ("englishSubtitles", 0) == 1)
		{
			english = true;
		}
		else
		{
			english = false;
		}
		
		if (PlayerPrefs.GetInt ("romanjiSubtitles", 0) == 1)
		{
			romanji = true;
		}
		else
		{	
			romanji = false;
		}
		
		Debug.Log ("English = " + english);
		Debug.Log ("Romanji = " + romanji);

		romanjiStoryTurn = false;
		englishStroryTurn = false;
		playing = true;
		currentSlide = 0;
		currentLine = -1;

		audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.Play ();
		StartCoroutine (PlayText ());

	}
	
	IEnumerator PlayText()
	{
		yield return new WaitForSeconds (0.1f);
		textCanvas.enabled = true;
	}
	
	void Update()
	{
		if (!((englishStroryTurn && english) || (romanjiStoryTurn && romanji)))
		{
			//Debug.Log ("Hiragana!");
			// Hiragana story
			// Implement scrolling
			if (playing) 
			{
				if (Input.GetMouseButtonUp (0) || first) 
				{
					first = false; // to play the text on scene start.
					if (textIsScrolling) 
					{
						// display full line 
						textOutput.text = storyArray_Hiragana [currentLine];
						textIsScrolling = false;
					} 
					else 
					{
						// display next line
						if (currentLine < storyArray_Hiragana.Length -1) 
						{
							NextLineAndNextSlide ();
							
							Debug.Log ("Story_H = " + storyArray_Hiragana [currentLine]);
							
							StartCoroutine (StartScrollingHiragana ());
							
						} 
						else 
						{
							currentLine = 0;
							textOutput.text = "";
							playing = false;
							Debug.Log ("Hiragana Side!");
							PlayerPrefs.SetInt("skipEnabled", 1);
							StartCoroutine(TransitionNextScene ());
						}
					}
				}
			}
		}
		else if (romanji && romanjiStoryTurn)
		{
			//Debug.Log ("Romanji!");
			// Romanji story
			// Implement scrolling
			if (playing)
			{
				if (Input.GetMouseButtonUp (0))
				{
					if (textIsScrolling)
					{
						// display full line 
						textOutput.text = storyArray_Romanji [currentLine];
						textIsScrolling = false;
					} 
					else 
					{
						// display next line
						if (currentLine < storyArray_Romanji.Length) 
						{
							Debug.Log ("Story_R = " + storyArray_Romanji [currentLine]);
							
							StartCoroutine (StartScrollingRomanji ());
							
						} 
						else 
						{
							currentLine = 0;
							textOutput.text = "";
							playing = false;
							Debug.Log ("Romanji Side!");
							StartCoroutine(TransitionNextScene ());
						}
					}
				}
			}
		}
		else if (english && englishStroryTurn) 
		{
			//Debug.Log ("English!");
			// English story
			// Implement scrolling
			if (playing) 
			{
				if (Input.GetMouseButtonUp (0)) 
				{
					if (textIsScrolling) 
					{
						// display full line 
						textOutput.text = storyArray_English [currentLine];
						textIsScrolling = false;
					} else 
					{
						// display next line
						if (currentLine < storyArray_English.Length) 
						{
							Debug.Log ("Story_E = " + storyArray_English [currentLine]);
							
							StartCoroutine (StartScrollingEnglish ());
							
						} else
						{
							currentLine = 0;
							textOutput.text = "";
							playing = false;
							Debug.Log ("English Side!");
							StartCoroutine(TransitionNextScene ());
						}
					}
				}
			}
		}
		
		if (textIsScrolling)
		{
			audioSource.UnPause();
		}
		else if (!textIsScrolling && audioSource != null)
		{
			audioSource.Pause ();
		}
	}
	
	void NextLineAndNextSlide()
	{
		currentLine++;
		
		if (currentLine > 0 && currentLine < 7)
		{
			currentSlide ++;
			display.sprite = slides [currentSlide];
		}
	}
	
	IEnumerator StartScrollingHiragana()
	{
		textIsScrolling = true;
		
		Debug.Log ("Hiragana story started.");
		Debug.Log ("[H] Current line = " + currentLine);
		
		string displayText = "";
		int startLine = currentLine;
		
		for (int i = 0; i < storyArray_Hiragana[currentLine].Length; i++)
		{
			//Debug.Log ("[H] Current line = " + currentLine);
			if (textIsScrolling && currentLine == startLine) 
			{
				displayText += storyArray_Hiragana [currentLine] [i];
				textOutput.text = displayText;
				yield return new WaitForSeconds (textScrollSpeed);
			}
		}
		
		textIsScrolling = false;
		
		if (english && romanji && !textIsScrolling)
			romanjiStoryTurn = true;
		else if (english && !romanji && !textIsScrolling)
			englishStroryTurn = true;
		else if (romanji && !english && !textIsScrolling)
			romanjiStoryTurn = true;
		else if (!english && !romanji && !textIsScrolling)
		{
			romanjiStoryTurn = false;
			englishStroryTurn = false;
		}
		
		Debug.Log ("Hiragana stoty ended.");
	}
	
	IEnumerator StartScrollingRomanji()
	{
		textIsScrolling = true;
		
		Debug.Log ("Romanji story started.");
		Debug.Log ("[R] Current line = " + currentLine);
		
		string displayText = "";
		int startLine = currentLine;
		
		for (int i = 0; i < storyArray_Romanji[currentLine].Length; i++)
		{
			if (textIsScrolling && currentLine == startLine) 
			{
				displayText += storyArray_Romanji [currentLine] [i];
				textOutput.text = displayText;
				yield return new WaitForSeconds (textScrollSpeed);
			}
		}
		
		textIsScrolling = false;
		
		if (!textIsScrolling)
		{
			romanjiStoryTurn = false;
			englishStroryTurn = true;
		}
		
		Debug.Log ("Romanji stoty ended.");
	}
	
	IEnumerator StartScrollingEnglish()
	{
		textIsScrolling = true;
		
		Debug.Log ("English story started.");
		Debug.Log ("[E] Current line = " + currentLine);
		
		string displayText = "";
		int startLine = currentLine;
		
		for (int i = 0; i < storyArray_English[currentLine].Length; i++)
		{
			if (textIsScrolling && currentLine == startLine) 
			{
				displayText += storyArray_English [currentLine] [i];
				textOutput.text = displayText;
				yield return new WaitForSeconds (textScrollSpeed);
			}
		}
		
		textIsScrolling = false;
		
		if (!textIsScrolling)
			englishStroryTurn = false;
		
		Debug.Log ("English stoty ended.");
	}

	IEnumerator TransitionNextScene () 
	{
		yield return new WaitForSeconds(0.1f);
		textCanvasObject.SetActive(false);
		storyCanvasObject.SetActive(false);
		
		ls.LoadScene("Level_02");
	}
}