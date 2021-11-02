using Mirror;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AltarChase.Player
{
    /// <summary>
    /// This class handles all the player interactions with environment.
    /// </summary>
    public class PlayerInteract : NetworkBehaviour
    {
	    private Camera playerCamera;
	    [SerializeField] private Vector3 camOffset;
	    
        /* 
         * trap number & update HUD 
         * pick up artifact
         * is holding artifact
         * drop artifact
         * WIN
         * LOOSE ??
         */

        [SerializeField] private GameObject trapPrefab;

        [SerializeField] public int trapCount = 0;
        public uint netID = 0;

        [SerializeField] private Light playerLight;
        private bool isLightOn = true;

        [SerializeField] private GameObject speedBoost;
        private PlayerInput playerInput;

        public bool isHoldingArtifact;
        [SerializeField] public Transform itemLocation;
        [SerializeField] public Transform itemDropLocation;
        [SerializeField] public Artifact artifact = null;

        [SerializeField] private GameObject artifactTest;


        public override void OnStartClient()
        {
	        PlayerMotor motor = gameObject.GetComponent<PlayerMotor>();
	        motor.enabled = isLocalPlayer;

	        playerInput = GetComponent<PlayerInput>();
	        playerCamera = FindObjectOfType<Camera>();
	        netID = gameObject.GetComponent<NetworkIdentity>().netId;

        }

        
        public void GetArtifact(Artifact _artifact)
        {
	        isHoldingArtifact = true;
	        artifact = _artifact;
        }

        [ClientRpc]
        public void RpcDropArtifact()
        {
	        if(artifact != null)
	        {
		        artifact.gameObject.transform.parent = null;

		        artifact.gameObject.transform.position = itemDropLocation.position;
		        artifact.isHeld = false;
		        artifact = null;
	        }
        }

        [Command]
        public void CmdDropArtifact()
        {
	        
		        Debug.Log("should be dropping the artifact");
				artifact.RpcDropItem(this);
				
		        
	        
        }


        /// <summary>
        /// The command for the Drop trap function
        /// </summary>
        [Command]
        public void CmdDropTrap()
        {
	        DropTrap();
        }

        /// <summary>
        /// Function for dropping traps.
        /// </summary>
        [Server] // Only runs on the server.
        private void DropTrap()
        {
	        // Calculate the distance to the ground from the player character.
	        float dist = 0;
	        Ray ray = new Ray(transform.position, Vector3.down);
	        RaycastHit hit;
	        if(Physics.Raycast(ray, out hit, 5f))
	        {
		        dist = hit.distance;
	        }

	        
	        if(trapCount > 0)
	        {
		        // Use the calculated distance to set the position for the trap.
		        Vector3 position = new Vector3(transform.position.x, transform.position.y - dist, transform.position.z);
		        GameObject droppedTrap = Instantiate(trapPrefab, position, Quaternion.identity);
		        Trap trap = droppedTrap.GetComponent<Trap>();
		        // Give trap the ID of the player and set it.
		        trap.trapID = netID;
		        trap.isSet = true;
		        // Minus 1 from the trap count.
		        trapCount -= 1;
		        NetworkServer.Spawn(droppedTrap);
	        }
	        else
	        {
		        Debug.Log("No Traps left.");
		        // todo UI feedback, no traps. Will need to be called in an RPC.
	        }
	        
	         
        }

        /// <summary>
        /// The command for the players lights on and off.
        /// </summary>
        [Command]
        public void CmdTurnOffLight()
        {
	        RpcTurnOffLight();
        }
        
        /// <summary>
        /// The RPC for turning on and off the players lights
        /// </summary>
        [ClientRpc]
        private void RpcTurnOffLight()
        {
	        isLightOn = !isLightOn;
	        playerLight.enabled = isLightOn;
        }

        

        
        
        // Update is called once per frame
        void Update()
        {
	        if(isLocalPlayer)
	        {
		        playerCamera.transform.position = transform.position + camOffset;
		        playerCamera.transform.LookAt(transform.position);
		        
		        if(Input.GetKeyDown(KeyCode.Space) || (playerInput.actions["Drop Trap"].triggered))
		        {
			        CmdDropTrap();
		        }
		        
		        if(Input.GetKeyDown(KeyCode.E) || (playerInput.actions["Light"].triggered))
		        {
			        CmdTurnOffLight();
		        }
		        
		        if(Input.GetKeyDown(KeyCode.Y))
		        {
			        GameObject speed = Instantiate(speedBoost);
			        NetworkServer.Spawn(speed);
		        }
		        if(Input.GetKeyDown(KeyCode.Q))
		        {
			        GameObject art = Instantiate(artifactTest);
			        NetworkServer.Spawn(art);
		        }
		        if(Input.GetKeyDown(KeyCode.P))
		        {
			        if(artifact != null)
			        {
						CmdDropArtifact();
				        
			        }
		        }
		        
	        }
        }
    }
}