using AltarChase.Player;

using System.Collections.Generic;
using UnityEngine;
using Mirror;

using System.Collections;

namespace AltarChase
{
	/// <summary>
	/// This class handles the speed boost pickup for the character
	/// </summary>
	public class SpeedBoost : NetworkBehaviour
	{
		
		/// <summary>
		/// This is the Ontrigger for the speed boost pickup and is running on the server.
		/// </summary>
		/// <param name="_collider">The collider of the player that hit the trap.</param>
		[Server]
		private void OnTriggerEnter(Collider _collider)
		{
			if(_collider.CompareTag("Player"))
			{
				NetworkIdentity identity = _collider.GetComponent<NetworkIdentity>();
				TargetSpeedBoost(identity.connectionToClient);
			}
            
		}
		
		/// <summary>
		/// This calls the coroutine that speeds up the PlayerMotor of the player that hits the pickup.
		/// </summary>
		/// <param name="_target">The NetworkConnection of the player</param>
		[TargetRpc]
		public void TargetSpeedBoost(NetworkConnection _target)
		{
			PlayerMotor motor = _target.identity.gameObject.GetComponent<PlayerMotor>();
			motor.CallSpeedBoostCR();
			NetworkServer.Destroy(gameObject);

		}
		
		
	}
}