using System;
using Unity.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private float _MaxSpeed;
        [SerializeField] private float _Acceleration;
        [SerializeField] private float _Friction;
        [SerializeField] private float _Epsilon;
        [SerializeField] private MouseLook _MouseLook;
        [SerializeField] private float _Speed;

        private Vector2 _Velocity = Vector2.zero;
        private Camera _Camera;
        private CharacterController _CharacterController;
        private CollisionFlags _CollisionFlags;
        private Vector3 _OriginalCameraPosition;

        // Use this for initialization
        private void Start()
        {
            _CharacterController = GetComponent<CharacterController>();
            _Camera = Camera.main;
            _OriginalCameraPosition = _Camera.transform.localPosition;
			_MouseLook.Init(transform , _Camera.transform);
        }


        // Update is called once per frame
        private void Update()
        {
            RotateView();
        }

        private void FixedUpdate()
        {
            var desiredMove = GetInput();
            var acceleration = desiredMove * _Acceleration - _Velocity * _Friction;

            _Velocity = Vector2.ClampMagnitude( _Velocity + acceleration * Time.fixedDeltaTime, _MaxSpeed);
            _Speed = _Velocity.magnitude;
            if(_Speed < _Epsilon) { _Velocity = Vector2.zero; }

            _CollisionFlags = _CharacterController.Move(new Vector3(_Velocity.x, 0f, _Velocity.y)*Time.fixedDeltaTime);
            _MouseLook.UpdateCursorLock();
        }


        private Vector2 GetInput()
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            // always move along the camera forward as it is the direction that it being aimed at
            var desiredMove = (transform.forward * vertical + transform.right * horizontal);

            return new Vector2(desiredMove.x, desiredMove.z).normalized;
        }


        private void RotateView()
        {
            _MouseLook.LookRotation (transform, _Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
