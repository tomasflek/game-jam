using System.Collections.Generic;
using System.Linq;
using Events;
using Events.Input;
using Helpers;
using Inputs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace GameManagers
{
	public class GameManager : UnitySingleton<GameManager>
	{
		//int controllerIndex, ControllerType controllerType
		public Dictionary<int, ControllerType> PlayerIndexType = new Dictionary<int, ControllerType>();

		// map player gameObject id to controller index
		public Dictionary<int, int> GameObjectIdPlayerIndex = new Dictionary<int, int>();
		public Dictionary<int, int> PlayerGameObjectIdHomeGameObjectId = new Dictionary<int, int>();

		[SerializeField] private List<GameObject> _characterPrefabs;
		[FormerlySerializedAs("_playerPrefabs")]
		[SerializeField] private GameObject _playerPrefab;

		private Dictionary<int, int> _playerIndexSelectedCharacterPrefabIndex = new Dictionary<int, int>();

		protected override void Awake()
		{
			base.Awake();
			StartPlayerRegistry();
			EventManager.Instance.Register<DeliveryEvent>(OnDelivery);
		}

		private void OnDelivery(DeliveryEvent obj)
		{
			Debug.Log("Delivery");
		}

		public void StartPlayerRegistry()
		{
			EventManager.Instance.Register<InputKeyEvent>(OnInputKey);
		}

		public void EndPlayerRegistry()
		{
			EventManager.Instance.Unregister<InputKeyEvent>(OnInputKey);
		}

		private void OnInputKey(InputKeyEvent inputKeyEvent)
		{
			if (inputKeyEvent.KeyPress is KeyPress.Pressed && inputKeyEvent.Action == InputAction.Start)
			{
				// Start the game.
				if (PlayerIndexType.Any())
					GameStart();

				return;
			}

			if (inputKeyEvent.KeyPress is KeyPress.Pressed)
			{
				if (!PlayerIndexType.ContainsKey(inputKeyEvent.ControllerIndex))
				{
					PlayerIndexType.Add(inputKeyEvent.ControllerIndex, inputKeyEvent.ControllerType);
					_playerIndexSelectedCharacterPrefabIndex.Add(inputKeyEvent.ControllerIndex, 0);
					Debug.Log($"Player with index {inputKeyEvent.ControllerIndex} has character with index {0}");
				}
				else
				{
					var index = _playerIndexSelectedCharacterPrefabIndex[inputKeyEvent.ControllerIndex];
					var newIndex = index >= _characterPrefabs.Count - 1 ? 0 : index + 1;
					_playerIndexSelectedCharacterPrefabIndex[inputKeyEvent.ControllerIndex] = newIndex;
					Debug.Log($"Player with index {inputKeyEvent.ControllerIndex} has character with index {newIndex}");
					// HANDLE SELECTION INPUTS AND PREFAB SELECTION AND START/END
				}
			}
		}

		private void GameStart()
		{
			SceneManager.LoadScene("GameScene");
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			// Find spawning points.
			var respawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
			foreach (var respawn in respawns)
			{
				respawn.SetActive(false);
			}

			foreach (var playerIndexSelectedCharacterPrefabIndex in _playerIndexSelectedCharacterPrefabIndex)
			{
				var prefabIndex = playerIndexSelectedCharacterPrefabIndex.Value;
				var charPrefab = _characterPrefabs[prefabIndex];
				
				// get random respawn point
				var random = new System.Random();
				var randomIndex = random.Next(0, respawns.Length);
				
				var player = Instantiate(_playerPrefab, respawns[randomIndex].transform.position, Quaternion.identity);
				var character = Instantiate(charPrefab, respawns[randomIndex].transform.position, Quaternion.identity);
				
			}
		}

		public void OnDestroy()
		{
			EndPlayerRegistry();
		}
	}
}