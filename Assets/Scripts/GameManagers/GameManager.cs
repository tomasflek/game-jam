using System.Collections.Generic;
using System.Linq;
using Character;
using Events;
using Events.Input;
using Helpers;
using Inputs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagers
{
	public class GameManager : UnitySingleton<GameManager>
	{
		//int controllerIndex, ControllerType controllerType
		public Dictionary<int, ControllerType> PlayerIndexType = new();

		// map player gameObject id to controller index
		public Dictionary<int, int> GameObjectIdPlayerIndex = new();
		public Dictionary<int, int> PlayerGameObjectIdHomeGameObjectId = new();

		[SerializeField]
		private List<GameObject> _characterPrefabs;
		[SerializeField]
		public GameObject PlayerPrefab;
		[SerializeField]
		private GameObject _pickupPrefab;

		public Dictionary<int, int> PlayerIndexSelectedCharacterPrefabIndex = new();
		public Action<int, int> PlayerSelectedEvent; 
		private GameObject _pickup;
		public bool Paused { get; set; }
		public GameObject PlayerWithPickup { get; set; }

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
					PlayerIndexSelectedCharacterPrefabIndex.Add(inputKeyEvent.ControllerIndex, 0);
					Debug.Log($"Player with index {inputKeyEvent.ControllerIndex} has character with index {0}");
				}
				else
				{
					var index = PlayerIndexSelectedCharacterPrefabIndex[inputKeyEvent.ControllerIndex];
					var newIndex = index >= _characterPrefabs.Count - 1 ? 0 : index + 1;
					PlayerIndexSelectedCharacterPrefabIndex[inputKeyEvent.ControllerIndex] = newIndex;
					Debug.Log($"Player with index {inputKeyEvent.ControllerIndex} has character with index {newIndex}");
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

			foreach (var playerIndexSelectedCharacterPrefabIndex in PlayerIndexSelectedCharacterPrefabIndex)
			{
				var prefabIndex = playerIndexSelectedCharacterPrefabIndex.Value;
				var playerIndex = playerIndexSelectedCharacterPrefabIndex.Key;
				var charPrefab = _characterPrefabs[prefabIndex];
				
				// get random respawn point
				var random = new System.Random();
				var randomIndex = random.Next(0, respawns.Length);
				
				var player = Instantiate(PlayerPrefab, respawns[randomIndex].transform.position, Quaternion.identity);
				var playerController = player.GetComponent<PlayerController>();
				playerController.PlayerIndex = playerIndex;
				var character = Instantiate(charPrefab, respawns[randomIndex].transform.position, Quaternion.identity);
				character.transform.parent = player.transform;
			}
		}

		public void OnDestroy()
		{
			EndPlayerRegistry();
		}
	}
}