using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Events;
using GameManagers;
using TMPro;
using UnityEngine;

public class UIScoreCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private List<TextMeshProUGUI> _playerScoreText;

    private void Start()
    {
        EventManager.Instance.Register<ScoreEvent>(OnScoreChanged);
        StringBuilder sb = new StringBuilder();
        sb.Append("Score:\n\n");
        foreach (var player in GameManager.Instance.Players)
        {
            sb.Append($"{player.PlayerName}\n");
        }
        ScoreText.text = sb.ToString();
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unregister<ScoreEvent>(OnScoreChanged);
    }

    private void OnScoreChanged(ScoreEvent scoreEvent)
    {
        var playerIndex = GameManager.Instance.Players.IndexOf(scoreEvent.Player);
        if (playerIndex >= 0 && playerIndex < _playerScoreText.Count)
        {
            _playerScoreText[playerIndex].text = scoreEvent.Player.Score.ToString();
        }
    }
}
