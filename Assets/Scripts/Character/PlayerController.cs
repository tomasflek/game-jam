using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Events.Input;
using GameManagers;
using Inputs;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Character
{
	[RequireComponent(typeof(Animator))]
	public class PlayerController : MonoBehaviour, IPrefab
	{
		[SerializeField] private Vector3 _borders;

		private Vector3 _movementVector;
		private Vector3 _sourceMovement;
		private Animator _animator;
		private static readonly int IsMovingTrigger = Animator.StringToHash("IsMovingTrigger");
		private float _movementDuration;
		private bool _moving;

		private Dictionary<InputAction, Vector3> _movementVectorDict = new()
		{
			{ InputAction.Left, Vector3.left },
			{ InputAction.Right, Vector3.right },
			{ InputAction.Up, Vector3.forward },
			{ InputAction.Down, Vector3.back },
		};


		public int PrefabInt { get; set; }

		private MovementImageIconsController _iconChnager;

		private bool _firstMovenemt = true;
		public string Name { get; set; }

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_movementDuration = GetMovementDuration();
			EventManager.Instance.Register<InputKeyEvent>(OnInputKey);
			_iconChnager = GetComponent<MovementImageIconsController>();
		}

		private void Start()
		{
			GenerateMovementVectors(false);
		}

		private void OnDestroy()
		{
			EventManager.Instance.Unregister<InputKeyEvent>(OnInputKey);
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
			if (GameManager.Instance.Paused)
				yield return null;

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
			_movementVector = Vector3.zero;
			GenerateMovementVectors(!_firstMovenemt);
		}

		public void GenerateMovementVectors(bool random)
		{
			random = false;
			var controllerType = GameManager.Instance.PlayerIndexType[PlayerIndex];
			var movementVectors = new List<Vector3>()
			{
				Vector3.left, Vector3.right, Vector3.forward, Vector3.back
			};

			Dictionary<InputAction, Image> super = new()
			{
				{ InputAction.Left, _iconChnager.ImageLeft },
				{ InputAction.Right, _iconChnager.ImageRight },
				{ InputAction.Up, _iconChnager.ImageUp },
				{ InputAction.Down, _iconChnager.ImageDown },
			};

			// LEFT
			var index = random ? Random.Range(0, movementVectors.Count) : 0;
			_movementVectorDict[InputAction.Left] = movementVectors[index];
			var newInputAction = GetInputActionFromMovementVector(movementVectors[index]);
			var sprite = UIManager.Instance.GetSprite(InputAction.Left, controllerType);
			super[newInputAction].sprite = sprite;
			movementVectors.RemoveAt(index);

			// RIGHT
			index = random ? Random.Range(0, movementVectors.Count) : 0;
			_movementVectorDict[InputAction.Right] = movementVectors[index];
			newInputAction = GetInputActionFromMovementVector(movementVectors[index]);
			sprite = UIManager.Instance.GetSprite(InputAction.Right, controllerType);
			super[newInputAction].sprite = sprite;
			movementVectors.RemoveAt(index);

			// UP
			index = random ? Random.Range(0, movementVectors.Count) : 0;
			_movementVectorDict[InputAction.Up] = movementVectors[index];
			newInputAction = GetInputActionFromMovementVector(movementVectors[index]);
			sprite = UIManager.Instance.GetSprite(InputAction.Up, controllerType);
			super[newInputAction].sprite = sprite;
			movementVectors.RemoveAt(index);

			// DOWN
			index = random ? Random.Range(0, movementVectors.Count) : 0;
			_movementVectorDict[InputAction.Down] = movementVectors[index];
			newInputAction = GetInputActionFromMovementVector(movementVectors[index]);
			sprite = UIManager.Instance.GetSprite(InputAction.Down, controllerType);
			super[newInputAction].sprite = sprite;
			movementVectors.RemoveAt(index);
		}

		private InputAction GetInputActionFromMovementVector(Vector3 vector)
		{
			if (vector == Vector3.left)
				return InputAction.Left;
			if (vector == Vector3.right)
				return InputAction.Right;
			if (vector == Vector3.forward)
				return InputAction.Up;
			if (vector == Vector3.back)
				return InputAction.Down;
			throw new Exception();
		}

		private void OnInputKey(InputKeyEvent inputKeyEvent)
		{
			if (_moving || inputKeyEvent.ControllerIndex != PlayerIndex || GameManager.Instance.Paused)
				return;

			if (inputKeyEvent.Action is InputAction.Start)
				return;

			_firstMovenemt = false;
			_movementVector = _movementVectorDict[inputKeyEvent.Action];

			var destinationPosition = _movementVector + transform.position;
			if (!CanMove(destinationPosition))
				return;

			_animator.SetTrigger(IsMovingTrigger);
			StartCoroutine(LerpPosition(destinationPosition, _movementDuration));
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.CompareTag("Player"))
				return;

			if (GameManager.Instance.PlayerWithPickup == gameObject)
				BattleManager.Instance.StartBattle(gameObject, other.gameObject);
		}

		private bool CanMove(Vector3 targetPosition)
		{
			// Check boundaries movement.
			if (Mathf.Abs(targetPosition.z) >= _borders.z)
				return false;
			if (Mathf.Abs(targetPosition.x) >= _borders.x)
				return false;

			return true;
		}

		public int PlayerIndex { get; set; }
	}
}