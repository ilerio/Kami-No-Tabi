using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameControllerDnDHiragana : MonoBehaviour 
{
	public Transform holder;
	public GameObject[] slots;
	public bool AllFilledIn = false;

	private string[] inputHiragana;
	private string[] correctHiragana; // The corect sequence of hiragana
	private Stack<int> wrongSpots = new Stack<int>();

	void Awake()
	{
		inputHiragana = new string[46];

		for(int i = 0; i < inputHiragana.Length; i++)
			inputHiragana[i] = "?";

		correctHiragana = new string[46];

		correctHiragana[0] = "A";
		correctHiragana[1] = "I";
		correctHiragana[2] = "U";
		correctHiragana[3] = "E";
		correctHiragana[4] = "O";
		correctHiragana[5] = "Ka";
		correctHiragana[6] = "Ki";
		correctHiragana[7] = "Ku";
		correctHiragana[8] = "Ke";
		correctHiragana[9] = "Ko";
		correctHiragana[10] = "Sa";
		correctHiragana[11] = "Shi";
		correctHiragana[12] = "Su";
		correctHiragana[13] = "Se";
		correctHiragana[14] = "So";
		correctHiragana[15] = "Ta";
		correctHiragana[16] = "Chi";
		correctHiragana[17] = "Tsu";
		correctHiragana[18] = "Te";
		correctHiragana[19] = "To";
		correctHiragana[20] = "Na";
		correctHiragana[21] = "Ni";
		correctHiragana[22] = "Nu";
		correctHiragana[23] = "Ne";
		correctHiragana[24] = "No";
		correctHiragana[25] = "Ha";
		correctHiragana[26] = "Hi";
		correctHiragana[27] = "Hu";
		correctHiragana[28] = "He";
		correctHiragana[29] = "Ho";
		correctHiragana[30] = "Ma";
		correctHiragana[31] = "Mi";
		correctHiragana[32] = "Mu";
		correctHiragana[33] = "Me";
		correctHiragana[34] = "Mo";
		correctHiragana[35] = "Ya";
		correctHiragana[36] = "Yu";
		correctHiragana[37] = "Yo";
		correctHiragana[38] = "Ra";
		correctHiragana[39] = "Ri";
		correctHiragana[40] = "Ru";
		correctHiragana[41] = "Re";
		correctHiragana[42] = "Ro";
		correctHiragana[43] = "Wa";
		correctHiragana[44] = "Wo";
		correctHiragana[45] = "N";
	}

	public bool CheckWinState()
	{
		bool pass = true;
		int i = 0;

		string temp = ""; // debug

		while(i < 46)
		{
			if (inputHiragana[i].CompareTo(correctHiragana[i]) != 0)
			{
				pass = false;
				wrongSpots.Push(i);
			}

			temp += "[" + inputHiragana[i] + "]"; // debug

			i++;
		}

		Debug.Log(temp);

		if (pass == true)
			return true;
		else
			return false;
	}

	public void setInputHirgana(int pos, string name)
	{
		inputHiragana[pos] = name;

		Debug.Log("Slot[" + pos + "] registered w/ " + name);

		bool temp = true;
		foreach (string s in inputHiragana)
		{
			if (s == "?"){temp = false;}
		}

		if (temp == true) {AllFilledIn = true;}
	}

	public void unsetInputHiragana(int pos)
	{
		Debug.Log(inputHiragana[pos] + " removed form slot[" + pos + "]");

		inputHiragana[pos] = "?";

		AllFilledIn = false;
	}

	public void WrongAnswer()
	{
		Color colorRed;
		colorRed = Color.red;
		colorRed.a = 0.393f;
		Transform[] wrong = new Transform[46];

		while(wrongSpots.Count > 0)
		{
			int cur = wrongSpots.Pop();
			slots[cur].GetComponent<Image>().color = colorRed;
			slots[cur].GetComponent<DropZone>().objectInThisSpot = null;
			if (slots[cur].transform.childCount > 0)
				unsetInputHiragana(cur);
			if (slots[cur].transform.childCount > 0)
			{
				wrong[cur] = slots[cur].transform.GetChild(0);
			}
		}

		foreach(Transform h in wrong)
		{
			if (h != null)
				h.SetParent(holder);
		}
	}
}