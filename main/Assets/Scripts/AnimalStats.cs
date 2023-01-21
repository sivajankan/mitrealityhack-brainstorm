using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

namespace Foundry
{
    public class AnimalStats : MonoBehaviourPunCallbacks, IPunObservable
    {
        public float deteriateRate = 0.01f;
        public float mindfullness = 0.5f;
        public AnimalMove move;
        public AudioSource audioSource;
        public Animator animator;

        public bool isSuperMindful = false;

        public void start()
        {
        }

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
            this.move.transition(1.5f, 0f);
        }

        // Start is called before the first frame update
        void Start()
        {
            photonView.ObservedComponents.Add(this);
            photonView.RPC(nameof(HitSound), RpcTarget.All);
        }

        // Update is called once per frame
        void Update()
        {
            // do something ...
            if (this.mindfullness > 0.5)
            {
                this.mindfullness -= this.deteriateRate * Time.deltaTime;

                if (this.mindfullness > 0.8 && !this.isSuperMindful)
                {
                    this.isSuperMindful = true;
                    //this.animator.SetTrigger("ConvertToEars");
                }
                else if (this.mindfullness < 0.8 && this.isSuperMindful)
                {
                    this.isSuperMindful = false;
                    //this.animator.SetTrigger("ConvertToBunny");
                }
            }
            else
            {
                this.mindfullness += this.deteriateRate * Time.deltaTime;
            }
        }

        [PunRPC]
        public void HitSound()
        {
            this.audioSource.Play();
        }
    }
}
