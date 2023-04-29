using System.Collections.Generic;
using Character;
using Events;
using Events.Input;
using GameManagers;
using Inputs;
using UnityEngine;

namespace Assets.Scripts.GameManagers
{
	public class BattleManager : MonoBehaviour
	{
		public int ComboCounter = 5;
		public float AiPressTimerMin = 0.5f;
		public float AiPressTimerMax = 1.5f;
		private bool _isBattling;
		[SerializeField] private Transform _playerOneFightPosition;
		[SerializeField] private Transform _playerTwoFightPosition;
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

		void Awake()
		{

		}

		void Start()
		{

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
				}
			}
			if (_playerTwoAi)
			{
				_playerTwoAiTimer -= Time.deltaTime;
				if (_playerTwoAiTimer < 0f)
				{
					_playerTwoInput = _currentCombo[_playerTwoCurrentCombo];
				}
			}

			if (_playerOneInput.HasValue)
			{
				CheckCombo(PlayerOnePanel, ref _playerOneCurrentCombo, _playerOneControllerIndex, _playerOneInput.Value);
			}
			if (_playerTwoInput.HasValue)
			{
				CheckCombo(PlayerTwoPanel, ref _playerTwoCurrentCombo, _playerTwoControllerIndex, _playerTwoInput.Value);
			}

			_playerOneInput = null;
			_playerTwoInput = null;
		}

		private void CheckCombo(GameObject playerPanel, ref int comboIndex, int playerIndex, InputAction inputAction)
		{
			if (_currentCombo[comboIndex] == inputAction)
			{
				playerPanel.transform.GetChild(comboIndex).gameObject.SetActive(false);
				comboIndex++;
				if (comboIndex == ComboCounter)
				{
					// END BATTLE
					EndBattle();
				}
			}
			else
			{
				comboIndex = 0;
				GenerateComboButtons(playerPanel, playerIndex);
			}
		}

		public void StartBattle(GameObject playerOne, int comboIndex, GameObject playerTwo)
		{
			_playerOne = playerOne;
			_playerTwo = playerTwo;
			_playerOneStartPosition = playerOne.transform.position;
			_playerTwoStartPosition = playerTwo.transform.position;
			_playerOne.transform.position = _playerOneFightPosition.position;
			_playerTwo.transform.position = _playerTwoFightPosition.position;
			BattleCanvas.gameObject.SetActive(true);
			ActivateController(this);
			_playerOneAi = playerOne.GetComponent<PlayerController>() != null;
			_playerTwoAi = playerTwo.GetComponent<PlayerController>() != null;
			if (_playerOneAi)
				_playerOneAiTimer = GetNewAiTimer();
			if (_playerTwoAi)
				_playerTwoAiTimer = GetNewAiTimer();


			if (GameManager.Instance.GameObjectIdPlayerIndex.ContainsKey(playerOne.GetInstanceID()))
				_playerOneControllerIndex = GameManager.Instance.GameObjectIdPlayerIndex[playerOne.GetInstanceID()];
			if (GameManager.Instance.GameObjectIdPlayerIndex.ContainsKey(playerTwo.GetInstanceID()))
				_playerTwoControllerIndex = GameManager.Instance.GameObjectIdPlayerIndex[playerTwo.GetInstanceID()];

			GenerateCombo();
			GenerateComboButtons(PlayerOnePanel, _playerOneControllerIndex);
			GenerateComboButtons(PlayerTwoPanel, _playerTwoControllerIndex);

			_isBattling = true;
		}

		public void EndBattle()
		{
			_isBattling = false;
			DeactivateController(this);
			_playerOne.transform.position = _playerOneStartPosition;
			_playerTwo.transform.position = _playerTwoStartPosition;
			BattleCanvas.gameObject.SetActive(false);
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
				_currentCombo[i] = (InputAction) Random.Range(0, 4);
			}
		}

		private void GenerateComboButtons(GameObject playerPanel, int playerIndex)
		{
			foreach (Transform child in playerPanel.transform)
			{
				Destroy(child.gameObject);
			}
			foreach (InputAction inputAction in _currentCombo)
			{
				GameObject button = UIManager.Instance.GetComboButton(inputAction, GameManager.Instance.PlayerIndexType[playerIndex]);
				button.transform.parent = playerPanel.transform;
			}
		}

		private void OnInputKey(InputKeyEvent inputKeyEvent)
		{
			if (inputKeyEvent.KeyPress is not KeyPress.Pressed)
				return;

			if (_playerOneControllerIndex == inputKeyEvent.ControllerIndex)
			{
				_playerOneInput = inputKeyEvent.Action;
			}
			if (_playerTwoControllerIndex == inputKeyEvent.ControllerIndex)
			{
				_playerTwoInput = inputKeyEvent.Action;
			}
		}

		public void ActivateController(object activator)
		{
			EventManager.Instance.Register<InputKeyEvent>(OnInputKey);
		}

		public void DeactivateController(object deactivator)
		{
			EventManager.Instance.Unregister<InputKeyEvent>(OnInputKey);
		}
	}
}
