using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Words : MonoBehaviour 
{
	private Dictionary<string, string[]> words = new Dictionary<string, string[]>();
	private string[] word;
	
	void Awake () 
	{
		word = new string[]{"か","ば","ん"};
		words["bag"] = word;
		word = new string[]{"ぎ","ん","こ","う"};
		words["bank"] = word;
		word = new string[]{"パ","ン"};
		words["bread"] = word;
		word = new string[]{"じ","て","ん","し","ゃ"};
		words["bicycle"] = word;
		word = new string[]{"こ","く","ば","ん"};
		words["blackboard"] = word;
		word = new string[]{"ほ","ん"};
		words["book"] = word;
		word = new string[]{"い","す"};
		words["chair"] = word;
		word = new string[]{"と","け","い"};
		words["clock"] = word;
		word = new string[]{"コ","ー","ヒ","ー"};
		words["coffee"] = word;
		word = new string[]{"さ","か","な"};
		words["fish"] = word;
		word = new string[]{"ハ","ン","バ","ー","ガ","ー"};
		words["hamburger"] = word;
		word = new string[]{"ぼ","う","し"};
		words["hat"] = word;
		word = new string[]{"い","え"};
		words["house"] = word;
		word = new string[]{"て","が","み"};
		words["letter"] = word;
		word = new string[]{"に","く"};
		words["meat"] = word;
		word = new string[]{"し","ん","ぶ","ん"};
		words["newspaper"] = word;
		word = new string[]{"ノ","ー","ト"};
		words["notebook"] = word;
		word = new string[]{"ぺ","ん"};
		words["pen"] = word;
		word = new string[]{"え","ん","び","つ"};
		words["pencil"] = word;
		word = new string[]{"ご","は","ん"};
		words["rice"] = word;
		word = new string[]{"サ","ラ","ダ"};
		words["salad"] = word;
		word = new string[]{"つ","く","え"};
		words["table"] = word;
		word = new string[]{"お","ち","ゃ"};
		words["tea"] = word;
		word = new string[]{"で","ん","わ"};
		words["telephone"] = word;
		word = new string[]{"ト","イ","レ"};
		words["toilet"] = word;
		word = new string[]{"テ","レ","ビ"};
		words["tv"] = word;
		word = new string[]{"か","さ"};
		words["umbrella"] = word;
		word = new string[]{"や","さ","い"};
		words["vegtable"] = word;
		word = new string[]{"さ","い","ふ"};
		words["wallet"] = word;
		word = new string[]{"ま","ど"};
		words["window"] = word;
	}

	public IEnumerator getAll()
	{
		return words.GetEnumerator();
	}

	public IEnumerator getKeys()
	{
		ICollection col = words.Keys;
		return col.GetEnumerator();
	}

	public IEnumerator getWords()
	{
		ICollection col = words.Values;
		return col.GetEnumerator();
	}

	public string[] getKeyArray()
	{
		int numWords = this.getCount();
		IEnumerator en = this.getKeys();
		en.MoveNext();
		string[] keyArray = new string[numWords];
		
		for (int i = 0; i < numWords; i++)
		{
			keyArray[i] = (string) en.Current;
			en.MoveNext();
		}

		return keyArray;
	}

	public int getCount()
	{
		return words.Count;
	}

	public string[] get(string key)
	{
		if(words.ContainsKey(key))
		{
			return words[key];
		}
		else
			return null;
	}
}
