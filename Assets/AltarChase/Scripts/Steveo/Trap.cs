using AltarChase.Player;

using Mirror;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

namespace AltarChase
{
    /// <summary>
    /// This class handles the functionality of the trap object.
    /// </summary>
    public class Trap : NetworkBehaviour
    {
        public uint trapID = 0;
        public bool isSet = false;
        [SerializeField] private float playerDisableTime = 5;

        private MeshRenderer rend;
        private SphereCollider trapCollider;

        [Server]
        private void OnTriggerEnter(Collider _collider)
        {
            
            if(isSet)
            {
                uint id = _collider.gameObject.GetComponent<NetworkIdentity>().netId;
                if(id != trapID) // network ID?? instead?
                {
                    //todo play trap animation.
                    
                    PlayerMotor motor = _collider.GetComponent<PlayerMotor>();
                    PlayerInteract interact = _collider.GetComponent<PlayerInteract>();
                    if(motor != null)
                    {
                        RpcHitTrap();
                        // rend.enabled = false;
                        // trapCollider.enabled = false;
                        StartCoroutine(DisablePlayer(motor, interact));
                    }
                }
            }
        }


        
        [ClientRpc]
        public void RpcHitTrap()
        {
            rend.enabled = false;
            trapCollider.enabled = false;
            //StartCoroutine(DisablePlayer(_motor));
        }

        // todo might need to use a target rpc here to call the diasable on the specific client.
        
        /// <summary>
        /// Turns on the passed in PlayerMotor allowing the player to move again.
        /// </summary>
        /// <param name="_motor"> The PlayerMotor to enable.</param>
        private IEnumerator DisablePlayer(PlayerMotor _motor, PlayerInteract _interact)
        {
            _motor.enabled = false;
            yield return new WaitForSeconds(playerDisableTime);
            _interact.OnStartClient();
            NetworkServer.Destroy(gameObject);
            //Destroy(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            rend = GetComponent<MeshRenderer>();
            trapCollider = GetComponent<SphereCollider>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}