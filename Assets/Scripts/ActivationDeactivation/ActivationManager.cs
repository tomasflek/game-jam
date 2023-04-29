using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Interface;

namespace ActivationDeactivation
{
    public sealed class ActivationManager : Singleton<ActivationManager>
    {
        #region Fields

        private Dictionary<int, IControllerActivable> _controllers = new Dictionary<int, IControllerActivable>();

        #endregion

        #region Public methods

        /// <summary>
        /// Register a controller for ActionvationManager.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="activate"></param>
        public void Register(IControllerActivable controller, bool activate)
        {
            if (controller == null)
                return;

            int hash = controller.GetHashCode();
            if (ControllerRegistered(hash)) 
                return;
            
            _controllers.Add(controller.GetHashCode(), controller);

            if (activate)
                Activate(controller, this);
            else
                Deactivate(controller, this);
        }

        /// <summary>
        /// Activates a controller.
        /// </summary>
        /// <param name="controller"></param>
        public void UnRegister(IControllerActivable controller)
        {
            if (controller == null)
                return;

            int hash = controller.GetHashCode();
            if (!ControllerRegistered(hash)) 
                return;
            
            _controllers.Remove(hash);
            Deactivate(controller, this);
        }

        /// <summary>
        /// Activate given controller.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="activator"></param>
        public void Activate(IControllerActivable controller, object activator)
        {
            if (controller == null)
                return;

            int hash = controller.GetHashCode();

            if (!ControllerRegistered(hash))
                return;

            controller.ControllerState = ControllerState.Active;
            controller.ActivateController(activator);
        }

        /// <summary>
        /// Deactivates a controller.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="deactivator"></param>
        public void Deactivate(IControllerActivable controller, object deactivator)
        {
            if (controller == null)
                return;

            int hash = controller.GetHashCode();

            if (!ControllerRegistered(hash))
                return;

            controller.ControllerState = ControllerState.Inactive;
            controller.DeactivateController(deactivator);
        }

        /// <summary>
        /// Deactivates all controllers of given type.
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="deactivator"></param>
        public void DeactivateAllOfType(Type controllerType, object deactivator)
        {
            foreach (var keyValuePair in _controllers.Where(p => p.GetType() == controllerType))
            {
                keyValuePair.Value.ControllerState = ControllerState.Inactive;
                keyValuePair.Value.DeactivateController(deactivator);
            }
        }

        /// <summary>
        /// Activates all controllers of given type.
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="activator"></param>
        public void ActivateAllOfType(Type controllerType, object activator)
        {
            foreach (var keyValuePair in _controllers.Where(p => p.GetType() == controllerType))
            {
                keyValuePair.Value.ControllerState = ControllerState.Active;
                keyValuePair.Value.ActivateController(activator);
            }
        }

        /// <summary>
        /// Deactivates all controllers except defines.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="deactivator"></param>
        public void DeactivateAllExcept(IControllerActivable controller, object deactivator)
        {
            if (controller == null)
                return;

            int hash = controller.GetHashCode();

            foreach (var keyValuePair in _controllers)
            {
                if (keyValuePair.Key == hash)
                    continue;

                Deactivate(keyValuePair.Value, deactivator);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns information whether a controller has been already registered.
        /// </summary>
        /// <param name="controllerHash"></param>
        /// <returns></returns>
        private bool ControllerRegistered(int controllerHash)
        {
            IControllerActivable foundController;
            _controllers.TryGetValue(controllerHash, out foundController);

            return foundController != null;
        }

        #endregion
    }
}