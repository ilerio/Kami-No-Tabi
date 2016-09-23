using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour // to debug uncomment all debug.log statements
{
	public Sprite[] hiragana;
	public AudioClip[] hiraganaSounds; 
	public GameObject gameOverScreen;
	public GameObject Win_GO;
	public GameObject GO_text;
	public GameObject nextLevelButton;
	public Text romanji;
	public Text WIN_text;
	public Text hiraganaCount;

	private int[] usedHiragana; 		// Uses the arrayLocation of the Hiragana to store wether or not that hiragana has been used by sotting 
										// either "0" for not used or "1" for used at that location.

	private int[] inUseRound;

	private int answer = -1; 			// This is the location in the array of the correct hiragana scrip (aka the corrct answer).
	private int oniLocateion = -1;		// This is the number of the location of correct oni with the answer on it's head; 
										// going from left to right {1, 2, 3}.

	private int hiraganaCallNum = 0; 	// keeps track of how many times the HiraganaSetUp, 
								 	 	// method has been called. Should never be over 3.
										// Should always be reset after every cycle of answers;

	private int countUsedHiragana = 0;	// Will keep track of totall HiraganaSetUp() calls;

	private AudioSource GMAudioSource;
	private Timer timer;
	private GameSetup gameSetup;
	private CameraShake shake;
	private RedFlash flash;
	private Stack<int> hiraganaNumStack = new Stack<int>();
 	
	void Awake () 
	{
		int[] check = new int[46];
		int randomNumber;
		int loopCount = 0;

		while(hiraganaNumStack.Count < 46)
		{
			randomNumber = Random.Range(0, hiragana.Length);
			bool flag = true;

			if(check[randomNumber] != 0)
				flag = false;

			if(flag == true)
			{
				hiraganaNumStack.Push(randomNumber);
				//Debug.Log (randomNumber + " selected.");
				check[randomNumber] = -1;
			}
			/*else
				Debug.Log ("DENIED!");*/

			loopCount ++;
		}
		
		Debug.Log ("pre-selection of integers done! " + loopCount + " loop throughs.");

		countUsedHiragana = 0;
		inUseRound = new int[3];
		usedHiragana = new int[hiragana.Length]; // Initializes to number of hiragana (will always be 46).
		//Debug.Log ("[GC_Awake] hiragana.Length = " + hiragana.Length);
		GMAudioSource = gameObject.GetComponent<AudioSource>();

		timer = Timer.FindObjectOfType (typeof (Timer)) as Timer;
		gameSetup = GameSetup.FindObjectOfType (typeof (GameSetup)) as GameSetup;
		shake = CameraShake.FindObjectOfType (typeof (CameraShake)) as CameraShake;
		flash = RedFlash.FindObjectOfType (typeof (RedFlash)) as RedFlash;
		
		InstantiateRound ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Checks timer for game state
		if (timer.getCountingTime () <= 0)
		{
			Invoke("GameOverDisplay",0.1f);
		}
	}

	void InstantiateRound()
	{
		hiraganaCallNum = 0;
		for (int i = 0; i < inUseRound.Length; i++)
			inUseRound[i] = -1;

		answer = hiraganaNumStack.Pop(); // use pre-determined hiragana number as answer

		//Debug.Log ("[GC_IR] answer = " + answer);

		// choose a character (for each game instance)
		oniLocateion = Random.Range (1, 4); // On which oni will the correct answer be placed {1, 2, 3}
		//Debug.Log ("[GC_IR] oniLocation: " + oniLocateion);

		GMAudioSource.clip = hiraganaSounds[answer];
		GMAudioSource.Play ();

		romanji.text = hiragana[answer].name;
	}

	//Chooses a character for the begining of each round
	public int HiraganaSetUp()
	{
		hiraganaCallNum++;
		Debug.Log ("[GC_HiraganaSetup] hiraganaCallNum = " + hiraganaCallNum);
		bool done = false/*, check = true*/;
		int randomNumber = -1;

		if (hiraganaCallNum != oniLocateion) 
		{
			while (done != true) 
			{
				randomNumber = Random.Range (0, hiragana.Length);
				//Debug.Log ("[GC_HiraganaSetup] randomNumber = " + randomNumber);
				if (randomNumber != answer)
					done = true;
				else done = false;

				for (int i = 0;i<inUseRound.Length;i++)
				{
					Debug.Log ("inUseRound[" + i + "] = " + inUseRound[i]);

					if (inUseRound[i] == randomNumber)
						done = false;

					//If statement to check for O / Wo conflict
					if ((inUseRound[i] == 25 && randomNumber == 42) || (inUseRound[i] == 42 && randomNumber == 25))
						done = false;
				}
			}

			inUseRound[hiraganaCallNum-1] = randomNumber;

			/*/ Debug start:
				string tempVar = "";
			foreach(int i in inUseRound)
				tempVar += "(" + i + ")";
			
			Debug.Log("inUseRound[] = " + tempVar);
			// Debug end.*/

		} else 
		{
			Debug.Log (" [GC_HiraganaSetup] I oni #" + hiraganaCallNum + " am the oni with the answer (" + answer + ").");
			usedHiragana [answer] = 1;
			inUseRound[hiraganaCallNum-1] = answer;
			return answer;
		}

		if (hiraganaCallNum > 3)
			Debug.Log ("Error: count overflow; too many oni are instantialized!");
 
		return randomNumber;
	}

	// Checks winstate
	public void CheckCorrect(string name, GameObject go)
	{
		int temp = name.CompareTo("Oni" + oniLocateion);

		if (temp == 0)
		{
			// correct
			countUsedHiragana++;
			bool win = CheckWinState();

			hiraganaCount.text = (countUsedHiragana) + " / 46";

			if (win)
				return;
			else
				CorrectAnswer ();

			/*/ Debug start:
			string tempVar = "";
			for(int i = 0; i < usedHiragana.Length; i++)
			{
				tempVar += "(" + i + ": " + usedHiragana[i] + ")";
			}

			Debug.Log("usedHiragana[] = " + tempVar);
			Debug.Log("countUsedHiragana = " + countUsedHiragana);
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
		timer.AddTime(); // Adds a set amount of time

		GameObject[] OniGOs = GameObject.FindGameObjectsWithTag ("Oni");
		foreach(GameObject go in OniGOs)
		{
			Destroy (go);
		}
	
		// reset hiraganaCallNum
		// reselect win oni
		InstantiateRound ();

		gameSetup.DrawOni(); // Disable all clickable components
	}

	void WrongAnswer()
	{
		GMAudioSource.Play ();
		shake.Shake (0.08f, 0.2f); // shakes screen slightly
		flash.Flash(); // Flashes a red screen
		timer.DecTime(); // Removs a set amount of time
	}

	void GameOverDisplay()
	{
		gameSetup.DisableAll(); // Disable all clickable components

		GameObject[] OniGOs = GameObject.FindGameObjectsWithTag ("Oni");
		foreach(GameObject go in OniGOs)
		{
			Destroy (go);
		}

		shake.Shake (0.08f, 0.2f); // shakes screen slightly
		flash.TurnOn(); // Turns on red screen

		nextLevelButton.SetActive(false);
		gameOverScreen.SetActive(true);
	}

	bool CheckWinState()
	{
		if (countUsedHiragana == hiragana.Length)
		{
			WinScreenDisplay ();
			return true;
		} else return false;
	}

	void WinScreenDisplay()
	{
		PlayerPrefs.SetInt("DnD_Hiragana",1);

		gameSetup.DisableAll(); // Disable all clickable components
		
		GameObject[] OniGOs = GameObject.FindGameObjectsWithTag ("Oni");
		foreach(GameObject go in OniGOs)
		{
			Destroy (go);
		}

		GO_text.SetActive(false);
		Win_GO.SetActive(true);
		nextLevelButton.SetActive(true);
		WIN_text.text = "おめでとう!\nスコア: " + timer.getCountingTime();
		gameOverScreen.SetActive(true);

		GameObject GM = GameObject.FindGameObjectWithTag("GM");
		Destroy(GM);
	}

	public int GetOniNum()
	{
		return hiraganaCallNum;
	}	
}