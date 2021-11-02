using AltarChase.Player;

using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace AltarChase
{
	/// <summary>
	/// This class handles the functionality of the Artifact Object.
	/// </summary>
	public class Artifact : NetworkBehaviour
	{
		[SyncVar] public bool isHeld;
	
		/// <summary>
		/// This is the Ontrigger for the Artifcat pickup and is running on the server.
		/// </summary>
		/// <param name="_collider">The collider of the player that hit the artifact.</param>
		[Server]
		private void OnTriggerEnter(Collider _collider)
		{
			if(_collider.CompareTag("Player") && !isHeld)
			{
				NetworkIdentity identity = _collider.GetComponent<NetworkIdentity>();
				PlayerInteract interact = identity.gameObject.GetComponent<PlayerInteract>();
				RpcPickUpItem(interact);
				TargetGotArtifact(identity.connectionToClient, interact);
			}
            
		}

		[ClientRpc]
		public void RpcPickUpItem(PlayerInteract _interact)
		{
			transform.parent = _interact.itemLocation.transform;
			transform.position = _interact.itemLocation.position;
		}

		[ClientRpc]
		public void RpcDropItem(PlayerInteract _interact)
		{
			Debug.Log("calling the rpcdropitem");
			transform.position = _interact.itemDropLocation.position;
			transform.parent = null;
			isHeld = false;
		}
		
		/// <summary>
		/// This calls the ....
		/// </summary>
		/// <param name="_target">The NetworkConnection of the player</param>
		[TargetRpc]
		public void TargetGotArtifact(NetworkConnection _target, PlayerInteract _interact)
		{
			
			_interact.GetArtifact(this);
			isHeld = true;
			// dont destroy. NetworkServer.Destroy(gameObject);

		}
	}
}

