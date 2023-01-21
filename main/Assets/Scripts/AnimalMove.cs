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

        public float poll = 1f;
        private float timePass = 0f;

        public bool isRun = true;

        // Start is called before the first frame update
        void Start()
        {
            //print("(" + transform.position.x + "," + transform.position.y + "," + transform.position.z + ")");
            start = transform.position;
            poll = UnityEngine.Random.Range(0f, 6f);
        }

        // Update is called once per frame
        void Update()
        {
            loop();
        }

        public void transition(float poll, float timePass)
        {
            this.isRun = false;
            this.poll = poll;
            this.timePass = timePass;
        }

        void loop()
        {
            // reason why animal is going crazy:
            // many client is doing the same calculation,
            // so animal end up rotating very fast
            timePass += Time.deltaTime;
            if (timePass > poll)
            {
                isRun = !isRun;
                timePass = 0;
                poll = UnityEngine.Random.Range(1f, 6f);
            }
            if (isRun)
            {  
                var currentDistance = Vector3.Distance(start, transform.position);

                if (currentDistance > radius)
                {
                    transform.Rotate(0.0f, UnityEngine.Random.Range(90, 270), 0.0f);
                }
                transform.position += transform.forward * movingSpeed * Time.deltaTime;
            } else {
                
            }
        }
    }
}
