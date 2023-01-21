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

        private float degradeSpeed = 0.01f;


        // Start is called before the first frame update
        void Start()
        {
            managers = PlayerManagers.Instance;
        }

        void Update()
        {
            if (managers.players?.Count > 0)
            {
                var PlayerMindfulnessInArea = managers.players.Values
                    .Select(CalculateMindfulness)
                    .Where(v => v > 0);

                // Maybe good to setup a enum state here...
                if (PlayerMindfulnessInArea.Any())
                {
                    mindfullness = PlayerMindfulnessInArea.Average();
                }
                else
                {
                    // degrade mindfulness
                    mindfullness -= degradeSpeed * Time.deltaTime;
                }

                onMindfulnessChange.Invoke(mindfullness);
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

        private float CalculateMindfulness(GameObject v)
        {
            var playerPos = v.transform.position;
            var myPos = transform.position;
            playerPos.y = 0;
            myPos.y = 0;
            var distanceAway = Vector3.Distance(playerPos, myPos);
            var stats = v.GetComponent<PlayerMindfullness>();

            if (distanceAway > dropOffEnd) return -1;

            Debug.Log($"transform {transform}, distance away {distanceAway}");
            Debug.Log($"mind: {mindfullness}");

            return stats.mindfullness
                * (this.dropOffEnd - distanceAway) / (this.dropOffEnd - dropOffStart);
        }
    }
}
