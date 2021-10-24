using Mirror;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
         * MOBILE INPUT - onscreen buttons for trap setting.
         */

        [SerializeField] private GameObject trapPrefab;

        [SerializeField] public int trapCount = 0;
        public uint netID = 0;


        public override void OnStartClient()
        {
	        PlayerMotor motor = gameObject.GetComponent<PlayerMotor>();
	        motor.enabled = isLocalPlayer;

	        playerCamera = FindObjectOfType<Camera>();
	        netID = gameObject.GetComponent<NetworkIdentity>().netId;

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
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        /// <summary>
        /// The command for the Drop trap function
        /// </summary>
        [Command]
        public void CmdDropTrap()
        {
	        DropTrap();
        }

        
        
        // Update is called once per frame
        void Update()
        {
	        if(isLocalPlayer)
	        {
		        playerCamera.transform.position = transform.position + camOffset;
		        playerCamera.transform.LookAt(transform.position);
		        
		        if(Input.GetKeyDown(KeyCode.Space))
		        {
			        CmdDropTrap();
		        }
	        }
        }
    }
}