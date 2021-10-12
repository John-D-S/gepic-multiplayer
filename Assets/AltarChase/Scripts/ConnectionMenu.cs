using Mirror;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace AltarChase
{
	public class ConnectionMenu : MonoBehaviour
	{
		private NetworkManager networkManager;

		[SerializeField] private Button hostButton;
		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private Button connectButton;

		private void Start()
		{
			networkManager = NetworkManager.singleton;
			
			hostButton.onClick.AddListener(OnClickHost);
			inputField.onEndEdit.AddListener(OnEndEditAddress);
			connectButton.onClick.AddListener(OnClickConnect);
		}

		private void OnClickHost() => networkManager.StartHost();
		private void OnEndEditAddress(string _value) => networkManager.networkAddress = _value;

		private void OnClickConnect()
		{
			string address = inputField.text;
			if(!IPAddress.TryParse(address, out IPAddress ipAddress))
			{
				address = "localhost";
			}

			networkManager.networkAddress = address;
			networkManager.StartClient();
		}
	}
}
