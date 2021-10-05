using Mirror;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace AltarChase.Networking
{

	[RequireComponent(typeof(PlayerController))]
	public class NetworkPlayer : NetworkBehaviour
	{
		[SerializeField] private GameObject enemyToSpawn;

		private void Update()
		{
			// First determine if this function is being run on the local player
			if(isLocalPlayer)
			{
				if(Input.GetKeyDown(KeyCode.Space))
				{
					// Run a function that tells every client to change the colour of this gameobject
					CmdRandomColor(Random.Range(0f, 1f));
				}

				if(Input.GetKeyDown(KeyCode.E))
				{
					CmdSpawnEnemy();			
				}
			}
		}

		[Command]
		public void CmdSpawnEnemy()
		{
			// NetworkServer.Spawn requires an instance of the object in the server's scene to be present
			// so if the object being spawned is a prefab, instantiate needs to be called first
			GameObject newEnemy = Instantiate(enemyToSpawn);
			NetworkServer.Spawn(newEnemy);
		}
		
		// IMPORTANT - RULES FOR COMMANDS:
		// 1. Cannot return anything
		// 2. Must follow the correct naming convention: The function name MUST start with 'Cmd' exactly like that
		// 3. The function must have the attribute [Command] found in the Mirror namespace
		// 4. Can only be certain serializable types (see Command in the Mirror documentation at https://mirror-networking.gitbook.io/docs/guides/data-types)
		[Command]
		public void CmdRandomColor(float _hue)
		{
			//this is running on the server
			RpcRandomColor(_hue);
		}
		
		// IMPORTANT - RULES FOR CLIENT RPC:
		// 1. Cannot return anything
		// 2. Must follow the correct naming convention: The function name MUST start with 'Rpc' exactly like that
		// 3. The function must have the attribute [ClientRpc] found in the Mirror namespace
		// 4.Can only be certain serializable types (see Command in the Mirror documentation at https://mirror-networking.gitbook.io/docs/guides/data-types)
		[ClientRpc]
		public void RpcRandomColor(float _hue)
		{
			// This is running on every instance of the same object that the client was calling from.
			// i.e. Red GO on Red Client runs Cmd, Red GO on Red, Green and Blue client's run Rpc
			MeshRenderer rend = gameObject.GetComponent<MeshRenderer>();
			rend.material.color = Color.HSVToRGB(_hue, 1, 1);
		}

		// This is run via the network starting and the player connecting
		// NOT unity
		// It is run when the object is spawned via the networking system NOT when Unity
		// instantiates the object
		public override void OnStartLocalPlayer()
		{
			// this is run if we are the local player and NOT a remote player
		}

		// This is run via the network starting and the player connecting
		// NOT unity
		// It is run when the object is spawned via the networking system NOT when Unity
		// instantiates the object
		public override void OnStartClient()
		{
			// this is run Regardless if we are the local or remote player
			// isLocalPlayer is true if this object is the client's local player otherwise it's false
			PlayerController controller = gameObject.GetComponent<PlayerController>();
			controller.enabled = isLocalPlayer;
		}
	}
}
