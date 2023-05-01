using System.Collections.Generic;
using System.Linq;
using Character;
using Events;
using Events.Input;
using Helpers;
using Inputs;
using UnityEngine;

namespace GameManagers
{
	public class BattleManager : UnitySingleton<BattleManager>
	{
		public int ComboCounter = 5;
		public float AiPressTimerMin = 0.5f;
		public float AiPressTimerMax = 1.5f;

		private Camera _mainCamera;
		public Camera battleCamera;

		private bool _isBattling;
		[SerializeField]
		private Transform _playerOneFightPosition;
		[SerializeField] 
		private Transform _playerTwoFightPosition;
		public Canvas BattleCanvas;
		public GameObject PlayerOnePanel;
		public GameObject PlayerTwoPanel;
		private Vector3 _playerOneStartPosition;
		private Vector3 _playerTwoStartPosition;
		private GameObject _playerOne;
		private GameObject _playerTwo;
		private int _playerOneControllerIndex;
		private int _playerTwoControllerIndex;
		private InputAction? _playerOneInput;
		private InputAction? _playerTwoInput;

		private int _playerOneCurrentCombo = 0;
		private int _playerTwoCurrentCombo = 0;

		private List<InputAction> _currentCombo;

		private bool _playerOneAi = false;
		private bool _playerTwoAi = false;
		private float _playerOneAiTimer = 0f;
		private float _playerTwoAiTimer = 0f;

		public GameObject playerOneMock;
		public GameObject playerTwoMock;

		private List<Transform> playerOneModelToTransport;
		private List<Transform> playerTwoModelToTransport;

		protected override void Awake()
		{
			base.Awake();
			
			_mainCamera = Camera.main;
			if (battleCamera == null)
			{
				battleCamera = Camera.main;
			}
		}

		void Start()
		{
			// mock data
			if (playerOneMock != null && playerTwoMock != null) 
				StartBattle(playerOneMock, playerTwoMock);
		}

		void Update()
		{
			if (!_isBattling) return;

			if (_playerOneAi)
			{
				_playerOneAiTimer -= Time.deltaTime;
				if (_playerOneAiTimer < 0f)
				{
					_playerOneInput = _currentCombo[_playerOneCurrentCombo];
					_playerOneAiTimer = GetNewAiTimer();
				}
			}
			if (_playerTwoAi)
			{
				_playerTwoAiTimer -= Time.deltaTime;
				if (_playerTwoAiTimer < 0f)
				{
					_playerTwoInput = _currentCombo[_playerTwoCurrentCombo];
					_playerTwoAiTimer = GetNewAiTimer();
				}
			}

			if (_playerOneInput.HasValue)
			{
				CheckCombo(PlayerOnePanel, ref _playerOneCurrentCombo, _playerOneControllerIndex, _playerOne, _playerOneInput.Value, _playerTwo);
			}
			if (_playerTwoInput.HasValue)
			{
				CheckCombo(PlayerTwoPanel, ref _playerTwoCurrentCombo, _playerTwoControllerIndex, _playerTwo, _playerTwoInput.Value, _playerOne);
			}

			_playerOneInput = null;
			_playerTwoInput = null;
		}

		private void CheckCombo(GameObject playerPanel, ref int comboIndex, int playerIndex, GameObject player, InputAction inputAction, GameObject otherPlayer)
		{
			if (_currentCombo[comboIndex] == inputAction)
			{
				playerPanel.transform.GetChild(comboIndex).gameObject.SetActive(false);
				comboIndex++;
				if (comboIndex == ComboCounter)
				{
					// END BATTLE
					GameManager.Instance.Pickup(player.transform);
					EndBattle(otherPlayer);
				}
			}
			else
			{
				comboIndex = 0;
				GenerateComboButtons(playerPanel, playerIndex);
			}
		}

		private void SetBattleCamera()
		{
			if (battleCamera == Camera.main) return;
			battleCamera.gameObject.SetActive(true);
			_mainCamera.gameObject.SetActive(false);
		}

		private void EndBattleCamera()
		{
			if (battleCamera == Camera.main) return;
			battleCamera.gameObject.SetActive(false);
			_mainCamera.gameObject.SetActive(true);
		}

