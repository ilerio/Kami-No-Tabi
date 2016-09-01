﻿using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	public Camera mainCam;
	private float shakeAmount = 0;
	
	void Start ()
	{
		if (mainCam == null)
			mainCam = Camera.main;
	}
	
	public void Shake(float amount, float length)
	{
		shakeAmount = amount;
		InvokeRepeating("DoShake", 0, 0.01f);
		Invoke ("StopShake", length);
	}
	
	void DoShake()
	{
		if (shakeAmount > 0)
		{
			Vector3 camPos = mainCam.transform.position;
			float OffsetX = Random.value * shakeAmount * 2 - shakeAmount;
			float OffsetY = Random.value * shakeAmount * 2 - shakeAmount;
			camPos.x += OffsetX;
			camPos.y += OffsetY;
			mainCam.transform.position = camPos;
		}
	}
	
	void StopShake()
	{
		CancelInvoke ("DoShake");
		mainCam.transform.localPosition = Vector3.zero;
	}
}
