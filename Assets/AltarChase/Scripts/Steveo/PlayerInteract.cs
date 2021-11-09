using AltarChase.Networking;

using Mirror;

using NetworkGame.Networking;

using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        [SerializeField] private TMP_Text trapCountHud;
        public uint netID = 0;

        [SerializeField] private Light playerLight;
        private bool isLightOn = true;

        [SerializeField] private GameObject speedBoost;
        private PlayerInput playerInput;

        public bool isHoldingArtifact;
        [SerializeField] public Transform itemLocation;
        [SerializeField] public Transform itemDropLocation = null;
        [SerializeField] public Transform itemDropLocationBack;
        [SerializeField] public Transform itemDropLocationForward;
        [SerializeField] public Transform itemDropLocationRight;
        [SerializeField] public Transform itemDropLocationLeft;
        [SerializeField] public Transform rayOrigin;
        [SerializeField] public Artifact artifact = null;

        [SerializeField] private GameObject artifactTest;

        [SerializeField] public float timeHeldArtifact = 0;

        [SerializeField] public int modelIndex = 0;
        [SerializeField] private List<GameObject> models = new List<GameObject>();
        [SerializeField] public string characterName = null;



        public override void OnStartClient()
        {
	        PlayerMotor motor = gameObject.GetComponent<PlayerMotor>();
	        motor.enabled = isLocalPlayer;
	        
	        CustomNetworkManager.AddPlayer(this);

	        playerInput = GetComponent<PlayerInput>();
	        playerCamera = FindObjectOfType<Camera>();
	        netID = gameObject.GetComponent<NetworkIdentity>().netId;
	        trapCountHud = FindObjectOfType<TMP_Text>();

        }

        public override void OnStartLocalPlayer()
        {
	        SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Additive);
	        
	        CmdAssignAuthority();
        }

        public void EnableMotor()
        {
	        PlayerMotor motor = gameObject.GetComponent<PlayerMotor>();
	        motor.enabled = isLocalPlayer;
        }

        [Command]
        public void CmdCharacterName(string _name) => RpcCharacterName(_name);

        [ClientRpc]
        public void RpcCharacterName(string _name)
        {
	        CharacterName(_name);
        }

        public void CharacterName(string _name)
        {
	        characterName = _name;
	        gameObject.name = _name;
        }

        public void ChangeModel(int _index)
        {
	        foreach(GameObject model in models)
	        {
		        model.SetActive(false);
	        }
	        models[_index].SetActive(true);
        }

        [Command]
        public void CmdChangeModel(int _index)
        {
	        RpcChangeModel(_index);
        }

        [ClientRpc]
        public void RpcChangeModel(int _index)
        {
	        ChangeModel(_index);

        }

        [Command]
        public void CmdAssignAuthority()
        {
	        MatchManager.instance.netIdentity.AssignClientAuthority(connectionToClient);
        }

		public void GetArtifact(GameObject _artifact)
        {
	        isHoldingArtifact = true;
	        artifact = _artifact.GetComponent<Artifact>();
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
        public void CmdDropArtifact(GameObject _artifact)
        {
	        
	        artifact = _artifact.GetComponent<Artifact>();
	        artifact.RpcDropItem(this.gameObject);
	        Debug.Log(itemDropLocation);
        }

        [Command]
        public void CmdGetDropLocation() => RpcGetDropLocation();

        [ClientRpc]
        public void RpcGetDropLocation() => GetDropLocation();

        
        public void GetDropLocation()
        {
	        Debug.DrawRay(rayOrigin.position, transform.forward * 1.23f, Color.cyan, 6);
	        if(Physics.Raycast(rayOrigin.position, transform.forward, 1.23f))
	        {
		        if(Physics.Raycast(rayOrigin.position, -transform.forward, 1.23f))
		        {
			        itemDropLocation = itemDropLocationLeft;
			        return;
		        }
		        
		        itemDropLocation = itemDropLocationBack;
		        return;
	        }

	        if(Physics.Raycast(rayOrigin.position, -transform.forward, 1.23f))
	        {
		        itemDropLocation = itemDropLocationForward;
		        return;
	        }
	        
	        // if(!Physics.Raycast(transform.position, -transform.forward,3f,6))
		       //  return itemDropLocationForward;
	        // if(Physics.Raycast(rayOrigin.position, -transform.forward,1.2f) && Physics.Raycast(rayOrigin.position, transform.forward,1.2f))
		       //   return itemDropLocationLeft;
	        // if(!Physics.Raycast(transform.position, -transform.right,3f,6))
		       //  return itemDropLocationRight;
	        itemDropLocation = itemDropLocationForward;
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

        public override void OnStopClient()
        {
	        CustomNetworkManager.RemovePlayer(this);
        }

        [Command]
        public void CmdDropSpeed() => DropSpeed();
        
        [Server]
        public void DropSpeed()
        { 
	        GameObject speed = Instantiate(speedBoost);
	        NetworkServer.Spawn(speed);
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
			        if(trapCount > 0)
			        {
				        trapCount -= 1;
						CmdDropTrap();
			        }
		        }
		        
		        if(Input.GetKeyDown(KeyCode.E) || (playerInput.actions["Light"].triggered))
		        {
			        CmdTurnOffLight();
		        }
		        
		        if(Input.GetKeyDown(KeyCode.Y))
		        {
			        CmdDropSpeed();
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
				        CmdGetDropLocation();
						CmdDropArtifact(artifact.gameObject);
				        
			        }
		        }

		        if(isHoldingArtifact)
			        timeHeldArtifact += Time.deltaTime;

		        if(trapCountHud != null)
			        trapCountHud.text = trapCount.ToString();
	        }
        }
    }
}