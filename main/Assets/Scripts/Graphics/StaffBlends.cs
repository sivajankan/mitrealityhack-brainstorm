using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundry
{
    public class StaffBlends : MonoBehaviour
    {
        SkinnedMeshRenderer _skinnedMeshRenderer;
        private int blendShapeIndex;
        public float brainInput;
        public float blendSpeed;

        private float brainInputRemaped;

        // Start is called before the first frame update
        void Start()
        {
            _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            AdjustBlendIndex(brainInput);
            RemapBrainInput(brainInput);
            LerpBlend(brainInputRemaped, blendSpeed, blendShapeIndex);
        }

        //This function will lerp the blend shape of the staff to the desired value
        public void LerpBlend(float blendValue, float blendSpeed, int blendIndex)
        {
            //Get the current blend value
            float currentBlend = _skinnedMeshRenderer.GetBlendShapeWeight(blendIndex);
            //Lerp the blend value
            float newBlend = Mathf.Lerp(currentBlend, blendValue, blendSpeed * Time.deltaTime);
            //Set the new blend value
            _skinnedMeshRenderer.SetBlendShapeWeight(blendIndex, newBlend);
        }

        //This function will set the blend shape index the staff to the desired value
        public void SetBlendIndex(int index)
        {
            blendShapeIndex = index;
        }

        public void SetBrainInput(float input)
        {
            this.brainInput = input;
        }

        //Adjust blend shape value based on float input value (0-1) where .5 is the default value
        public void AdjustBlendIndex(float input)
        {
            //if the float input is less than .5 blend index is 1
            if (input < .5f)
            {
                SetBlendIndex(0);
                _skinnedMeshRenderer.SetBlendShapeWeight(1, 0);
            }
            //if the float input is greater than .5 blend index is 0
            else if (input > .5f)
            {
                SetBlendIndex(1);
                _skinnedMeshRenderer.SetBlendShapeWeight(0, 0);
            }

        }

        //This function will remap the brainInput float to a value between 0 and 100
        public void RemapBrainInput(float input)
        {
            //brainInputRemaped = input * 100f;

            //If the input is bigger than 0.5 then you want 0 to 100 based off of 1-0.5
            if (input > .5f)
            {
                brainInputRemaped = (input - .5f) * 200f;
            }

            //If the input is less than 0.5 then you want 0 to 100 based off of 0.5-0

            if (input <= .5f)
            {
                brainInputRemaped = (.5f - input) * 200f;
            }

        }

    }
}