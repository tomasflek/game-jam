using System.Collections;
using ActivationDeactivation;
using Events;
using Events.Input;
using Inputs;
using Interface;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Animator))]
    public class CharacterController : MonoBehaviour, IControllerActivable
    {
        private Vector3 _targetMovement;
        private Vector3 _sourceMovement;
        private Animator _animator;
        private static readonly int IsMovingTrigger = Animator.StringToHash("IsMovingTrigger");
        private float _movementDuration;
        private bool _moving;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _movementDuration = GetMovementDuration();
            RegisterController();
        }

        private float GetMovementDuration()
        {
            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                switch (clip.name)
                {
                    case "Jump":
                        return clip.length;
                }
            }
            return 0;
        }

        IEnumerator LerpPosition(Vector3 targetPosition, float duration)
        {
            _moving = true;
            var time = 0f;
            var startPosition = transform.position;
            while (time < duration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
            _moving = false;
        }
        
        private void OnInputKey(InputKeyEvent inputKeyEvent)
        {
            if (inputKeyEvent.KeyPress is not KeyPress.Pressed || _moving) 
                return;
        
            switch (inputKeyEvent.Action)
            {
                case InputAction.Left:
                    _targetMovement = Vector3.left;
                    break;
                case InputAction.Right:
                    _targetMovement = Vector3.right;
                    break;
                case InputAction.Up:
                    _targetMovement = Vector3.forward;
                    break;
                case InputAction.Down:
                    _targetMovement = Vector3.back;
                    break;
            }
        
            _animator.SetTrigger(IsMovingTrigger);
            StartCoroutine(LerpPosition(_targetMovement + transform.position, _movementDuration));
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

        /// <summary>
        /// Disables character controller and unregisters all events.
        /// </summary>
        public void DeactivateController(object deactivator)
        {
            EventManager.Instance.Unregister<InputKeyEvent>(OnInputKey);
        }

        #endregion IControllerActivable implementation
    }
}
