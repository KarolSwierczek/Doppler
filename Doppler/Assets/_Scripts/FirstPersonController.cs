using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

namespace WaveTerrain
{
    [RequireComponent(typeof (CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Settings _Settings;
        #endregion Inspector Variables

        #region Private Variables
        private MouseLook                  _MouseLook = new MouseLook();       
        private Camera                     _Camera;
        private CharacterController        _CharacterController;

        private Vector2                    _Velocity = Vector2.zero;
        #endregion Private Variables

        #region Unity Methods
        private void Start()
        {
            _CharacterController = GetComponent<CharacterController>();
            _Camera = Camera.main;
			_MouseLook.Init(transform , _Camera.transform);
        }

        private void Update()
        {
            RotateView();
        }

        private void FixedUpdate()
        {
            //get desired move direction vector
            var desiredMoveDir = GetInput();
            //current acceleration based on player input and friction
            var acceleration = desiredMoveDir * _Settings.Acceleration - _Velocity * _Settings.Friction;
            //current velocity vector clamped, so that the speed does not exceed max speed
            _Velocity = Vector2.ClampMagnitude( _Velocity + acceleration * Time.fixedDeltaTime, _Settings.MaxSpeed);

            //if the speed is smaller than epsilon, reset the velocity to zero
            if(_Velocity.magnitude < _Settings.SpeedCutoff) { _Velocity = Vector2.zero; }

            //move the character with the calculated velocity
            _CharacterController.Move(new Vector3(_Velocity.x, 0f, _Velocity.y) * Time.fixedDeltaTime);
            _MouseLook.UpdateCursorLock();
        }
        #endregion Unity Methods

        #region Private Methods
        /// <summary>
        /// returns desired move direction based on player input at the moment
        /// </summary>
        private Vector2 GetInput()
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            // desired move direction vector
            var desiredMoveDir = (transform.forward * vertical + transform.right * horizontal);

            return new Vector2(desiredMoveDir.x, desiredMoveDir.z).normalized;
        }

        /// <summary>
        /// rotates the main camera along with the mouse look
        /// </summary>
        private void RotateView()
        {
            _MouseLook.LookRotation (transform, _Camera.transform);
        }
        #endregion Private Methods

    }
}
