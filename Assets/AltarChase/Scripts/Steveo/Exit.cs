using AltarChase.Player;

using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace AltarChase
{
	public class Exit : NetworkBehaviour
	{
		/// <summary>
		/// This is the Ontrigger for the exit and is running on the server.
		/// </summary>
		/// <param name="_collider">The collider of the player that hit the trap.</param>
		[Server]
		private void OnTriggerEnter(Collider _collider)
		{
			if(_collider.CompareTag("Player"))
			{
				NetworkIdentity identity = _collider.GetComponent<NetworkIdentity>();
				PlayerInteract playerInteract = identity.GetComponent<PlayerInteract>();
				if(playerInteract.isHoldingArtifact)
				{
					FindObjectOfType<Popup>().RpcPopupText($"{playerInteract.characterName} has escaped with the artifact");
				}
				
				if(!playerInteract.isHoldingArtifact)
				{
					Debug.Log("You need the artifact to escape.");
					// Put in a local player side display message.
				}

			}
            
		}
	}
}