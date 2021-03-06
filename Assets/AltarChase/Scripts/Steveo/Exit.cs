using AltarChase.Player;

using System.Collections.Generic;
using UnityEngine;
using Mirror;

using NetworkGame.Networking;

using System;

using UnityEngine.SceneManagement;

namespace AltarChase
{
	public class Exit : NetworkBehaviour
	{
		private string characterName = "";
		/// <summary>
		/// This is the Ontrigger for the exit and is running on the server.
		/// </summary>
		/// <param name="_collider">The collider of the player that hit the exit.</param>
		private void OnTriggerEnter(Collider _collider)
		{
			if(_collider.CompareTag("Player"))
			{
				NetworkIdentity identity = _collider.GetComponent<NetworkIdentity>();
				PlayerInteract playerInteract = identity.GetComponent<PlayerInteract>();
				if(playerInteract.isHoldingArtifact)
				{
					CmdPlayerWinsGame(identity, playerInteract.characterName);
				}
			}
		}
		
		[Command(requiresAuthority = false)]
		private void CmdPlayerWinsGame(NetworkIdentity playerIdentity, string characterName)
		{
			FindObjectOfType<Popup>().RpcPopupText($"{characterName} has escaped with the artifact");
			RpcHidePayer(playerIdentity.gameObject);
		}

		/// <summary>
		/// Hides the player once they exit and loads main menu and stops host and server.
		/// </summary>
		/// <param name="_player"></param>
		[ClientRpc]
		public void RpcHidePayer(GameObject _player)
		{
			_player.SetActive(false);
			MatchManager.instance.CallLoadMainMenu(3);
		}

		
	}
}