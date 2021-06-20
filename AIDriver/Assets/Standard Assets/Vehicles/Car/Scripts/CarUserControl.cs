using System;
using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        [SerializeField] private float v;
        [SerializeField] private float h;
        [SerializeField] private int currentNode = 0;
        [SerializeField] private float handbrake;
        [SerializeField] private float test;
        public Transform path;
        private List<Transform> nodes;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }
        private void Start()
        {
            Transform[] pathTransform = path.GetComponentsInChildren<Transform>();
            nodes = new List<Transform>();

            for (int i = 0; i < pathTransform.Length; i++)
            {
                if (pathTransform[i] != path.transform)
                {
                    nodes.Add(pathTransform[i]);
                }
            }
        }

        private void FixedUpdate()
        {
            // pass the input to the car!
            //float h = CrossPlatformInputManager.GetAxis("Horizontal");
            //float v = CrossPlatformInputManager.GetAxis("Vertical");
            //#if !MOBILE_INPUT
            //float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            h = 0f;
            //float v = 10f;
            ApplySteer();
            CheckWaypointDistance();
            test = m_Car.CurrentSpeed;
            if (m_Car.CurrentSpeed > 50)
                v = 0.5f;
            else if (m_Car.CurrentSpeed > 30)
                v = 1 - Math.Abs(h);
            else
                v = 1f;
            handbrake = 0f;
#if !MOBILE_INPUT
            if (m_Car.CurrentSpeed > 50 && Math.Abs(h) > 0.5)
                handbrake = 1f;
            else if (m_Car.CurrentSpeed > 30 && Math.Abs(h) > 0.5)
                handbrake = 0.5f;
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
        private void ApplySteer()
        {
            Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
            h = (relativeVector.x / relativeVector.magnitude);
        }
        private void CheckWaypointDistance()
        {
            if (Vector3.Distance(transform.position, nodes[currentNode].position) < 1f)
            {
                if (currentNode == nodes.Count - 1)
                    currentNode = 0;
                else
                    currentNode++;
            }
        }
    }
}
