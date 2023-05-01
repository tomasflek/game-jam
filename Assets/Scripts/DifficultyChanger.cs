using Events;
using Events.Input;
using GameManagers;
using Inputs;
using TMPro;
using UnityEngine;

public class DifficultyChanger : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _difficultyText;
    // Start is called before the first frame update
    void Awake()
    {
        EventManager.Instance.Register<InputKeyEvent>(OnInputKey);
    }

    private void OnInputKey(InputKeyEvent obj)
    {
        if (obj.Action is InputAction.Select)
        {
            GameManager.Instance.ChangeDifficulty();

            _difficultyText.text = GameManager.Instance.Difficulty is Difficulty.ClassicalControls ?
                "CLASSICAL CONTROL" :
                "RANDOM CONTROL";
        }
    }

    // Update is called once per frame
    void OnDestroy()
    {
        EventManager.Instance.Unregister<InputKeyEvent>(OnInputKey);
    }
}
