using System;
using System.Collections;
using UnityEngine;

namespace Demo.Entity
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement3D : MonoBehaviour
    {
        private Vector2 moveInput;

        private Coroutine movementCoroutine;
        private bool isMoving = false;

        //components
        private Rigidbody rb;

        [Header("Movement")]

        [SerializeField][Min(0f)]
        private float maxSpeed = 5;
        public float MaxSpeed => maxSpeed;

        [SerializeField][Min(0f)] private float maxAccelerationForce = 2;
        [SerializeField][Min(0f)] private float brakingForce = 5;

        private bool isPaused = false;

        public Action<Vector3> OnPositionChanged;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void TriggerMovementCoroutine(Vector2 inputMovement)
        {
            moveInput = inputMovement;

            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
                movementCoroutine = null;
            }

            if (moveInput.sqrMagnitude > 0 && movementCoroutine == null)
            {
                isMoving = true;
                movementCoroutine = StartCoroutine(c_MovementCoroutine());
            }
        }

        public void TriggerCancelMovement()
        {
            moveInput = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            isMoving = false;

            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
                movementCoroutine = null;
            }
            movementCoroutine = StartCoroutine(c_StoppingCoroutine());
        }

        private IEnumerator c_MovementCoroutine()
        {
            while (isMoving)
            {
                yield return new WaitUntil(() => !isPaused);
                yield return new WaitForFixedUpdate();

                MoveEntity();
            }
        }

        private void MoveEntity()
        {
            Vector3 convertedMoveInput = new Vector3(moveInput.x, 0, moveInput.y);
            //update velocity
            Vector3 maxVelocity = convertedMoveInput.normalized * maxSpeed;
            Vector3 deltaVelocity = maxVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            Vector3 deltaAcceleration = deltaVelocity / Time.fixedDeltaTime;
            deltaAcceleration = Vector3.ClampMagnitude(deltaAcceleration, maxAccelerationForce);

            rb.AddForce(deltaAcceleration, ForceMode.Force);

            OnPositionChanged?.Invoke(transform.position);
        }

        private IEnumerator c_StoppingCoroutine()
        {
            while (rb.linearVelocity.sqrMagnitude >= 1)
            {
                yield return new WaitUntil(() => !isPaused);
                yield return new WaitForFixedUpdate();

                Vector3 deltaVelocity = -new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

                Vector3 deltaAcceleration = deltaVelocity / Time.fixedDeltaTime;
                deltaAcceleration = Vector3.ClampMagnitude(deltaAcceleration, brakingForce);

                rb.AddForce(deltaAcceleration, ForceMode.Force);
            }

            StopMovement();
        }

        private void StopMovement()
        {
            moveInput = Vector3.zero;

            StopAllCoroutines();
            movementCoroutine = null;
            isMoving = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        private Vector3 cachedVelocity = Vector3.zero;
        private Vector3 cachedAngularVelocity = Vector3.zero;

        // public virtual void PauseGame(EPauseType pauseType, bool isPaused)
        // {
        //     this.isPaused = isPaused;
        //
        //     if (pauseType.HasFlag(EPauseType.PauseMotion))
        //     {
        //         //save: velocity, angular velocity, move input
        //         if (isPaused)
        //         {
        //             cachedVelocity = rb.velocity;
        //             cachedAngularVelocity = rb.angularVelocity;
        //         }
        //         else
        //         {
        //             rb.velocity = cachedVelocity;
        //             rb.angularVelocity = cachedAngularVelocity;
        //         }
        //
        //     }
        // }

        public void OnDead()
        {
            StopMovement();
        }

    }
}
