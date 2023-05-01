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
	public class AIController : MonoBehaviour, IPrefab
	{
		[SerializeField] private Vector3 _borders;

		private Vector3 _movementVector;
		private Vector3 _sourceMovement;
		private Animator _animator;
		private static readonly int IsMovingTrigger = Animator.StringToHash("IsMovingTrigger");
		private float _movementDuration;
		private bool _moving;

		public float randomMoveChance = 30f;
		[SerializeField]
		private float AiMoveMin = 0.3f;
		[SerializeField]
		private float AiMoveMAx = 0.8f;
		private float aiMoveRemain;

		[SerializeField]
		private bool randomBehaviour = false;

		public AIBehaviour Behavior = AIBehaviour.Follow;


		private Dictionary<InputAction, Vector3> _movementVectorDict = new()
		{
			{ InputAction.Left, Vector3.left },
			{ InputAction.Right, Vector3.right },
			{ InputAction.Up, Vector3.forward },
			{ InputAction.Down, Vector3.back },
		};


		public int PrefabInt { get; set; }
		public int PlayerIndex { get; set; }
		public bool PickedUp { get; set; }

		private MovementImageIconsController _iconChnager;
		public string Name { get; set; }

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_movementDuration = GetMovementDuration();
		}

		private void Start()
		{
			SetAiMoveRemain();
			SetBehavior();
		}

		private void SetAiMoveRemain()
		{
			aiMoveRemain = Random.Range(AiMoveMin, AiMoveMAx);
		}

		private void SetBehavior()
		{
			if (!randomBehaviour) return;
			if (Random.Range(0, 100) > 75)
			{
				Behavior = AIBehaviour.Follow;
			}
			else
				Behavior = AIBehaviour.Blocker;
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

		private Vector3 GetTarget(AIBehaviour behaviour, out Transform transform)
		{
			transform = null;
			if (behaviour == AIBehaviour.Follow)
			{				 
				if (GameManager.Instance.PlayerWithPickup?.GetInstanceID() == this.gameObject.GetInstanceID())
				{
					transform = GameManager.Instance.Home.transform;
					return transform.position;
				}
				else
				{
					transform = GameManager.Instance.PickupObject?.transform;
					return transform?.position ?? Vector3.zero;
				}
			}

			if (behaviour == AIBehaviour.Blocker)
			{
				Vector3 targetDir = GameManager.Instance.PickupObject?.transform.position ?? Vector3.zero - GameManager.Instance.Home.transform.position;
				targetDir.Normalize();
				transform = GameManager.Instance.Home.transform;
				return transform.position + targetDir;
			}
			return Vector3.zero;		
		}

		private Vector3 GetMoveVector()
		{
			Vector3 target = GetTarget(Behavior, out _);
			if (target == null) return Vector3.zero;
			var destination = transform.position - target;
			Vector3 targetVector;
			if (Mathf.Abs(destination.x) > Mathf.Abs(destination.z))
			{
				targetVector = Vector3.left * destination.x;
			}
			else
			{
				targetVector = Vector3.back * destination.z;
			}
			targetVector.Normalize();
			return targetVector;
		}

		private void Update()
		{
			if (_moving || GameManager.Instance.Paused)
				return;

			aiMoveRemain -= Time.deltaTime;
			if (aiMoveRemain > 0) return;
			SetAiMoveRemain();
			bool random = Random.Range(0,100) > 100 - randomMoveChance;

			if (random)
			{
				int index = Random.Range(0, _movementVectorDict.Count);
				var action = _movementVectorDict.Keys.ToList()[index];
				_movementVector = _movementVectorDict[action];
			}
			else
			{
				_movementVector = GetMoveVector();
			}

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
			if (targetPosition == new Vector3(0, 0, 7) && !PickedUp)
				return false;
			
			// Check boundaries movement.
			if (Mathf.Abs(targetPosition.z) >= _borders.z)
				return false;
			if (Mathf.Abs(targetPosition.x) >= _borders.x)
				return false;
			
			return true;
		}
	}

	public enum AIBehaviour
	{
		Follow,
		Blocker
	}
}