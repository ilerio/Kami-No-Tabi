using UnityEngine;
using System.Collections;

public class Pictures : MonoBehaviour 
{
	public Sprite[] images;
	public SpriteRenderer imageView;

	public Sprite noImageFound;

	public void setImage(string name)
	{
		bool found = false;
		for (int i = 0; i < images.Length; i++)
		{
			if (images[i].name.Equals(name))
			{
				imageView.sprite = images[i];
				found = true;
			}
		}

		if (found == false)
		{
			imageView.sprite = noImageFound;
		}
	}
}
