using System;
using Unity.Mathematics;
using UnityEngine;

namespace NajakBoi.Scripts
{
    public class SmoothCamera : MonoBehaviour {

        public static Transform Target;
        public float smoothSpeed = 0.125f;
        public Vector3 offset;
        public static Camera Camera;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
        }

        void FixedUpdate ()
        {
            if (Target == null)
                return;

            var t = transform;
            var desiredPosition = Target.position + offset;
            var smoothedPosition = Vector3.Lerp(t.position, desiredPosition, smoothSpeed);
            
            t.position = smoothedPosition;
            t.LookAt(Target);
            t.rotation = quaternion.identity;
        }

    }
}