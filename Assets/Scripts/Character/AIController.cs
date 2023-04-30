using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public class AIController : MonoBehaviour
	{
		[SerializeField] private Vector3 _borders;

		private Vector3 _movementVector;
		private Vector3 _sourceMovement;
		private Animator _animator;
		private static readonly int IsMovingTrigger = Animator.StringToHash("IsMovingTrigger");
		private float _movementDuration;
		private bool _moving;

		[SerializeField]
		private float AiMoveMin = 0.3f;
		[SerializeField]
		private float AiMoveMAx = 0.8f;
		private float aiMoveRemain;


		private Dictionary<InputAction, Vector3> _movementVectorDict = new()
		{
			{ InputAction.Left, Vector3.left },
			{ InputAction.Right, Vector3.right },
			{ InputAction.Up, Vector3.forward },
			{ InputAction.Down, Vector3.back },
		};

		private MovementImageIconsController _iconChnager;

		private bool _firstMovenemt = true;
		public string Name { get; set; }

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_movementDuration = GetMovementDuration();
		}

		private void Start()
		{
			SetAiMoveRemain();
		}

		private void SetAiMoveRemain()
		{
			aiMoveRemain = Random.Range(AiMoveMin, AiMoveMAx);
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

		private void Update()
		{
			if (_moving || GameManager.Instance.Paused)
				return;

			aiMoveRemain -= Time.deltaTime;
			if (aiMoveRemain > 0) return;
			SetAiMoveRemain();

			int index = Random.Range(0, _movementVectorDict.Count);
			var action = _movementVectorDict.Keys.ToList()[index];
			_movementVector = _movementVectorDict[action];

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
	}
}