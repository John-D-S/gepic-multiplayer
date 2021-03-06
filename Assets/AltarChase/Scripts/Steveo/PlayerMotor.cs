using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AltarChase.Player
{
    /// <summary>
    /// Handles the movement of the player objects.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMotor : MonoBehaviour
    {
        [Header("Player Variables")]
        [SerializeField, Tooltip("Player Speed")] public float speed = 20;
        [SerializeField, Tooltip("Player Original Speed")] public float originalSpeed = 20;
        [SerializeField] private float boostTime = 10;
        private GameObject cameraGameObject;
        private PlayerInput playerInput;

        private Rigidbody rb;
        private float movementVelocity;
        private Vector3 movementDirection;

        private Vector3 lastLookDirection;

        public bool playingOnMobile;

        //todo Add Animator for movement animations.
        // todo MOBILE INPUT

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            cameraGameObject = FindObjectOfType<Camera>().gameObject;
            playerInput = GetComponent<PlayerInput>();
        }

        /// <summary>
        /// Handles Input for the character movement and direction. Called in Update.
        /// </summary>
        private void GetMovementInput()
        {
            movementDirection = Quaternion.Euler(0, cameraGameObject.transform.eulerAngles.y, 0) 
                                * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f)
            {
                movementVelocity = speed;
            }
            else
            {
                movementVelocity = 0;
            }

            if(playingOnMobile)
            {
                Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
                Vector3 move = new Vector3(input.x, 0, input.y);
                if(Mathf.Abs(input.x) + Mathf.Abs(input.y) > 0.1f)
                {
                    movementDirection = move;
                    movementVelocity = speed;
                }
                else
                {
                    movementVelocity = 0;
                }
            }

            //todo Add animation checks into here
        }

        /// <summary>
        /// Moves the character by applying force to the rigidbody. Called in Fixed Update.
        /// </summary>
        private void Move()
        {
            Vector3 travelDirection = rb.velocity;
            float friction = travelDirection.magnitude;
            Vector3 frictionDirection = new Vector3(-travelDirection.x, 0, -travelDirection.z).normalized;
            Vector3 frictionForce = friction * frictionDirection;
            
            Vector3 force = movementDirection * movementVelocity;
            
            rb.AddForce(frictionForce);
            rb.AddForce(force,ForceMode.Force);
            
        }

        /// <summary>
        /// This stops the player when hitting a set trap.
        /// </summary>
        public void StopPlayer()
        {
            rb.velocity = new Vector3(0,0,0);
        }
        
        // Update is called once per frame
        void Update()
        {   
            GetMovementInput();
        }
        
        private void FixedUpdate()
        {
            Move();
            
            // Sets the character facing direction when moving
            if(movementVelocity != 0)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, lastLookDirection,0.25f));
                // Saves the direction to use when stopped.
                lastLookDirection = movementDirection;
            }
        }

        /// <summary>
        /// Calls the coroutine for giving the player a speed boost.
        /// </summary>
        public void CallSpeedBoostCR() => StartCoroutine(SpeedBoost_CR());
        
        /// <summary>
        /// Changes the speed of the player after picking up the speedboost pickup.
        /// </summary>
        public IEnumerator SpeedBoost_CR()
        {
            Debug.Log("cr running");
            speed = 30;
            yield return new WaitForSeconds(boostTime);
            Debug.Log("cr done");
            speed = originalSpeed;
        }

    }
}