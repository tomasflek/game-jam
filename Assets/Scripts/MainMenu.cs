using Events;
using Inputs;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private GameObject TutorialPanel;
	[SerializeField] private Transform ButtonParentObject;
	private ButtonControl[] _buttons;
	private int _activeButton = -1;

	private void Awake()
	{
		EventManager.Instance.Register<InputKeyEventUI>(ToggleTutorialPanel);
		EventManager.Instance.Register<InputKeyEventUI>(MenuControl);
	}

	private void OnDestroy()
	{
		EventManager.Instance.Unregister<InputKeyEventUI>(ToggleTutorialPanel);
		EventManager.Instance.Unregister<InputKeyEventUI>(MenuControl);
	}

	private void Start()
	{
		_buttons = ButtonParentObject.GetComponentsInChildren<ButtonControl>();
		AudioManager.Instance.PlayMusicSound("PostmanBattle", false);
		AudioManager.Instance.MusicEnd += PlayNextMainTheme;
		//Activate first button
		MenuControl(new InputKeyEventUI(InputAction.ListDown, -1, ControllerType.Keyboard));
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

	public void ToggleTutorialPanel(InputKeyEventUI inputKeyEvent)
	{
		if (TutorialPanel.activeSelf)
		{
			ToggleTutorialPanel();
		}
	}

	public void ExitGame()
	{
		#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
}

	private void MenuControl(InputKeyEventUI inputKeyEvent)
	{
		if (inputKeyEvent != null )
		{
			if (inputKeyEvent.Action == InputAction.ListUp)
			{
				_buttons[_activeButton].DehoverButton();
				_activeButton--;
				if(_activeButton < 0)
					_activeButton = _buttons.Length	- 1;
				_buttons[_activeButton].HoverButton();
			}
			else if (inputKeyEvent.Action == InputAction.ListDown)
			{
				if (_activeButton != -1)
					_buttons[_activeButton].DehoverButton();
				_activeButton++;
				if (_activeButton >= _buttons.Length)
					_activeButton = 0;
				_buttons[_activeButton].HoverButton();
			}
			else if(inputKeyEvent.Action == InputAction.ListAccept)
			{
				_buttons[_activeButton].ClickButton();
			}
		}
	}

	public void ActivateButton(int index)
	{
		_buttons[_activeButton].DehoverButton();
		_activeButton = index;
		_buttons[_activeButton].HoverButton();
	}
}
