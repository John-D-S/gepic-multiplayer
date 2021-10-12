using kcp2k;

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
			ushort port = 7777;
			//if the address contains a colon, it has a port
			if(address.Contains(":"))
			{
				//get everything after the colon
				string portID = address.Substring(address.IndexOf(":", StringComparison.Ordinal) + 1);
				//turn it into a port
				port = ushort.Parse(portID);
				//remove the port from the address
				address = address.Substring(0, address.IndexOf(":", StringComparison.Ordinal));
			}
			
			if(!IPAddress.TryParse(address, out IPAddress ipAddress))
			{
				Debug.LogError($"Invalid IP: {address}");
				address = "localhost";
			}
			
			((KcpTransport)Transport.activeTransport).Port = port;
			networkManager.networkAddress = address;
			networkManager.StartClient();
		}
	}
}
