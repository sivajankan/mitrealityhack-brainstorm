using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Foundry
{
    public class PlayerManagers : MonoBehaviour
    {
        public static PlayerManagers Instance { get; private set; }
        public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

        [SerializeField]
        private float pollInterval = 1f;
        private float timePass = 0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            timePass += Time.deltaTime;
            if (timePass > pollInterval)
            {
                foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
                {
                    var data = p.GetComponent<PlayerMindfullness>();
                    var key = data?.photonView?.ViewID ?? 0;
                    
                    players[key] = p;
                };
            }
        }
    }
}
