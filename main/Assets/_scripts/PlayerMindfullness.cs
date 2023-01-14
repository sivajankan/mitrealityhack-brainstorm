using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Foundry
{
    public class PlayerMindfullness : MonoBehaviourPunCallbacks, IPunObservable
    {

        // FOR DEBUG PURPOSE
        public InputActionReference incrementButton;
        public InputActionReference decrementButton;
        public float deltaRange = 0.1f;
        public UnityEvent<string> onValueChange;

        public float mindfullness = 0f;

        void Start()
        {
            incrementButton.action.performed += Increment;
            decrementButton.action.performed += Decrement;
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

        public void Increment(InputAction.CallbackContext ctx)
        {
            Debug.Log("increase");
            var val = mindfullness + deltaRange;
      
            mindfullness = val;
            onValueChange.Invoke(mindfullness.ToString());
        }

        public void Decrement(InputAction.CallbackContext ctx)
        {
            Debug.Log("increase");
            var val = mindfullness - deltaRange;

            mindfullness = val;
            onValueChange.Invoke(mindfullness.ToString());
        }
    }
}