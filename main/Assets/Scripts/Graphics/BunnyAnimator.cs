using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundry
{
    public class BunnyAnimator : MonoBehaviour
    {
        [SerializeField]
        private GameObject bunnyEarOne;
        [SerializeField]
        private GameObject bunnyEarTwo;

        [SerializeField] private float currentEarBlend;
        [SerializeField] private float targetEarBlend;
        [SerializeField] private float earBlendSpeed = 0.1f;
        private int earBlendIndex = 0;

        SkinnedMeshRenderer bunnyEarOneRenderer;
        SkinnedMeshRenderer bunnyEarTwoRenderer;
        // Start is called before the first frame update
        void Start()
        {
            bunnyEarOneRenderer = bunnyEarOne.GetComponent<SkinnedMeshRenderer>();
            bunnyEarTwoRenderer = bunnyEarTwo.GetComponent<SkinnedMeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            //if the current blend is not equal to the target blend update blend
            if (currentEarBlend != targetEarBlend)
            {
                //if the current blend is less than the target blend
                if (currentEarBlend < targetEarBlend)
                {
                    //increase the current blend
                    currentEarBlend += earBlendSpeed * Time.deltaTime;
                    //if the current blend is greater than the target blend
                }
                else if (currentEarBlend > targetEarBlend)
                {
                    LerpBunnyEars(earBlendIndex, currentEarBlend, targetEarBlend);
                }
            }
        }

        //This function starts the animation controller transition when when "ConvertToEars" trigger is called
        public void ConvertToEars()
        {
            GetComponent<Animator>().SetTrigger("ConvertToEars");
            //Change bunny ears blend shape index 1 to 100%
            bunnyEarOneRenderer.SetBlendShapeWeight(1, 100);
            earBlendIndex = 0;

        }

        //This function starts the animation controller transition when when "ConvertToBunny" trigger is called
        public void ConvertToBunny()
        {
            GetComponent<Animator>().SetTrigger("ConvertToBunny");
            earBlendIndex = 1;
        }

        //lerp the blend shape weight of the bunny ears
        public void LerpBunnyEars(int index, float curBlend, float targetBlend)
        {
            currentEarBlend = Mathf.Lerp(curBlend, targetBlend, Time.deltaTime * earBlendSpeed);
            bunnyEarOneRenderer.SetBlendShapeWeight(index, currentEarBlend);
            bunnyEarTwoRenderer.SetBlendShapeWeight(index, currentEarBlend);
        }
    }
}
