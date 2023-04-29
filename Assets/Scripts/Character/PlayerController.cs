using System;
using System.Collections;
using ActivationDeactivation;
using Events;
using Events.Input;
using Inputs;
using Interface;
using UnityEngine;

namespace Character
{
	[RequireComponent(typeof(Animator))]
	public class PlayerController : MonoBehaviour, IControllerActivable
	{
		[SerializeField]
		private Vector3 _borders;

		private Vector3 _targetIncrementMovement;
		private Vector3 _sourceMovement;
		private Animator _animator;
		private static readonly int IsMovingTrigger = Animator.StringToHash("IsMovingTrigger");
		private float _movementDuration;
		private bool _moving;

		public string Name { get; set; }

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_movementDuration = GetMovementDuration();
			RegisterController();
		}

		private float GetMovementDuration()
		{
			AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
			foreach (AnimationClip clip in clips)
			{
				switch (clip.name)
				{
					case "Jump":
						return clip.length;
				}
			}

			return 0;
		}

		IEnumerator LerpPosition(Vector3 targetPosition, float duration)
		{
			_moving = true;
			var time = 0f;
			var startPosition = transform.position;
			while (time < duration)
			{
				transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
				time += Time.deltaTime;
				yield return null;
			}

			transform.position = targetPosition;
			_moving = false;
		}

		private void OnInputKey(InputKeyEvent inputKeyEvent)
		{
			if (inputKeyEvent.KeyPress is not KeyPress.Pressed || _moving)
				return;

			switch (inputKeyEvent.Action)
			{
				case InputAction.Left:
					_targetIncrementMovement = Vector3.left;
					break;
				case InputAction.Right:
					_targetIncrementMovement = Vector3.right;
					break;
				case InputAction.Up:
					_targetIncrementMovement = Vector3.forward;
					break;
				case InputAction.Down:
					_targetIncrementMovement = Vector3.back;
					break;
			}

			var destinationPosition = _targetIncrementMovement + transform.position;
			if (!CanMove(destinationPosition))
				return;
			
			_animator.SetTrigger(IsMovingTrigger);

			
			StartCoroutine(LerpPosition(destinationPosition, _movementDuration));
		}

		private bool CanMove(Vector3 targetPosition)
		{
			if (Mathf.Abs(targetPosition.z) >= _borders.z)
				return false;
			if (Mathf.Abs(targetPosition.x) >= _borders.x)
				return false;

			return true;
		}

		#region IControllerActivable implementation

		public void RegisterController()
		{
			ActivationManager.Instance.Register(this, true);
		}

		public void UnregisterController()
		{
			ActivationManager.Instance.UnRegister(this);
		}

		public ControllerState ControllerState { get; set; }

		/// <summary>
		/// Enables character controller and registers all events.
		/// </summary>
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

		#endregion IControllerActivable implementation
	}
}