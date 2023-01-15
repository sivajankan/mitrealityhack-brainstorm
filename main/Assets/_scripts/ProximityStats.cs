using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Photon.Pun;

namespace Foundry
{
    public class ProximityStats : MonoBehaviourPunCallbacks, IPunObservable
    {
        PlayerManagers managers;
        public float mindfullness = 0.5f;
        public UnityEvent<float> onMindfulnessChange;
        public float dropOffStart = 3f;
        public float dropOffEnd = 10f;

        // Start is called before the first frame update
        void Start()
        {
            managers = PlayerManagers.Instance;
        }

        void Update()
        {
            if (managers.players?.Count > 0)
            {
                // so here we are getting the distance between player and animals
                foreach (var v in managers.players.Values.ToArray())
                {
                    var playerPos = v.transform.position;
                    var myPos = transform.position;

                    playerPos.y = 0;
                    myPos.y = 0;
                    var distanceAway = Vector3.Distance(playerPos, myPos);

                    if (distanceAway > this.dropOffEnd) return; 
                    
                    var stats = v.GetComponent<PlayerMindfullness>();
                    mindfullness = stats.mindfullness
                        * (this.dropOffEnd - distanceAway) / (this.dropOffEnd - dropOffStart);

                    // TODO: Do our compute here
                    onMindfulnessChange.Invoke(mindfullness);

                    Debug.Log($"transform {transform}, distance away {distanceAway}");
                    Debug.Log($"mind: {mindfullness}");
                };
            }
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
    }
}
