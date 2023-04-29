using System;
using ActivationDeactivation;
using Events;
using Events.Input;
using Interface;
using UnityEngine;

public class CharacterController : MonoBehaviour, IControllerActivable
{
    private void Awake()
    {
        RegisterController();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region IControllerActivable implementation

    public void RegisterController()
    {
        ActivationManager.Instance.Register(this, true);
    }

    public void UnregisterController()
    {
        ActivationManager.Instance.UnRegister(this);
    }
    public ControllerState ControllerState { get; set; }

    /// <summary>
    /// Enables character controller and registers all events.
    /// </summary>
    public void ActivateController(object activator)
    {
        EventManager.Instance.Register<InputKeyEvent>(OnInputKey);
    }

    private void OnInputKey(InputKeyEvent obj)
    {
        Debug.Log(obj.Action);
    }

    /// <summary>
    /// Disables character controller and unregisters all events.
    /// </summary>
    public void DeactivateController(object deactivator)
    {
        EventManager.Instance.Unregister<InputKeyEvent>(OnInputKey);
    }

    #endregion IControllerActivable implementation

}
