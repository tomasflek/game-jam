using System.Collections.Generic;
using Events;
using Events.Input;
using Helpers;
using UnityEngine.InputSystem;

namespace Inputs
{
	public class InputManager : UnitySingleton<InputManager>
	{
		#region Fields

		private PlayerInputActions _playerInputActions;
		private readonly HashSet<(int, InputAction)> _hackSet = new();

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
			_playerInputActions.Player.Start.Enable();
			_playerInputActions.Player.ListUp.Enable();
			_playerInputActions.Player.ListDown.Enable();
			_playerInputActions.Player.ListAccept.Enable();

			_playerInputActions.Player.Down.performed += OnDown;
			_playerInputActions.Player.Right.performed += OnRight;
			_playerInputActions.Player.Up.performed += OnUp;
			_playerInputActions.Player.Left.performed += OnLeft;
			_playerInputActions.Player.Start.performed += OnStart;
			_playerInputActions.Player.ListUp.performed += OnListUp;
			_playerInputActions.Player.ListDown.performed += OnListDown;
			_playerInputActions.Player.ListAccept.performed += OnListAccept;
		}

		private void OnUp(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			 SendConditionallyEvent(InputAction.Up, obj.control.device);
		}

		private void OnRight(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			SendConditionallyEvent(InputAction.Right, obj.control.device);
		}

		private void OnDown(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			SendConditionallyEvent(InputAction.Down, obj.control.device);
		}

		private void OnLeft(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			SendConditionallyEvent(InputAction.Left, obj.control.device);
		}

		private void OnStart(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			SendConditionallyEvent(InputAction.Start, obj.control.device);
		}

		private void OnListUp(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			SendConditionallyEvent(InputAction.ListUp, obj.control.device);
		}

		private void OnListDown(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			SendConditionallyEvent(InputAction.ListDown, obj.control.device);
		}

		private void OnListAccept(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			SendConditionallyEvent(InputAction.ListAccept, obj.control.device);
		}

		private void SendConditionallyEvent(InputAction action, InputDevice inputDevice)
		{
			if (!_hackSet.Contains((inputDevice.deviceId, action)))
			{
				_hackSet.Add((inputDevice.deviceId, action));
				var controllerType = GetControllerType(inputDevice);
				EventManager.Instance.SendEvent(new InputKeyEvent(action, inputDevice.deviceId, controllerType));				
			}
			else
			{
				_hackSet.Remove((inputDevice.deviceId, action));
			}
		}
		
		private ControllerType GetControllerType(InputDevice device)
		{
			if (device.description.manufacturer.Contains("Sony"))
				return ControllerType.DualShock;
			if (device.displayName.Contains("Xbox"))
				return ControllerType.Xbox;
			if (device.displayName is "Keyboard")
				return ControllerType.Keyboard;

			return ControllerType.Xbox;
		}
		
		#endregion
	}
}