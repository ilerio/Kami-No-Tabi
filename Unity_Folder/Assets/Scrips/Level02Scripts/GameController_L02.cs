using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController_L02 : MonoBehaviour // to debug uncomment all debug.log statements
{
	public Sprite[] hiragana;
	public Sprite[] katakana;
	public SpriteRenderer hiraganaDisplay;
	public GameObject gameOverScreen;
	public GameObject nextLevelButton;
	public GameObject Win_GO;
	public GameObject GO_text;
	public Text WIN_text;
	public AudioClip wrongAnswer;
	public AudioClip correctAnswer;
	public Text katakanaCount;
	
	private int[] usedKatakana; 		// Uses the arrayLocation of the katakana to store wether or not that katakana has been used by sotting 
										// either "0" for not used or "1" for used at that location.
	
	private int[] inUseRound;
	
	private int answer = -1; 			// This is the location in the array of the correct katakana scrip (aka the corrct answer).
	private int katakanaLocateion = -1;	// This is the number of the location of correct katakana with the answer on it's head. 
										// going from left to right {1, 2, 3}.
	
	private int katakanaCallNum = 0; 	// keeps track of how many times the KatakanaSetUp, 
										// method has been called. Should never be over 3.
										// Should always be reset after every cycle of answers.
	
	private int countUsedKatakana = 0;	// Will keep track of totall used katakana.

	private int countConsecutiveCorrect;

	private AudioSource GMAudioSource;
	private GameSetup_L02 gameSetup;
	private Lives lives;
	private CameraShake shake;
	private RedFlash flash;
	private Stack<int> katakanaNumStack = new Stack<int>();
	
	void Awake () 
	{
		int[] check = new int[46];
		int randomNumber;
		int loopCount = 0;
		
		while(katakanaNumStack.Count < 46)
		{
			randomNumber = Random.Range(0, katakana.Length);
			bool flag = true;
			
			if(check[randomNumber] != 0)
				flag = false;
			
			if(flag == true)
			{
				katakanaNumStack.Push(randomNumber);
				//Debug.Log (randomNumber + " selected.");
				check[randomNumber] = -1;
			}
			/*else//debugging purposes
				Debug.Log ("DENIED!");*/
			
			loopCount ++;
		}
		
		Debug.Log ("pre-selection of integers done! " + loopCount + " loop throughs.");

		countUsedKatakana = 0;
		inUseRound = new int[3];
		usedKatakana = new int[katakana.Length]; // Initializes to number of katakana (will always be 46).
		//Debug.Log ("[GC_Awake] katakana.Length = " + katakana.Length);

		GMAudioSource = gameObject.GetComponent<AudioSource>();

		gameSetup = GameObject.FindObjectOfType (typeof (GameSetup_L02)) as GameSetup_L02;
		lives = GameObject.FindObjectOfType (typeof (Lives)) as Lives;
		shake = GameObject.FindObjectOfType (typeof (CameraShake)) as CameraShake;
		flash = GameObject.FindObjectOfType (typeof (RedFlash)) as RedFlash;
		
		InstantiateRound ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Check heart-state for game-over
		if (lives.getNumLives() <= 0)
		{
			GameOverDisplay();
		}
	}
	
	void InstantiateRound()
	{
		katakanaCallNum = 0;
		for (int i = 0; i < inUseRound.Length; i++) // for katakana
			inUseRound[i] = -1;

		answer = katakanaNumStack.Pop(); // use pre-determined katakana number as answer

		//Debug.Log ("[GC_IR] answer = " + answer);
		
		// choose a character (for each game instance)
		katakanaLocateion = Random.Range (1, 4); // On which katakana will the correct answer be placed {1, 2, 3}
		//Debug.Log ("[GC_IR] katakanaLocation: " + katakanaLocateion);

		hiraganaDisplay.sprite = hiragana[answer];
	}
	
	//Chooses a character for the begining of each round
	public int KatakanaSetUp()
	{
		katakanaCallNum++;
		Debug.Log ("[GC_KatakanaSetup] katakanaCallNum = " + katakanaCallNum);
		bool done = false;
		int randomNumber = -1;
		
		if (katakanaCallNum != katakanaLocateion) 
		{
			while (done != true) 
			{
				// will need to be changed!! -> Why?
				randomNumber = Random.Range (0, katakana.Length);
				//Debug.Log ("[GC_KatakanaSetup] randomNumber = " + randomNumber);
				if (randomNumber != answer)
					done = true;
				else done = false;
				
				for (int i = 0;i<inUseRound.Length;i++)
				{
					Debug.Log ("inUseRound[" + i + "] = " + inUseRound[i]);
					if (inUseRound[i] == randomNumber)
					{
						done = false;
					}
				}
			}
			
			inUseRound[katakanaCallNum-1] = randomNumber;
			
			/*/ Debug start:
			string tempVar = "";
			foreach(int i in inUseRound)
				tempVar += "(" + i + ")";
			
			Debug.Log("inUseRound[] = " + tempVar);
			// Debug end.*/
			
		} else 
		{
			Debug.Log (" [GC_KatakanaSetup] I katakana #" + katakanaCallNum + " am the katakana with the answer (" + answer + ").");
			usedKatakana [answer] = 1;
			inUseRound[katakanaCallNum-1] = answer;
			return answer;
		}
		
		if (katakanaCallNum > 3)
			Debug.Log ("Error: count overflow; too many katakana are instantialized!");
		
		return randomNumber;
	}
	
	/* Checkes if a katakana charicter has already been used, by taking in the position of the 
	// character then returning a boolian true/false value depending on if the passed in character 
	// is in the usedKatakana array.
	// Returns true if used or in the usedKatakana array or false if otherwise.*/
	public bool CheckUsed(int arrayPosition)
	{
		Debug.Log ("[GC_CheckUsed] arrayPosition = " + arrayPosition);
		Debug.Log ("[GC_CheckUsed] usedKatakana[arrayPosition] = " + usedKatakana[arrayPosition]);
		
		if (usedKatakana[arrayPosition] == 0)
			return false;
		else
			return true;
	}
	
	// Checks if chosen answer is correct and checks winstate
	public void CheckCorrect(string name, GameObject go)
	{
		int temp = name.CompareTo("Katakana" + katakanaLocateion);
		
		if (temp == 0)
		{
			// correct
			countUsedKatakana++;
			bool win = CheckWinState();

			katakanaCount.text = (countUsedKatakana) + " / 46";

			if (win)
				return;
			else
				CorrectAnswer ();
			
			/*/ Debug start:
			string tempVar = "";
			for(int i = 0; i < usedKatakana.Length; i++)
			{
				tempVar += "(" + i + ": " + usedKatakana[i] + ")";
			}
			
			Debug.Log("usedKatakana[] = " + tempVar);
			Debug.Log("countUsedKatakana = " + countUsedKatakana);
			// Debug end.*/
		}
		else
		{
			// not correct
			WrongAnswer ();
		}
	}
	
	void CorrectAnswer()
	{
		countConsecutiveCorrect++;

		if(countConsecutiveCorrect % 15 == 0)
			lives.AddLife(); // Adds a life if 15 in a row is achived

		GMAudioSource.clip = correctAnswer;
		GMAudioSource.Play ();

		GameObject[] KatakanaGOs = GameObject.FindGameObjectsWithTag ("Katakana");
		foreach(GameObject go in KatakanaGOs)
		{
			Destroy (go);
		}
		
		// reset katakanaCallNum
		// reselect win katakana
		InstantiateRound ();
		
		gameSetup.DrawKatakana(); // Disable all clickable components
	}

	IEnumerator transition() // add transition animation (possibility)
	{
	
		return null;
	}
	
	void WrongAnswer()
	{
		countConsecutiveCorrect = 0;

		GMAudioSource.clip = wrongAnswer;
		GMAudioSource.Play ();

		shake.Shake (0.08f, 0.2f); // shakes screen slightly
		flash.Flash(); // Flashes a red screen
		lives.DecLife(); // Removes a life
	}
	
	void GameOverDisplay()
	{
		gameSetup.DisableAll(); // Disable all clickable components
		
		GameObject[] KatakanaGOs = GameObject.FindGameObjectsWithTag ("Katakana");
		foreach(GameObject go in KatakanaGOs)
		{
			Destroy (go);
		}
		
		shake.Shake (0.08f, 0.2f); // shakes screen slightly
		flash.TurnOn(); // Turns on red screen
		
		gameOverScreen.SetActive(true);
	}
	
	void WinScreenDisplay()
	{
		PlayerPrefs.SetInt("DnD_Katakana", 1);

		gameSetup.DisableAll(); // Disable all clickable components
		
		GameObject[] KatakanaGOs = GameObject.FindGameObjectsWithTag ("Katakana");
		foreach(GameObject go in KatakanaGOs)
		{
			Destroy (go);
		}
		
		GO_text.SetActive(false);
		Win_GO.SetActive(true);
		WIN_text.text = "おめでとう!";
		nextLevelButton.SetActive(true);
		gameOverScreen.SetActive(true);
		
		GameObject GM = GameObject.FindGameObjectWithTag("GM");
		Destroy(GM);
	}
	
	public int GetNum()
	{
		return katakanaCallNum;
	}	
	
	bool CheckWinState()
	{
		if (countUsedKatakana == katakana.Length)
		{
			WinScreenDisplay ();
			return true;
		} else return false;
	}
}