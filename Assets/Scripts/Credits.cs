using Events.Input;
using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
	private void Awake()
	{
		EventManager.Instance.Register<InputKeyEvent>(BackToMenu);
	}

	private void OnDestroy()
	{
		EventManager.Instance.Unregister<InputKeyEvent>(BackToMenu);
	}

	public void BackToMenu()
	{
		SceneManager.LoadScene(0);
	}

	public void BackToMenu(InputKeyEvent inputKeyEvent)
	{
		BackToMenu();
	}
}
