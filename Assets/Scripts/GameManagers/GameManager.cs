using System.Collections.Generic;
using Events.Input;
using Events;
using Helpers;
using Inputs;

namespace Assets.Scripts.GameManagers
{
	public class GameManager : UnitySingleton<GameManager> 
	{
		//int controllerIndex, ControllerType controllerType
		public Dictionary<int, ControllerType> PlayerIndexType = new Dictionary<int, ControllerType>();
		public Dictionary<int, int> GameObjectIdPlayerIndex = new Dictionary<int, int>();

		// Start is called before the first frame update
		void Start()
		{
        
		}

		// Update is called once per frame
		void Update()
		{
        
		}

		public void StartPlayerRegistry()
		{
			ActivateController(this);
		}

		public void EndPlayerRegistry()
		{
			DeactivateController(this);
		}

		private void OnInputKey(InputKeyEvent inputKeyEvent)
		{
			if (inputKeyEvent.KeyPress is KeyPress.Pressed)
			{
				if (!PlayerIndexType.ContainsKey(inputKeyEvent.ControllerIndex))
				{
					PlayerIndexType.Add(inputKeyEvent.ControllerIndex, inputKeyEvent.ControllerType);
				}
				else
				{
					// HANDLE SELECTION INPUTS AND PREFAB SELECTION AND START/END
				}
			}
		}

		public void OnDestroy()
		{
			DeactivateController(this);
		}

		public void ActivateController(object activator)
		{
			EventManager.Instance.Register<InputKeyEvent>(OnInputKey);
		}

		/// <summary>
		/// Disables character controller and unregisters all events.
		/// </summary>
		public void DeactivateController(object deactivator)
		{
			EventManager.Instance.Unregister<InputKeyEvent>(OnInputKey);
		}
	}
}
