using System.Collections.Generic;
using Assets.Scripts.Events;
using Events.Input;
using Events;
using GameManagers;
using Inputs;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SelectionVisualizer : MonoBehaviour
{
	[SerializeField] private List<Transform> _playerHookUps;
	[SerializeField] private List<TextMeshProUGUI> _playerNames;
	private List<int> _playerIndices = new List<int>(4);

	private void Start()
	{
		EventManager.Instance.Register<PlayerSelectionEvent>(VisualizePlayerSelection);
	}

	private void OnDestroy()
	{
		EventManager.Instance.Unregister<PlayerSelectionEvent>(VisualizePlayerSelection);
	}

	private void VisualizePlayerSelection(PlayerSelectionEvent playerSelectionEvent)
	{
		var characterPrefabs = GameManager.Instance.CharacterPrefabs;
		foreach (KeyValuePair<int, int> index  in GameManager.Instance.PlayerIndexSelectedCharacterPrefabIndex)	
		{
			if (!_playerIndices.Contains(index.Key))
				_playerIndices.Add(index.Key);

			int playerIndex = _playerIndices.IndexOf(index.Key);
			Transform hookup = _playerHookUps[playerIndex];
			// Destroy if some object already present
			if (hookup.childCount > 0)
				Destroy(hookup.GetChild(0).gameObject);
			Instantiate(characterPrefabs[index.Value], hookup);
			_playerNames[playerIndex].text = $"Player {playerIndex + 1}";
		}
	}
}
