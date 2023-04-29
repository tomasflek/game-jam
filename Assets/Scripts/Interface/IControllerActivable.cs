namespace Interface
{
    /// <summary>
    /// IControllerActivable interface is used for all controllers (rope controller, character controller, etc ...)
    /// where controller can be switched on to inactive or active state.
    /// </summary>
    public interface IControllerActivable
    {
        #region Properties

        ControllerState ControllerState { get; set; }

        #endregion

        #region Methods

        void ActivateController(object activator);
        void DeactivateController(object deactivator);
        void RegisterController();
        void UnregisterController();

        #endregion
    }

    public enum ControllerState
    {
        Inactive,
        Active,
    }
}
