using System.Collections.Generic;
using GameManagers;
using TMPro;
using UnityEngine;

public class SelectionVisualizer : MonoBehaviour
{
	[SerializeField] private List<Transform> _playerHookUps;
	[SerializeField] private List<TextMeshProUGUI> _playerNames;

	private void Start()
	{
		GameManager.Instance.PlayerSelectedEvent += VisualizePlayerSelection;
	}

	private void VisualizePlayerSelection(int controllerIndex, int avatarIndex)
	{
		var characterPrefabs = GameManager.Instance.CharacterPrefabs;
		if (_playerHookUps.Count != characterPrefabs.Count || _playerNames.Count != characterPrefabs.Count)
		{
			Debug.LogWarning("Suspiscisisiously different number of slots");
		}

		if (avatarIndex >= characterPrefabs.Count)
		{
			Debug.LogWarning("Player index is suspiscisisiously larger than number of prefabs");
			return;
		}

		Transform hookUp = _playerHookUps[avatarIndex];
		// Destroy if figure is already there
		if (hookUp.childCount > 0)
		{
			Destroy(hookUp.transform.GetChild(0).gameObject);
		}
		Instantiate(characterPrefabs[avatarIndex], hookUp);
		_playerNames[avatarIndex].text = $"Player {avatarIndex + 1}";
	}
}
