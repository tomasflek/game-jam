using System.Collections;
using System.Collections.Generic;
using Events;
using Events.Input;
using Inputs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private GameObject TutorialPanel;

	private void Awake()
	{
		EventManager.Instance.Register<InputKeyEvent>(ToggleTutorialPanel);
	}

	private void OnDestroy()
	{
		EventManager.Instance.Unregister<InputKeyEvent>(ToggleTutorialPanel);
	}

	private void Start()
	{
		AudioManager.Instance.PlayMusicSound("PostmanBattle", false);
		AudioManager.Instance.MusicEnd += PlayNextMainTheme;
	}

	private void PlayNextMainTheme()
	{
		AudioManager.Instance.MusicEnd -= PlayNextMainTheme;
		AudioManager.Instance.PlayMusicSound("MenuMusic", true);
	}

	public void SwitchToScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void ToggleTutorialPanel()
	{
		TutorialPanel.SetActive(!TutorialPanel.activeSelf);
	}

	public void ToggleTutorialPanel(InputKeyEvent inputKeyEvent)
	{
		if (TutorialPanel.activeSelf)
		{
			ToggleTutorialPanel();
		}
	}
}
