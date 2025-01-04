using System;
using System.Collections;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Demo.Camera
{
    public class FollowCamera : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float cameraSpeed = 5f;
        
        [Header("Position")]
        [SerializeField] private Vector3 distanceOffset = Vector3.zero;

        [Header("Zoom")]
        [SerializeField] private float minimumFOV = 60f;
        [SerializeField] private float edgeOffset = 10f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float zoomStoppingOffset = 1f;

        [SerializeField] private Transform[] targets;
        private Coroutine camCoroutine;
        private UnityEngine.Camera Camera;

        private void Awake()
        {
            this.Camera = this.GetComponent<UnityEngine.Camera>();
        }

        public virtual void StartCameraMovement()
        {
            this.StopCameraMovement();

            this.camCoroutine = StartCoroutine(nameof(c_CameraCoroutine));
        }

        public virtual void StopCameraMovement()
        {
            if (camCoroutine != null)
            {
                StopCoroutine(camCoroutine);
                camCoroutine = null;
            }
        }

        public virtual void SetTarget(Transform target)
        {
            this.SetTargets(new Transform[] { target });
        }

        public virtual void SetTargets(Transform[] targets)
        {
            this.targets = targets;

            this.StartCameraMovement();
        }

        private IEnumerator c_CameraCoroutine()
        {
            if(!targets.Any())
            {
                camCoroutine = null;
                yield break;
            }

            do
            {
                yield return new WaitForFixedUpdate();

                transform.position = this.MoveCamera(out Vector3 targetPosition);

                Camera.fieldOfView = this.ZoomCamera(targetPosition);
            }
            while (true);
        }

        private Vector3 MoveCamera(out Vector3 targetPosition)
        {
            targetPosition = this.CalculateTargetCentre();
            Vector3 realPosition = targetPosition + distanceOffset;

            float cameraDistance = cameraSpeed * Time.fixedDeltaTime;

            return Vector3.MoveTowards(transform.position, realPosition, cameraDistance);
        }

        private float ZoomCamera(Vector3 targetPosition)
        {
            float fullZoom = this.CalculateFOV(targetPosition);
            fullZoom = Mathf.Max(fullZoom + edgeOffset, minimumFOV);

            float totalDeltaZoom = fullZoom - this.Camera.fieldOfView;

            if(totalDeltaZoom < zoomStoppingOffset)
            {
                return fullZoom;
            }

            return this.Camera.fieldOfView + ((1/totalDeltaZoom) * zoomSpeed * Time.fixedDeltaTime);
        }

        private Vector3 CalculateTargetCentre()
        {
            int numTargets = targets.Length;
            if(numTargets <= 0)
            {
                return gameObject.transform.position;
            }

            if(numTargets == 1)
            {
                return targets[0].position;
            }

            var bound = new Bounds();
            foreach(var target in targets)
            {
                bound.Encapsulate(target.position);
            }
            return bound.center;
        }

        private float CalculateFOV(Vector3 targetPosition)
        {
            int numTargets = targets.Length;

            if( numTargets <= 1)
            {
                return minimumFOV;
            }

            Vector3 maxDistanceTargetPosition = Vector3.zero;
            float maxDistanceTargetFloat = 0;
            foreach (var target in this.targets)
            {
                float dist = Vector3.Distance(targetPosition, target.position);
                if (dist > maxDistanceTargetFloat)
                {
                    maxDistanceTargetPosition = target.position;
                    maxDistanceTargetFloat = dist;
                }
            }

            Vector3 targetLocalPos = transform.InverseTransformPoint(maxDistanceTargetPosition);
            Vector3 posToTarget = targetLocalPos - targetPosition;

            return Mathf.Max(Mathf.Abs(posToTarget.x), Mathf.Abs(posToTarget.y));
        }
    }
}
