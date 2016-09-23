using UnityEngine;
using System.Collections;

public class ObjectFloating : MonoBehaviour 
{
	private bool floatUp;
	private bool floatRight;

	void Awake()
	{
		if (Random.value > 0.5f)
			floatUp = true;
		else
			floatUp = false;
		
		if (Random.value > 0.5f)
			floatRight = true;
		else
			floatRight = false;

		if(floatUp)
			Invoke("FloatingUp", 0.1f);
		else
			Invoke("FloatingDown", 0.1f);
		
		if (floatRight)
			Invoke("FloatingRight", 0.1f);
		else
			Invoke("FloatingLeft", 0.1f);
	}

	// Update is called once per frame
	void Update () 
	{
		if(floatUp)
			Invoke("FloatingUp", 1.2f);
		else
			Invoke("FloatingDown", 1.2f);

		if (floatRight)
			Invoke("FloatingRight", 0.8f);
		else
			Invoke("FloatingLeft", 0.8f);
	}

	void FloatingUp()
	{
		Vector3 pos = this.transform.position; 
		float OffsetY = (float)(0.4*Time.deltaTime);
		pos.y += OffsetY;
		this.transform.position = pos;

		floatUp = false;
	}

	void FloatingDown()
	{
		Vector3 pos = this.transform.position; 
		float OffsetY = (float)(0.4*Time.deltaTime);
		pos.y -= OffsetY;
		this.transform.position = pos;

		floatUp = true;
	}

	void FloatingRight()
	{
		Vector3 pos = this.transform.position; 
		float OffsetX = (float)(0.4*Time.deltaTime);
		pos.x += OffsetX;
		this.transform.position = pos;
		
		floatRight = false;
	}

	void FloatingLeft()
	{
		Vector3 pos = this.transform.position; 
		float OffsetX = (float)(0.4*Time.deltaTime);
		pos.x -= OffsetX;
		this.transform.position = pos;
		
		floatRight = true;
	}
}
