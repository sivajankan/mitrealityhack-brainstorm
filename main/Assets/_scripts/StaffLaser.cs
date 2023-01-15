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
        }
    }
}
