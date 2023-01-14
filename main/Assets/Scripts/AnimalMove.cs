using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Foundry
{
    public class AnimalMove : MonoBehaviour
    {
        public GameObject animal;
        public int movingSpeed;
        public int radius;

        private Vector3 start;

        private float speed;
        //private float xDirection = 1f;
        //private float zDirection = 1f;

        // Start is called before the first frame update
        void Start()
        {
            //print("(" + transform.position.x + "," + transform.position.y + "," + transform.position.z + ")");
            start = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            loop();
        }

        void loop()
        {
            var currentDistance = Vector3.Distance(start, transform.position);

            if (currentDistance > radius)
            {
                //transform.
                this.transform.Rotate(0.0f, UnityEngine.Random.Range(30, 120), 0.0f);
            }
            transform.position += transform.forward * movingSpeed * Time.deltaTime;
            //transform.position = new Vector3(xPos, yPos, zPos);
        }
    }
}
