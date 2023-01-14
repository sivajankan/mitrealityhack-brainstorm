using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundry
{
    public class AnimalMove : MonoBehaviour
    {
        public GameObject animal;
        public int movingSpeed;
        private float speed;

        // Start is called before the first frame update
        void Start()
        {
            speed = movingSpeed / 1000f;
        }

        // Update is called once per frame
        void Update()
        {
            loop();
        }

        void loop()
        {
            float xPos = transform.position.x + speed;
            float yPos = transform.position.y + speed;
            transform.position = new Vector3(xPos, yPos, transform.position.z);
        }
    }
}
