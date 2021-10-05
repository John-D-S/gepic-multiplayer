using Mirror;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltarChase.Networking
{
	[RequireComponent(typeof(PlayerController))]
	public class NetworkPlayer : NetworkBehaviour
	{
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
