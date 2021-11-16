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
        [Header("Trap Variables")]
        [Tooltip("This is the ID of the player setting the trap. Gets set when they spawn the trap.")] 
        public uint trapID;
        [Tooltip("If false the trap can be picked up into inventory, is true when player sets the trap.")]
        public bool isSet;
        [SerializeField, Tooltip("The amount of time in seconds the player is disabled when hitting the trap.")] 
        private float playerDisableTime = 5;

        public MeshRenderer rend;
        [SerializeField] private MeshRenderer proximityRend;
        private SphereCollider trapCollider;
        public GameObject trapLight;

        /// <summary>
        /// This is the Ontrigger for the trap and is running on the server.
        /// </summary>
        /// <param name="_collider">The collider of the player that hit the trap.</param>
        [Server]
        private void OnTriggerEnter(Collider _collider)
        {
            
            if(_collider.CompareTag("Player"))
            {
                // Check for the player object so no errors from wall or floor colliders
                if(isSet)
                {
                    // Gets the player ID then checks it to the traps ID
                    uint id = _collider.gameObject.GetComponent<NetworkIdentity>().netId;
                    if(id != trapID)
                    {
                        PlayerMotor motor = _collider.GetComponent<PlayerMotor>();
                        if(motor != null)
                        {
                            RpcHitTrap();

                            NetworkIdentity identity = _collider.GetComponent<NetworkIdentity>();

                            TargetDisablePlayer(identity.connectionToClient, _collider.gameObject);
                        }
                    }
                }
                else
                {
                    RpcPickUpTrap(_collider.gameObject);
                }
            }
            
        }

        /// <summary>
        /// Adds a trap to the player trap count.
        /// </summary>
        /// <param name="_player"></param>
        [ClientRpc]
        public void RpcPickUpTrap(GameObject _player)
        {
            PlayerInteract interact = _player.GetComponent<PlayerInteract>();
            interact.trapCount += 1;
            NetworkServer.Destroy(gameObject);
        }


        /// <summary>
        /// This disables the traps collider and mesh renderer so that it can't be hit while it is disabling
        /// the PlayerMotor of the hit player.
        /// </summary>
        [ClientRpc]
        public void RpcHitTrap()
        {
            //todo trap animation here instead of turning off the renderer
            if(rend != null)
                rend.enabled = false;
            if(proximityRend != null)
                proximityRend.enabled = false;
            if(trapCollider != null)
                trapCollider.enabled = false;
        }

        
        /// <summary>
        /// This calls the coroutine that disables the PlayerMotor of the player that hits the trap.
        /// </summary>
        /// <param name="_target">The NetworkConnection of the player</param>
        /// <param name="_player">The player game object</param>
        [TargetRpc]
        public void TargetDisablePlayer(NetworkConnection _target, GameObject _player)
        {
            PlayerMotor motor = _target.identity.gameObject.GetComponent<PlayerMotor>();
            PlayerInteract interact = _target.identity.GetComponent<PlayerInteract>();
            interact.trapAudio.Play();
            if(interact.artifact != null)
            {
                interact.CmdGetDropLocation();
                interact.CmdDropArtifact(interact.artifact.gameObject);
            }
            
            // todo Drop the artifact if holding it.
            
            StartCoroutine(DisablePlayer(motor, interact));

        }

        

        /// <summary>
        /// Turns off and on the passed in PlayerMotor when a player hits a trap.
        /// </summary>
        /// <param name="_motor"> The PlayerMotor to enable.</param>
        /// <param name="_interact"> The PlayerInteract of the hit player</param>
        private IEnumerator DisablePlayer(PlayerMotor _motor, PlayerInteract _interact)
        {
            _motor.StopPlayer();
            _motor.enabled = false;
            yield return new WaitForSeconds(playerDisableTime);
            _interact.EnableMotor();
            NetworkServer.Destroy(gameObject);
        }

        
        // Start is called before the first frame update
        void Start()
        {
            rend = GetComponent<MeshRenderer>();
            trapCollider = GetComponent<SphereCollider>();
        }

        public override void OnStartServer()
        {
            Start();
        }
    }
}