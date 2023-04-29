using Events;
using Events.Input;
using Helpers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
	public class InputManager : UnitySingleton<InputManager>
	{
		#region Fields

		private PlayerInputActions _playerInputActions;

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

			_playerInputActions.Player.Down.performed += OnDown;
			_playerInputActions.Player.Right.performed += OnRight;
			_playerInputActions.Player.Up.performed += OnUp;
			_playerInputActions.Player.Left.performed += OnLeft;
			_playerInputActions.Player.Start.performed += OnStart;
			
			// _playerInputActions.Player.Down.canceled += OnDownReleased;
			// _playerInputActions.Player.Right.canceled += OnRightReleased;
			// _playerInputActions.Player.Up.canceled += OnUpReleased;
			// _playerInputActions.Player.Left.canceled += OnLeftReleased;
		}

		private void OnUp(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			var controllerType = GetControllerType(obj.control.device);
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Up, KeyPress.Pressed, obj.control.device.deviceId, controllerType));
		}

		private void OnRight(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			var controllerType = GetControllerType(obj.control.device);
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Right, KeyPress.Pressed, obj.control.device.deviceId, controllerType));
		}

		private void OnDown(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			var controllerType = GetControllerType(obj.control.device);
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Down, KeyPress.Pressed, obj.control.device.deviceId, controllerType));
		}

		private void OnLeft(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			var controllerType = GetControllerType(obj.control.device);
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Left, KeyPress.Pressed, obj.control.device.deviceId, controllerType));
		}
		
		private void OnStart(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			var controllerType = GetControllerType(obj.control.device);
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Start, KeyPress.Pressed, obj.control.device.deviceId, controllerType));
		}
		
		private void OnUpReleased(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			var controllerType = GetControllerType(obj.control.device);
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Up, KeyPress.Released, obj.control.device.deviceId, controllerType));
		}

		private void OnRightReleased(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			var controllerType = GetControllerType(obj.control.device);
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Right, KeyPress.Released, obj.control.device.deviceId, controllerType));
		}

		private void OnDownReleased(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			var controllerType = GetControllerType(obj.control.device);
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Down, KeyPress.Released, obj.control.device.deviceId,controllerType));
		}

		private void OnLeftReleased(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			var controllerType = GetControllerType(obj.control.device);
			EventManager.Instance.SendEvent(new InputKeyEvent(InputAction.Left, KeyPress.Released, obj.control.device.deviceId, controllerType));
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