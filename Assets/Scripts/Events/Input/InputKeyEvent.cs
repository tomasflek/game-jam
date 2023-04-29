
using Inputs;

namespace Events.Input
{
	/// <summary>
	/// Event used to propagate key pressed by a user.
	/// </summary>
	public class InputKeyEvent : EventBase
	{
		/// <summary>
		/// Key pressed by a user.
		/// </summary>
		public InputAction Action { get; }

		/// <summary>
		/// Was a key pressed or release?
		/// </summary>
		public KeyPress KeyPress { get; }
		
		public int ControllerIndex { get; }
		public ControllerType ControllerType { get; }
		
		
		public InputKeyEvent(InputAction action, KeyPress keyPress, int controllerIndex, ControllerType controllerType)
		{
			Action = action;
			KeyPress = keyPress;
			ControllerIndex = controllerIndex;
			ControllerType = controllerType;
		}
		
		public InputKeyEvent(InputAction action, KeyPress keyPress)
		{
			Action = action;
			KeyPress = keyPress;
			// ControllerIndex = controllerIndex;
		}
	}
	
	public enum KeyPress
	{
		Pressed,
		Released,
	}
}
