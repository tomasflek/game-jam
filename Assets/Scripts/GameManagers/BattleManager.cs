using Events.Input;
using Events;
using System.Collections;
using System.Collections.Generic;
using Inputs;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
	private bool _isBattling;
	[SerializeField] private Transform _playerOneFightPosition;
	[SerializeField] private Transform _playerTwoFightPosition;
	public Canvas BattleCanvas;
	private Vector3 _playerOneStartPosition;
	private Vector3 _playerTwoStartPosition;
	private GameObject _playerOne;
	private GameObject _playerTwo;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

	public void StartBattle(GameObject playerOne, GameObject playerTwo)
	{
		_playerOne = playerOne;
		_playerTwo = playerTwo;
		_playerOneStartPosition = playerOne.transform.position;
		_playerTwoStartPosition = playerTwo.transform.position;
		_playerOne.transform.position = _playerOneFightPosition.position;
		_playerTwo.transform.position = _playerTwoFightPosition.position;
		BattleCanvas.gameObject.SetActive(true);
	}

	public void EndBattle()
	{
		_playerOne.transform.position = _playerOneStartPosition;
		_playerTwo.transform.position = _playerTwoStartPosition;
		BattleCanvas.gameObject.SetActive(false);
	}

	private void OnInputKey(InputKeyEvent inputKeyEvent)
	{
		//if (inputKeyEvent.KeyPress is not KeyPress.Pressed || _moving)
		//	return;

		//switch (inputKeyEvent.Action)
		//{
		//	case InputAction.Left:
		//		_targetIncrementMovement = Vector3.left;
		//		break;
		//	case InputAction.Right:
		//		_targetIncrementMovement = Vector3.right;
		//		break;
		//	case InputAction.Up:
		//		_targetIncrementMovement = Vector3.forward;
		//		break;
		//	case InputAction.Down:
		//		_targetIncrementMovement = Vector3.back;
		//		break;
		//}
	}

	public void ActivateController(object activator)
	{
		EventManager.Instance.Register<InputKeyEvent>(OnInputKey);
	}

	/// <summary>
	/// Disables character controller and unregisters all events.
	/// </summary>
	public void DeactivateController(object deactivator)
	{
		EventManager.Instance.Unregister<InputKeyEvent>(OnInputKey);
	}
}
