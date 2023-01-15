using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

namespace Foundry
{
    public class AnimalStats : MonoBehaviourPunCallbacks, IPunObservable
    {
        public float deteriateRate = 0.01f;
        private float mindfullness = 0.5f;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(mindfullness);
            }
            else
            {
                mindfullness = (float)stream.ReceiveNext();
            }
        }

        public void OnAnimalHit(float mindfullness){
            this.mindfullness = mindfullness;
        }

        // Start is called before the first frame update
        void Start()
        {
            this.photonView.ObservedComponents.Add(this);
        }

        // Update is called once per frame
        void Update()
        {
            // do something ...
            if (this.mindfullness > 0.5){
                this.mindfullness -= this.deteriateRate * Time.deltaTime;
            } else {
                this.mindfullness += this.deteriateRate * Time.deltaTime;
            }
        }
    }
}
