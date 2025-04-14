using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
	public GameObject gameCanvas;
	public GameObject startCanvas;

	public void ShowStartCanvas()
	{
		gameCanvas.SetActive(false);
		startCanvas.SetActive(true);
	}

	public void ShowGameCanvas()
	{
		Debug.Log("Game Canvas is now active");
		gameCanvas.SetActive(true);
		startCanvas.SetActive(false);
	}
}
