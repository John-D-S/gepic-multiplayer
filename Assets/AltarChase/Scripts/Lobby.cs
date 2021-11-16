

using AltarChase.LevelGen;
using AltarChase.Networking;
using AltarChase.Player;

using Mirror;

using NetworkGame.Networking;

using System;
using System.Collections.Generic;
using AltarChase.Scripts.Xavier_Scripts;
using TMPro;

using UnityEngine;
using UnityEngine.UI;


namespace Networking.Scripts
{
	public class Lobby: MonoBehaviour
	{
		[SerializeField] private Button startButton;
		[Header("Character Variables")]
		[SerializeField] private TMP_InputField characterNameInput;
		[SerializeField] private TMP_Dropdown characterDropdown;
		[SerializeField] private List<Sprite> characterSprites = new List<Sprite>();
		[SerializeField] private Image characterImage;
		[SerializeField] private int index = 0;

		private LevelGenerator theLevelGenerator;
		public LevelGenerator TheLevelGenerator
		{
			get
			{
				LevelGenerator returnVal = null;
				if(!theLevelGenerator)
				{
					returnVal = FindObjectOfType<LevelGenerator>();
				}

				return returnVal;
			}
		}

		public void SetNumberOfTiles(float _numberOfTiles) => TheLevelGenerator.SetNumberOfTiles(Mathf.RoundToInt(_numberOfTiles));
		public void SetNumberOfSpawnPoints(float _numberOfSpawnPoints) => TheLevelGenerator.SetNumberOfSpawnPoints(Mathf.RoundToInt(_numberOfSpawnPoints));
		public void SetNumberOfPickups(float _numberOfPickups) => TheLevelGenerator.SetNumberOfPickups(Mathf.RoundToInt(_numberOfPickups));
		public void SetNumberOfExits(float _numberOfExits) => TheLevelGenerator.SetNumberOfExits(Mathf.RoundToInt(_numberOfExits));
		
		private void Awake()
		{
			startButton.interactable = CustomNetworkManager.Instance.IsHost;
		}

		public void OnClickStartMatch()
		{
			
			PlayerInteract localPlayer = CustomNetworkManager.LocalPlayer;

			TheLevelGenerator.RegenerateLevelOnline();


			MatchManager.instance.StartMatch();
			
			gameObject.SetActive(false);

			CountdownTimer count = FindObjectOfType<CountdownTimer>();
			count.timerRunning = true;
		}

		/// <summary>
		/// Calls functions on the player to change the character model.
		/// </summary>
		public void CharacterChoice()
		{
			index = characterDropdown.value;
			PlayerInteract localPlayer = CustomNetworkManager.LocalPlayer;
			localPlayer.modelIndex = index;
			localPlayer.CmdChangeModel(localPlayer.modelIndex);
			characterImage.sprite = characterSprites[index];
		}

		/// <summary>
		/// Calls functions on the player to change character name.
		/// </summary>
		public void CharacterName()
		{
			PlayerInteract localPlayer = CustomNetworkManager.LocalPlayer;
			localPlayer.CmdCharacterName(characterNameInput.text);
		}
	}
}