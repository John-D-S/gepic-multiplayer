

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
		[SerializeField] private Slider noOfTilesSlider;
		
		
		[SerializeField] private Slider noOfExitsSlider;
		private bool setExits = false;

		private CountdownTimer countdownTimer;
		private MatchManager matchManager;
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

		private int numberOfSpawnpoints = 4;
		public void SetNumberOfSpawnPoints(float _numberOfSpawnPoints)
		{
			numberOfSpawnpoints = Mathf.RoundToInt(_numberOfSpawnPoints);
			TheLevelGenerator.SetNumberOfSpawnPoints(numberOfSpawnpoints);
			noOfTilesSlider.minValue = numberOfSpawnpoints + numberOfPickups + numberOfExits + 1;
		}

		private int numberOfPickups = 25;
		public void SetNumberOfPickups(float _numberOfPickups)
		{
			numberOfPickups = Mathf.RoundToInt(_numberOfPickups);
			TheLevelGenerator.SetNumberOfPickups(numberOfPickups);
			noOfTilesSlider.minValue = numberOfSpawnpoints + numberOfPickups + numberOfExits + 1;
		}
		
		private int numberOfExits = 1;
		public void SetNumberOfExits(float _numberOfExits)
		{
			numberOfExits = Mathf.RoundToInt(_numberOfExits);
			TheLevelGenerator.SetNumberOfExits(numberOfExits);
			noOfTilesSlider.minValue = numberOfSpawnpoints + numberOfPickups + numberOfExits + 1;
		} 
		public void SetTimerValue(float _finalTime) => countdownTimer.timeRemaining = _finalTime;

		private void Awake()
		{
			startButton.interactable = CustomNetworkManager.Instance.IsHost;
			countdownTimer = FindObjectOfType<CountdownTimer>();
			matchManager = FindObjectOfType<MatchManager>();
			noOfTilesSlider.minValue = numberOfSpawnpoints + numberOfPickups + numberOfExits + 1;
		}

		private void Update()
		{
			// if playing in score mode the no. of exits will be set to zero and slider non-interacable.
			if(matchManager.isScoreMode)
			{
				noOfExitsSlider.interactable = false;
				if(!setExits)
				{
					SetNumberOfExits(0);
					setExits = true;
				}
			}
			else
			{
				noOfExitsSlider.interactable = true;
				if(!setExits)
				{
					SetNumberOfExits(1);
					setExits = true;
				}
			}
		}

		public void OnClickStartMatch()
		{
			
			PlayerInteract localPlayer = CustomNetworkManager.LocalPlayer;

			TheLevelGenerator.RegenerateLevelOnline();


			MatchManager.instance.StartMatch();
			
			gameObject.SetActive(false);

			CountdownTimer count = FindObjectOfType<CountdownTimer>();
			//count.timerRunning = true;
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
		
		/// <summary>
		/// Function for the Game Mode dropdown selector.
		/// </summary>
		/// <param name="value">value of the dropdown choice</param>
		public void DropdownCheck(int value)
		{
			if (value == 0)
			{
				matchManager.isScoreMode = false;
				setExits = false;
			}
			else
			{
				matchManager.isScoreMode = true;
				setExits = false;
			}
		}
	}
}