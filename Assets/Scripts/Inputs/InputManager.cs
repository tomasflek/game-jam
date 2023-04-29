using Events;
using Events.Input;
using Helpers;
using UnityEngine;

namespace Inputs
{
	public class InputManager : UnitySingleton<InputManager>
	{
		#region Fields

		private PlayerInputActions _playerInputActions;

		#endregion

		#region Properties

		// private Vector2 Movement => _playerInputActions.Player.Move.ReadValue<Vector2>();
		//
		// private Direction Direction => Mathf.Abs(Movement.x) > Constants.Math.FloatEps ?
		// 	(Direction)Mathf.Sign(Movement.x) :
		// 	Direction.NoDirection;

		#endregion

		#region Methods

		protected override void Awake()
		{
			base.Awake();

			_playerInputActions = new PlayerInputActions();
			
			_playerInputActions.Player.Left.Enable();
			_playerInputActions.Player.Up.Enable();
			_playerInputActions.Player.Right.Enable();
			_playerInputActions.Player.Down.Enable();

			_playerInputActions.Player.Down.performed += OnDown;
			_playerInputActions.Player.Right.performed += OnRight;
			_playerInputActions.Player.Up.performed += OnUp;
			_playerInputActions.Player.Left.performed += OnLeft;
			
			_playerInputActions.Player.Down.canceled += OnDownReleased;
			_playerInputActions.Player.Right.canceled += OnRightReleased;
			_playerInputActions.Player.Up.canceled += OnUpReleased;
			_playerInputActions.Player.Left.canceled += OnLeftReleased;

			// _playerInputActions.Player.Jump.performed += OnJumpPerformed;
			// _playerInputActions.Player.Jump.canceled += OnJumpCanceled;
			//
			// _playerInputActions.Player.Dash.performed += OnDashPerformed;
		}

		private void OnUp(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Left, KeyPress.Pressed));
		}

		private void OnRight(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Left, KeyPress.Pressed));
		}

		private void OnDown(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Left, KeyPress.Pressed));
		}

		private void OnLeft(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Left, KeyPress.Pressed));
		}
		
		private void OnUpReleased(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Left, KeyPress.Pressed));
		}

		private void OnRightReleased(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Left, KeyPress.Pressed));
		}

		private void OnDownReleased(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Left, KeyPress.Pressed));
		}

		private void OnLeftReleased(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Left, KeyPress.Pressed));
		}
		
		#endregion
	}
}