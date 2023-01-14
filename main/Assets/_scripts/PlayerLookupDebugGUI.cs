using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

namespace Foundry
{
    public class PlayerLookupDebugGUI : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameManager gameManager;
        public TextMeshProUGUI gui;

        private float timerRange = 1f;
        private float timePass = 0f;

        // Update is called once per frame
        void Update()
        {
            timePass += Time.deltaTime;

            if (timePass > timerRange)
            {
                var players = GameObject.FindGameObjectsWithTag("Player");
                var text = players.Aggregate("", (acc, cur) => {
                    var mind = cur.GetComponent<PlayerMindfullness>();
                    acc += $"mindfullness: {mind.mindfullness} \n";
                    return acc;
                });
                gui.text = text;
            }
        }
    }
}
