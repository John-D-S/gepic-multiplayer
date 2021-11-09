

using AltarChase.Networking;
using AltarChase.Player;

using Mirror;

using NetworkGame.Networking;

using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;


namespace Networking.Scripts
{
	public class Lobby: MonoBehaviour
	{
		[SerializeField] private Button startButton;
		[SerializeField] private TMP_Dropdown characterDropdown;
		[SerializeField] private int index = 0;
		
		private void Awake()
		{
			startButton.interactable = CustomNetworkManager.Instance.IsHost;
			
		}

		public void OnClickStartMatch()
		{
			PlayerInteract localPlayer = CustomNetworkManager.LocalPlayer;
			localPlayer.modelIndex = index;
			localPlayer.CmdChangeModel(localPlayer.modelIndex);
			
			MatchManager.instance.StartMatch();
			
			gameObject.SetActive(false);
		}

		public void CharacterChoice()
		{
			index = characterDropdown.value;
			PlayerInteract localPlayer = CustomNetworkManager.LocalPlayer;
			localPlayer.modelIndex = index;
			localPlayer.CmdChangeModel(localPlayer.modelIndex);
		}
	}
}