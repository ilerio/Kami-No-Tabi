using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController_L03 : MonoBehaviour 
{

	public GameObject item;
	public GameObject itemView;
	public GameObject slot;
	public Transform slotHolder;
	public Transform heartHolder;

	public GameObject UI_Interface;
	public GameObject UI_DnD;
	public GameObject Pause_Menue;
	public GameObject DieScreen;
	public GameObject GOScreen;

	public GameObject kappa;
	public Text wordCounter;

	private int countCurWord;

	private Words words;
	private Characters ch;
	private Pictures pics;
	private RedFlash flash;
	private CameraShake shake;
	private Stack<string> answerStack = new Stack<string>();

	private string currentKey;
	private string[] currentWord;
	private int curWordLength;
	private GameObject[] currentSlots  = new GameObject[0];

	private int lives = 5;

	void Start()
	{
		flash = this.GetComponent<RedFlash>();
		shake = this.GetComponent<CameraShake>();
		pics = this.GetComponent<Pictures>();
		words = this.GetComponent<Words>();
		int numWords = words.getCount();
		int[] check = new int[numWords];
		check.Initialize();
		int rand;
		countCurWord = 0;

		string[] keyArray = words.getKeyArray();

		int loopCount = 0; // debug
		string temp = ""; // debug
		while(answerStack.Count < numWords)
		{
			rand = Random.Range(0, numWords);
			bool flag = true;
			
			if(check[rand] != 0)
				flag = false;
			
			if(flag == true)
			{
				answerStack.Push(keyArray[rand]);
				check[rand] = -1;

				temp += "[" + keyArray[rand] + "]"; // debug
			}
			/*else
				Debug.Log ("DENIED!"); //debugging purposes*/
			
			loopCount ++;
		}
		
		Debug.Log ("pre-selection of words done! " + loopCount + " loop throughs.\n" + temp);

		wordCounter.text = countCurWord + " / 38";
		Init();
	}

	void Init()
	{
		Debug.Log(answerStack.Count);

		currentKey = answerStack.Pop();
		currentWord = words.get(currentKey);

		// Set Image
		pics.setImage(currentKey);

		if (currentWord == null)
		{
			Debug.Log(currentKey.ToUpper() + "was not found in the dictionary.");
		}
		else
			curWordLength = currentWord.Length;

		// Clear previous slots
		if (currentSlots != null)
		{
			foreach(GameObject go in currentSlots)
				Destroy(go);
		}

		string temp = ""; // debug

		// Initalize number of slots curesponding to number of characters in the word
		currentSlots = new GameObject[curWordLength];
		for (int i = 0; i < curWordLength; i++)
		{
			GameObject slot = Instantiate(this.slot);
			slot.name = currentWord[i];
			slot.transform.SetParent(slotHolder);
			currentSlots[i] = slot;

			temp += "[" + currentWord[i] + "]"; // debug
		}

		Debug.Log("Current key: ["+ currentKey +"] = word: " + temp);
	}

	public void CheckAnswer()
	{
		bool flag = true; 

		for(int i = 0; i < currentSlots.Length; i++)
		{
			string answer = currentSlots[i].name;
			Transform cst = currentSlots[i].transform; // Current Slot Transform
			string currentChar;

			if (cst.childCount > 0)
				currentChar = cst.GetChild(0).name;
			else
				currentChar = "No character in slot [" + answer + "].";

			Debug.Log("[answer]: " + answer + " == [currentChar]: " + currentChar + " = " + currentChar.Equals(answer));

			if (currentChar.Equals(answer) == false)
			{
				flag = false;
			}
		}

		if (flag == true) {
			countCurWord++;
			CorrectAnswer();
			wordCounter.text = countCurWord + " / 10";
		} else
			WrongAnswer();
	}

	void CorrectAnswer()
	{
		if (answerStack.Count <= 0 || countCurWord > 10)
			win();
		else
			Init();
	}

	void WrongAnswer()
	{
		Debug.Log("Wrong!");
		shake.Shake(0.08f, 0.2f);
		flash.Flash();

		Vector3 pos = kappa.transform.localPosition;
		pos.y -= 1.15f;
		kappa.transform.localPosition = pos;

		Vector3 scale = kappa.transform.localScale;
		scale.x += 0.16f;
		scale.y += 0.16f;
		kappa.transform.localScale = scale;

		lives -= 1;
		Debug.Log("lives = " + lives);
		
		heartHolder.GetChild(lives).gameObject.SetActive(false);

		if (lives <= 0) // Die
		{
			UI_Interface.SetActive(false);
			UI_DnD.SetActive(false);
			Pause_Menue.SetActive(false);
			itemView.SetActive(false);
			DieScreen.SetActive(true);
		}
	}

	void win()
	{
		Debug.Log("Win");

		PlayerPrefs.SetInt("Level_03", 1);

		UI_Interface.SetActive(false);
		UI_DnD.SetActive(false);
		Pause_Menue.SetActive(false);
		GOScreen.SetActive(true);
	}

	public string getCurrentKey()
	{
		return currentKey;
	}

	public void clearSlots()
	{
		for (int i = 0; i < currentSlots.Length;i++)
		{
			if (currentSlots[i].transform.childCount > 0)
				Destroy(currentSlots[i].transform.GetChild(0).gameObject); // This assumes there is always only one child
		}
	}
}