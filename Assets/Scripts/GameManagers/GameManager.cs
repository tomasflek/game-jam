using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Events;
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
		public Dictionary<int, int> PlayerGameObjectIdPlayerIndex = new();
		public Dictionary<int, int> PlayerIndexSelectedCharacterPrefabIndex = new();

		[SerializeField] public List<GameObject> CharacterPrefabs;
		[SerializeField] private GameObject _playerPrefab;
		[SerializeField] private GameObject _pickupPrefab;
		[SerializeField] private GameObject _homePrefab;
		
		[SerializeField] private Vector3 _gamePlaneSize = new Vector3(7,0,7);

		private GameObject _pickup;
		public bool Paused { get; set; }
		public GameObject PlayerWithPickup { get; set; }

		protected override void Awake()
		{
			base.Awake();
			StartPlayerRegistration();
			EventManager.Instance.Register<DeliveryEvent>(OnDelivery);
		}

		private void OnDelivery(DeliveryEvent obj)
		{
			// remove pickup and spawn a new one.
			Destroy(_pickup);

			var x = UnityEngine.Random.Range(-_gamePlaneSize.x + 1, _gamePlaneSize.x - 1);
			var z = UnityEngine.Random.Range(-_gamePlaneSize.z + 1, _gamePlaneSize.z - 1);
			var pickupPosition = new Vector3((float)Math.Truncate(x),
			                                 0,
			                                 (float)Math.Truncate(z));

			_pickup = Instantiate(_pickupPrefab, pickupPosition, Quaternion.identity);
		}

		public void StartPlayerRegistration()
		{
			EventManager.Instance.Register<InputKeyEvent>(OnInputKey);
			
		}

		public void EndPlayerRegistration()
		{
			EventManager.Instance.Unregister<InputKeyEvent>(OnInputKey);
		}

		private void OnInputKey(InputKeyEvent inputKeyEvent)
		{
			if (inputKeyEvent.Action == InputAction.Start)
			{
				// Start the game.
				if (PlayerIndexType.Any())
					GameStart();

				return;
			}

			if (!PlayerIndexType.ContainsKey(inputKeyEvent.ControllerIndex))
			{
				PlayerIndexType.Add(inputKeyEvent.ControllerIndex, inputKeyEvent.ControllerType);
				PlayerIndexSelectedCharacterPrefabIndex.Add(inputKeyEvent.ControllerIndex, 0);
				Debug.Log($"Player with index {inputKeyEvent.ControllerIndex} has character with index {0}");
				EventManager.Instance.SendEvent(new PlayerSelectionEvent());
			}
			else
			{
				var index = PlayerIndexSelectedCharacterPrefabIndex[inputKeyEvent.ControllerIndex];
				var newIndex = index >= CharacterPrefabs.Count - 1 ? 0 : index + 1;
				PlayerIndexSelectedCharacterPrefabIndex[inputKeyEvent.ControllerIndex] = newIndex;
				Debug.Log($"Player with index {inputKeyEvent.ControllerIndex} has character with index {newIndex}");
				EventManager.Instance.SendEvent(new PlayerSelectionEvent());
			}
		}

		private void GameStart()
		{
			if (SceneManager.GetActiveScene().name != "GameScene")
			{
				LoadGameScene();
			}
		}

		private void LoadGameScene()
		{
			PlayerGameObjectIdPlayerIndex = new();

			SceneManager.LoadScene("GameScene");
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			EndPlayerRegistration();
			
			// Find spawning points for players.
			var playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn").ToList();
			var homeSpawn = GameObject.FindGameObjectsWithTag("HomeSpawn").First();

			foreach (var playerIndexSelectedCharacterPrefabIndex in PlayerIndexSelectedCharacterPrefabIndex)
			{
				var prefabIndex = playerIndexSelectedCharacterPrefabIndex.Value;
				var playerIndex = playerIndexSelectedCharacterPrefabIndex.Key;
				var charPrefab = CharacterPrefabs[prefabIndex];

				// get random respawn point
				var random = new System.Random();
				var randomIndex = random.Next(0, playerSpawns.Count);

				var respawnPoint = playerSpawns[randomIndex];
				var player = Instantiate(_playerPrefab, respawnPoint.transform.position, Quaternion.identity);
				player.name = playerIndex.ToString();
				PlayerGameObjectIdPlayerIndex[player.GetInstanceID()] = playerIndex;

				var playerController = player.GetComponent<PlayerController>();
				playerController.PlayerIndex = playerIndex;
				var character = Instantiate(charPrefab, playerSpawns[randomIndex].transform.position, Quaternion.identity);
				character.transform.parent = player.transform;
				playerSpawns.Remove(respawnPoint);
			}

			// spawn a home for the player
			
			Instantiate(_homePrefab, homeSpawn.transform.position, Quaternion.identity);
			
			// Spawn pickup
			var pickaupSpawningPoint = GameObject.FindGameObjectWithTag("PickupSpawn");
			_pickup = Instantiate(_pickupPrefab, pickaupSpawningPoint.transform.position, Quaternion.identity);
		}

		private void OnDestroy()
		{
			EndPlayerRegistration();
			EventManager.Instance.Unregister<DeliveryEvent>(OnDelivery);
		}

		public void Pickup(Transform player)
		{
			PlayerWithPickup = player.gameObject;
			_pickup.transform.position = player.transform.position;
			_pickup.transform.Translate(Vector3.up);
			_pickup.transform.parent = player.transform;
		}
	}
}