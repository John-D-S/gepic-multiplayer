using AltarChase.Networking;
using AltarChase.Player;

using Mirror;

using System;
using System.Collections;
using System.Collections.Generic;
using AltarChase.Scripts.Xavier_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkGame.Networking
{
    public class MatchManager : NetworkBehaviour
    {
        public static MatchManager instance = null;
        private CountdownTimer countdownTimer;
    
        [SyncVar(hook  = nameof(OnRecievedMatchStarted))] public bool matchStarted = false;

        // Any match settings you want here

        [SyncVar] public bool doubleSpeed = false;
        [SyncVar] public bool isScoreMode = false;
        
        public void StartMatch()
        {
            if(hasAuthority)
            {
                CmdStartMatch();
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdStartMatch()
        {
            matchStarted = true;
        }
        
        private void OnRecievedMatchStarted(bool _old, bool _new)
        {
            if(_new)
            {
                SceneManager.UnloadSceneAsync("Lobby");

                PlayerInteract player = CustomNetworkManager.LocalPlayer;
                Transform startPos = CustomNetworkManager.Instance.GetStartPosition();
                player.transform.position = new Vector3(startPos.position.x, startPos.position.y + 1, startPos.position.z);
                player.transform.rotation = startPos.rotation;
            }
        }
        
        

        protected void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else if(instance != this)
            {
                Destroy(gameObject);
                return;
            }
            // Anything else you want to do in awake
            
        }
    }
}