		public void StartBattle(GameObject playerOne, GameObject playerTwo)
		{
			GameManager.Instance.Paused = true;
			_playerOne = playerOne;
			_playerTwo = playerTwo;
			_playerOneStartPosition = playerOne.transform.position;
			_playerTwoStartPosition = playerTwo.transform.position;
			_playerOneAi = playerOne.GetComponent<PlayerController>() == null;
			_playerTwoAi = playerTwo.GetComponent<PlayerController>() == null;

			TransportToBattle();

			BattleCanvas.gameObject.SetActive(true);
			EventManager.Instance.Register<InputKeyEvent>(OnInputKey);
			
			if (_playerOneAi)
				_playerOneAiTimer = GetNewAiTimer();
			if (_playerTwoAi)
				_playerTwoAiTimer = GetNewAiTimer();


			if (GameManager.Instance.PlayerGameObjectIdPlayerIndex.ContainsKey(playerOne.GetInstanceID()))
				_playerOneControllerIndex = GameManager.Instance.PlayerGameObjectIdPlayerIndex[playerOne.GetInstanceID()];
			if (GameManager.Instance.PlayerGameObjectIdPlayerIndex.ContainsKey(playerTwo.GetInstanceID()))
				_playerTwoControllerIndex = GameManager.Instance.PlayerGameObjectIdPlayerIndex[playerTwo.GetInstanceID()];

			_playerOneCurrentCombo = _playerTwoCurrentCombo = 0;
				
			GenerateCombo();
			GenerateComboButtons(PlayerOnePanel, _playerOneControllerIndex);
			GenerateComboButtons(PlayerTwoPanel, _playerTwoControllerIndex);

			SetBattleCamera();
			AudioManager.Instance.PlayMusicSound("PostmanBattle", false);
			AudioManager.Instance.MusicEnd += PlayBattleMusic;

			_isBattling = true;
		}

		private void TransportToBattle()
		{
			playerOneModelToTransport = new List<Transform>();
			playerTwoModelToTransport = new List<Transform>();
			// FILTER HACK
			foreach (Transform item in _playerOne.transform)
			{
				if (item.gameObject.tag != "Pickup" &&
					item.gameObject.name != "MovementCanvas")
				{
					playerOneModelToTransport.Add(item);
				}
			}
			foreach (Transform item in _playerTwo.transform)
			{
				if (item.gameObject.tag != "Pickup" &&
					item.gameObject.name != "MovementCanvas")
				{ 
					playerTwoModelToTransport.Add(item);
				}
			}

			// TRANSPORT HACK
			foreach (Transform item in playerOneModelToTransport)
			{
				item.SetParent(_playerOneFightPosition, false);
			}
			foreach (Transform item in playerTwoModelToTransport)
			{
				item.SetParent(_playerTwoFightPosition, false);
			}
		}

		private void TransportBack()
		{
			foreach (Transform item in playerOneModelToTransport)
			{
				item.SetParent(_playerOne.transform, false);
			}
			foreach (Transform item in playerTwoModelToTransport)
			{
				item.SetParent(_playerTwo.transform, false);
			}
		}

		public void EndBattle(GameObject loser)
		{
			// Remove next music event. If battle ends before postman battle music finishes, battle music will be playing instead of theme music
			AudioManager.Instance.MusicEnd -= PlayBattleMusic;
			_isBattling = false;
			EventManager.Instance.Unregister<InputKeyEvent>(OnInputKey);

			TransportBack();

			var playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn").ToList();
			var respawnPoint = playerSpawns[Random.Range(0, playerSpawns.Count)];
			loser.transform.position = respawnPoint.transform.position;

			BattleCanvas.gameObject.SetActive(false);
			EndBattleCamera();

			AudioManager.Instance.PlayMusicSound("ThemeMusic", true);

			GameManager.Instance.Paused = false;
		}

		private void PlayBattleMusic()
		{
			AudioManager.Instance.PlayMusicSound("BattleMusic", true);
		}

		private float GetNewAiTimer()
		{
			return Random.Range(AiPressTimerMin, AiPressTimerMax);
		}

		private void GenerateCombo()
		{
			_currentCombo = new List<InputAction>(ComboCounter);
			for (int i = 0; i < ComboCounter; i++)
			{
				_currentCombo.Add((InputAction)Random.Range(0, 4));
			}
		}

		private void GenerateComboButtons(GameObject playerPanel, int playerIndex)
		{
			foreach (Transform child in playerPanel.transform)
			{
				Object.Destroy(child.gameObject);
			}
			foreach (InputAction inputAction in _currentCombo)
			{
				if (!GameManager.Instance.PlayerIndexType.TryGetValue(playerIndex, out ControllerType controllerType))
				{
					controllerType = ControllerType.Xbox;
				}
				GameObject button = UIManager.Instance.GetComboButton(inputAction, controllerType);
				button.transform.SetParent(playerPanel.transform, false);
			}
		}

		private void OnInputKey(InputKeyEvent inputKeyEvent)
		{
			if (_playerOneControllerIndex == inputKeyEvent.ControllerIndex)
			{
				_playerOneInput = inputKeyEvent.Action;
			}
			if (_playerTwoControllerIndex == inputKeyEvent.ControllerIndex)
			{
				_playerTwoInput = inputKeyEvent.Action;
			}
		}
	}
}
