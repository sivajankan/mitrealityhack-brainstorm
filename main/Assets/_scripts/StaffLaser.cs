using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Foundry
{
    public class StaffLaser : MonoBehaviour
    {
        public InputAction inputAction;
        public float activationValue;
        public Transform firingPoint;

        void Start()
        {
            // SHOOT LASERS!
            inputAction.Enable();
        }

        // Update is called once per frame
        void Update()
        {
            activationValue = inputAction.ReadValue<float>();

            if (activationValue > 0.5) { Fire(); }
        }

        public void Fire()
        {
            Debug.Log("FIRE!");
            var ray = new Ray(transform.position, firingPoint.up);
            RaycastHit hitData;
            if (Physics.Raycast(ray, out hitData))
            {
                // 80 X // 0.2 Z

            }
        }
    }
}
