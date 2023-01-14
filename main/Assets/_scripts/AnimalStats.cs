using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Foundry
{
    public class AnimalStats : MonoBehaviour
    {
        // what do we do here? we need a global singleton
        PlayerManagers managers;

        // Start is called before the first frame update
        void Start()
        {
            managers = PlayerManagers.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (managers.players?.Count > 0)
            {
                // so here we are getting the distance between player and animals
                foreach (var v in managers.players.Values.ToArray())
                {
                    var playerPos = v.transform.position;
                    var animalPos = transform.position;

                    playerPos.y = 0;
                    animalPos.y = 0;
                    var distanceAway = Vector3.Distance(playerPos, animalPos);
                    var stats = v.GetComponent<PlayerMindfullness>();
                    var mindfullness = stats.mindfullness;
                    var restlessness = stats.restlessness;

                    // TODO: Do our compute here
                    Debug.Log($"transform {transform}, distance away {distanceAway}");
                    Debug.Log($"mind: {mindfullness} | rest: {restlessness}");
                };
            }
        }
    }
}
