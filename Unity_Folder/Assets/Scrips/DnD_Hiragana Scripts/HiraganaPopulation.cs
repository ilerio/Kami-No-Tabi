using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HiraganaPopulation : MonoBehaviour 
{
	public Sprite[] hiragana;
	public GameObject hiraganaItem;
	public Transform holder;

	private Stack<int> hiraganaNumStack = new Stack<int>();

	void Awake()
	{
		int[] check = new int[46];
		int randomNumber;
		int loopCount = 0;
		
		while(hiraganaNumStack.Count < 46) // should always be 46
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

		while (hiraganaNumStack.Count > 0)
		{
			int current = hiraganaNumStack.Pop();
			//Debug.Log (hiraganaNumStack.Count + ": " + current);
			GameObject newHiragana = Instantiate(hiraganaItem);
			newHiragana.transform.SetParent(holder);
			newHiragana.GetComponent<Image>().sprite = hiragana[current];
			newHiragana.name = hiragana[current].name;

			Vector3 scale;
			scale.x = 1f;
			scale.y = 1f;
			scale.z = 1f;
			newHiragana.transform.localScale = scale;
		}
	}
}
