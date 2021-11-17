using AltarChase.Player;

using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
		[Server]
		private void OnTriggerEnter(Collider _collider)
		{
			if(_collider.CompareTag("Player"))
			{
				NetworkIdentity identity = _collider.GetComponent<NetworkIdentity>();
				PlayerInteract playerInteract = identity.GetComponent<PlayerInteract>();
				if(playerInteract.isHoldingArtifact)
				{
					characterName = playerInteract.characterName;
					FindObjectOfType<Popup>().RpcPopupText($"{characterName} has escaped with the artifact");
					RpcHidePayer(identity.gameObject);
				}
				
				if(!playerInteract.isHoldingArtifact)
				{
					
					Debug.Log("You need the artifact to escape.");
					// Put in a local player side display message.
				}

			}
            
		}

		[ClientRpc]
		public void RpcHidePayer(GameObject _player)
		{
			_player.SetActive(false);
			Invoke(nameof(LoadMain), 5);
		}

		private void LoadMain() => SceneManager.LoadScene("MainMenu");

	}
}