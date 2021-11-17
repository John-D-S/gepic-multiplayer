using Mirror;
using NetworkGame.Networking;
using TMPro;
using UnityEngine;

namespace AltarChase.Scripts.Xavier_Scripts
{
    public class CountdownTimer : NetworkBehaviour
    {
        [Header("Timer Set")]
        [SerializeField,SyncVar] public float timeRemaining = 120;

        private Popup _popup;
        [SyncVar] private float minutes;
        [SyncVar] private float seconds;
        public TextMeshProUGUI timer;
  
        [SyncVar]public bool timerRunning = false;
   
   
        private void Start()
        {
            _popup = FindObjectOfType<Popup>();
            timerRunning = false;
        }
   
        private void Update()
        {
            if(timerRunning)
            {
                TimeOnEveryone();
            }
            
            if(MatchManager.instance.isScoreMode && MatchManager.instance.matchStarted)
            {
                timerRunning = true;
                timer.gameObject.SetActive(true);
            }
            else if(MatchManager.instance.matchStarted && !MatchManager.instance.isScoreMode)
            {
                timer.gameObject.SetActive(false);
            }
        }
   
        
        /// <summary>
        /// This is the function that will be called in every client version.
        /// </summary>
        void TimeOnEveryone()
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                ServerDisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has ran out!");
                if (timerRunning)
                {
                    MatchManager.instance.FindHighestScore();
                    if(MatchManager.instance.playerWithHighestScore != null)
                    {
                        _popup.RpcPopupText($"{MatchManager.instance.playerWithHighestScore.characterName} WINS! \n They held onto the artifact the longest");
                    }
                    else
                    {
                        _popup.RpcPopupText($"Nobody WINS! \n The artifact was never found");

                    }
                    timeRemaining = 0;
                    timerRunning = false;
                    MatchManager.instance.CallLoadMainMenu(5);
                }
                timer.text = "0:00";
            }
        }
   
        /// <summary>
        /// And here is how we display the time ACCURATELY in the game for every player.
        /// </summary>
        /// <param name="timeToDisplay"> This float value can be changed and then turned into minutes and seconds!</param>
        void ServerDisplayTime(float timeToDisplay)
        {
            minutes = Mathf.FloorToInt(timeToDisplay / 60); 
            seconds = Mathf.FloorToInt(timeToDisplay % 60);

            timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        /// <summary>
        ///  Same thing as server OLD CODE!
        /// </summary>
        /// <param name="_timeToDisplay"></param>
        void ClientDisplayTime(float _timeToDisplay)
        {
            float minutes = Mathf.FloorToInt(_timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(_timeToDisplay % 60);

            timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}