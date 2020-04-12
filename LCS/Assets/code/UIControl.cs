using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControl : MonoBehaviour
{
	public GameObject controlPanel;
	public GameObject loadBtn;
	public GameObject selector;
	void Start()
	{
		ShowSelectorPanel(false);
		EnableControls(false);
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void EnableControls(bool enable)
	{
		controlPanel.GetComponent<CanvasGroup>().interactable = enable;
	}

	public void EnableLoadBtn(bool enable)
	{
		loadBtn.GetComponent<CanvasGroup>().interactable = enable;
	}

	public void ShowSelectorPanel(bool show)
	{
		//Debug.Log("Show? " + show);
		selector.GetComponent<CanvasGroup>().alpha = show ? 1 : 0;
		//Debug.Log("Alpha " + selector.GetComponent<CanvasGroup>().alpha);
		selector.GetComponent<CanvasGroup>().blocksRaycasts = show;
		selector.GetComponent<CanvasGroup>().interactable = show;
		if (show)
		{
			selector.GetComponentInChildren<LoadDirectories>().Reload();
		}
	}
}
