using System;
using System.Collections;
using System.Linq;
using ActivationDeactivation;
using Events;
using Events.Input;
using GameManagers;
using Inputs;
using Interface;
using UnityEngine;

namespace Character
{
	[RequireComponent(typeof(Animator))]
	public class PlayerController : MonoBehaviour, IControllerActivable
	{
		[SerializeField] private Vector3 _borders;

		[SerializeField] private GameObject _home;

		private Vector3 _movementVector;
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
			if (inputKeyEvent.KeyPress is not KeyPress.Pressed || _moving || inputKeyEvent.ControllerIndex != PlayerIndex)
				return;

			switch (inputKeyEvent.Action)
			{
				case InputAction.Left:
					_movementVector = Vector3.left;
					break;
				case InputAction.Right:
					_movementVector = Vector3.right;
					break;
				case InputAction.Up:
					_movementVector = Vector3.forward;
					break;
				case InputAction.Down:
					_movementVector = Vector3.back;
					break;
			}

			var destinationPosition = _movementVector + transform.position;
			if (!CanMove(destinationPosition))
				return;

			_animator.SetTrigger(IsMovingTrigger);
			StartCoroutine(LerpPosition(destinationPosition, _movementDuration));
		}

		private bool CanMove(Vector3 targetPosition)
		{
			// Check boundaries movement.
			if (Mathf.Abs(targetPosition.z) >= _borders.z)
				return false;
			if (Mathf.Abs(targetPosition.x) >= _borders.x)
				return false;

			// Check whether it's possible to move to home (cannot move only to my home)
			// var home = LayerMask.GetMask("Home");
			Collider[] hitColliders = Physics.OverlapSphere(targetPosition, 0.5f);
			foreach (var hitCollider in hitColliders.Where(p => p.CompareTag("Home")))
			{
				var playerId = gameObject.GetInstanceID();
				var homeId = hitCollider.gameObject.GetInstanceID();
				if (GameManager.Instance.PlayerGameObjectIdHomeGameObjectId.TryGetValue(playerId, out int homeGameObjectId))
				{
					return homeGameObjectId == homeId;
				}
			}
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
		public int PlayerIndex { get; set; }

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