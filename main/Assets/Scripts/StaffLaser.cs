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
        public PlayerMindfullness mindfullness;

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
            if (Physics.SphereCast(ray, 1, out hitData))
            {
                // 80 X // 0.2 Z
                Debug.Log("HIT SOMETHING");
                try
                {
                    hitData.transform.GetComponent<AnimalStats>().OnAnimalHit(mindfullness.mindfullness);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("OH NO!" + e.Message);
                }
            }
        }
    }
}
