using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Characters : MonoBehaviour 
{
	private Dictionary<string, string> hiragana = new Dictionary<string, string>();
	private Dictionary<string, string> katakana = new Dictionary<string, string>();

	void Awake () 
	{
		// Hiragana Dictionary
		hiragana["A"] = "あ";
		hiragana["I"] = "い";
		hiragana["U"] = "う";
		hiragana["E"] = "え";
		hiragana["O"] = "お";
		hiragana["KA"] = "か";
		hiragana["GA"] = "が";
		hiragana["KI"] = "き";
		hiragana["GI"] = "ぎ";
		hiragana["KU"] = "く";
		hiragana["GU"] = "ぐ";
		hiragana["KE"] = "け";
		hiragana["GE"] = "げ";
		hiragana["KO"] = "こ";
		hiragana["GO"] = "ご";
		hiragana["SA"] = "さ";
		hiragana["ZA"] = "ざ";
		hiragana["SI"] = "し";
		hiragana["ZI"] = "じ";
		hiragana["SU"] = "す";
		hiragana["ZU"] = "ず";
		hiragana["SE"] = "せ";
		hiragana["ZE"] = "ぜ";
		hiragana["SO"] = "そ";
		hiragana["ZO"] = "ぞ";
		hiragana["TA"] = "た";
		hiragana["DA"] = "だ";
		hiragana["TI"] = "ち";
		hiragana["DI"] = "ぢ";
		hiragana["tu"] = "っ";
		hiragana["TU"] = "つ";
		hiragana["DU"] = "づ";
		hiragana["TE"] = "て";
		hiragana["DE"] = "で";
		hiragana["TO"] = "と";
		hiragana["DO"] = "ど";
		hiragana["NA"] = "な";
		hiragana["NI"] = "に";
		hiragana["NU"] = "ぬ";
		hiragana["NE"] = "ね";
		hiragana["NO"] = "の";
		hiragana["HA"] = "は";
		hiragana["BA"] = "ば";
		hiragana["PA"] = "ぱ";
		hiragana["HI"] = "ひ";
		hiragana["BI"] = "び";
		hiragana["PI"] = "ぴ";
		hiragana["HU"] = "ふ";
		hiragana["BU"] = "ぶ";
		hiragana["PU"] = "ぷ";
		hiragana["HE"] = "へ";
		hiragana["BE"] = "べ";
		hiragana["PE"] = "ぺ";
		hiragana["HO"] = "ほ";
		hiragana["BO"] = "ぼ";
		hiragana["PO"] = "ぽ";
		hiragana["MA"] = "ま";
		hiragana["MI"] = "み";
		hiragana["MU"] = "む";
		hiragana["ME"] = "め";
		hiragana["MO"] = "も";
		hiragana["ya"] = "ゃ";
		hiragana["YA"] = "や";
		hiragana["yu"] = "ゅ";
		hiragana["YU"] = "ゆ";
		hiragana["yo"] = "ょ";
		hiragana["YO"] = "よ";
		hiragana["RA"] = "ら";
		hiragana["RI"] = "り";
		hiragana["RU"] = "る";
		hiragana["RE"] = "れ";
		hiragana["RO"] = "ろ";
		hiragana["WA"] = "わ";
		hiragana["WO"] = "を";
		hiragana["N"] = "ん";

		// Katakana Dictionary
		katakana["a"] = "ァ";
		katakana["A"] = "ア";
		katakana["i"] = "ィ";
		katakana["I"] = "イ";
		katakana["u"] = "ゥ";
		katakana["U"] = "ウ";
		katakana["e"] = "ェ";
		katakana["E"] = "エ";
		katakana["o"] = "ォ";
		katakana["O"] = "オ";
		katakana["KA"] = "カ";
		katakana["GA"] = "ガ";
		katakana["KI"] = "キ";
		katakana["GI"] = "ギ";
		katakana["KU"] = "ク";
		katakana["GU"] = "グ";
		katakana["KE"] = "ケ";
		katakana["GE"] = "ゲ";
		katakana["KO"] = "コ";
		katakana["GO"] = "ゴ";
		katakana["SA"] = "サ";
		katakana["ZA"] = "ザ";
		katakana["SI"] = "シ";
		katakana["ZI"] = "ジ";
		katakana["SU"] = "ス";
		katakana["ZU"] = "ズ";
		katakana["SE"] = "セ";
		katakana["ZE"] = "ゼ";
		katakana["SO"] = "ソ";
		katakana["ZO"] = "ゾ";
		katakana["TA"] = "タ";
		katakana["DA"] = "ダ";
		katakana["TI"] = "チ";
		katakana["DI"] = "ヂ";
		katakana["tu"] = "ッ";
		katakana["TU"] = "ツ";
		katakana["DU"] = "ヅ";
		katakana["TE"] = "テ";
		katakana["DE"] = "デ";
		katakana["TO"] = "ト";
		katakana["DO"] = "ド";
		katakana["NA"] = "ナ";
		katakana["NI"] = "ニ";
		katakana["NU"] = "ヌ";
		katakana["NE"] = "ネ";
		katakana["NO"] = "ノ";
		katakana["HA"] = "ハ";
		katakana["BA"] = "バ";
		katakana["PA"] = "パ";
		katakana["HI"] = "ヒ";
		katakana["BI"] = "ビ";
		katakana["PI"] = "ピ";
		katakana["HU"] = "フ";
		katakana["BU"] = "ブ";
		katakana["PU"] = "プ";
		katakana["HE"] = "ヘ";
		katakana["BE"] = "ベ";
		katakana["PE"] = "ペ";
		katakana["HO"] = "ホ";
		katakana["BO"] = "ボ";
		katakana["PO"] = "ポ";
		katakana["MA"] = "マ";
		katakana["MI"] = "ミ";
		katakana["MU"] = "ム";
		katakana["ME"] = "メ";
		katakana["MO"] = "モ";
		katakana["ya"] = "ャ";
		katakana["YA"] = "ヤ";
		katakana["yu"] = "ュ";
		katakana["YU"] = "ユ";
		katakana["yo"] = "ョ";
		katakana["YO"] = "ヨ";
		katakana["RA"] = "ラ";
		katakana["RI"] = "リ";
		katakana["RU"] = "ル";
		katakana["RE"] = "レ";
		katakana["RO"] = "ロ";
		katakana["WA"] = "ワ";
		katakana["WO"] = "ヲ";
		katakana["N"] = "ン";
		katakana["DASH"] = "ー";
	}
	
	public IEnumerator getHiraganaEnum()
	{
		ICollection col = hiragana.Values;
		return col.GetEnumerator();
	}

	public IEnumerator getKatakanaEnum()
	{
		ICollection col = katakana.Values;
		return col.GetEnumerator();
	}

	public string getHiragana(string key)
	{
		if (hiragana.ContainsKey(key))
			return hiragana[key];
		else
			return null;
	}

	public string getKatakana(string key)
	{
		if (katakana.ContainsKey(key))
			return katakana[key];
		else
			return null;
	}
}
