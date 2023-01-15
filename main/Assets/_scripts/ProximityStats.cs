using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Foundry
{
    public class ProximityStats : MonoBehaviour
    {
        PlayerManagers managers;

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
                    var stats = v.GetComponent<PlayerMindfullness>();
                    var mindfullness = stats.mindfullness;
                    // var restlessness = stats.restlessness;

                    // TODO: Do our compute here
                    Debug.Log($"transform {transform}, distance away {distanceAway}");
                    Debug.Log($"mind: {mindfullness}");
                };
            }
        }
    }
}
