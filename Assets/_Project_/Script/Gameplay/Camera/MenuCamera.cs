using UnityEngine;
using System;
using NaughtyAttributes;


namespace FXnRXn
{
    public class MenuCamera : MonoBehaviour
    {
        #region Properties
        [Header("Bob Settings")]
        [HorizontalLine(color: EColor.Green)]
        public float bobSpeed = 1f;
        public float bobAmount = 0.1f;
        public bool rotate = true;
        public float rotationAmount = 1f;
        
        [Header("Axes")]
        [HorizontalLine(color: EColor.Green)]
        public bool useX = true;
        public bool useY = true;
        public bool useZ = true;

        private Vector3 startPosition;
        private Vector3 startRotation;
        private float timer = 0f;
        #endregion

        #region Unity Callbacks

        private void Start()
        {
            // Store initial position and rotation
            startPosition = transform.localPosition;
            startRotation = transform.localEulerAngles;
        }
        
        void OnDisable()
        {
            transform.localPosition = startPosition;
            transform.localEulerAngles = startRotation;
        }

        private void Update()
        {
            // Calculate bob movement
            timer += Time.deltaTime * bobSpeed;
            float bobOffset = Mathf.Sin(timer) * bobAmount;

            // Apply position bob
            Vector3 newPosition = startPosition;
            if (useX) newPosition.x += bobOffset;
            if (useY) newPosition.y += bobOffset;
            if (useZ) newPosition.z += bobOffset;
        
            transform.localPosition = newPosition;

            // Apply rotation bob if enabled
            if (rotate)
            {
                Vector3 newRotation = startRotation;
                newRotation.z += Mathf.Cos(timer) * rotationAmount;
                transform.localEulerAngles = newRotation;
            }
        }

        #endregion

        #region Custom Method

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
    
    }
}

