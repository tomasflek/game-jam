using Events;
using Inputs;

/// <summary>
/// Event used to propagate key pressed by a user.
/// </summary>
public class InputKeyEventUI : EventBase
{
    /// <summary>
    /// Key pressed by a user.
    /// </summary>
    public InputAction Action { get; }

    public int ControllerIndex { get; }
    public ControllerType ControllerType { get; }
		
		
    public InputKeyEventUI(InputAction action, int controllerIndex, ControllerType controllerType)
    {
        Action = action;
        ControllerIndex = controllerIndex;
        ControllerType = controllerType;
    }
}