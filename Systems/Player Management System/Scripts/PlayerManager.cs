using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class PlayerManager : ManagerBase
    {
        public static PlayerManager Instance { get; private set; }

        [SerializeField] GameObject _playerPrefab;
        [SerializeField] float _verticalDeathThreshold = -36f;
        [SerializeField] string _playerInteractionText = "F";
        [SerializeField] public bool _recordLevelData = true;

        GameObject _activePlayerObject;
        PlayerCharacter _activePlayer;
        SpawnController _spawnController;

        #region Public accessors and events
        public GameObject ActivePlayerObject => _activePlayerObject;
        public PlayerCharacter ActivePlayer => _activePlayer;
        public string PlayerInteractionText => _playerInteractionText;
        //Events

        public delegate void NewPlayerSpawnedHandler(PlayerCharacter newPlayer);
        public NewPlayerSpawnedHandler onNewPlayerSpawned { get; set; }

        public delegate void PlayerEnteredLightHandler();
        public PlayerEnteredLightHandler onPlayerEnteredLight { get; set; }

        public delegate void PlayerExitLightHandler();
        public PlayerExitLightHandler onPlayerExitLight { get; set; }
        public delegate void PlayerEnteredSanityAfflictor();
        public PlayerEnteredSanityAfflictor onPlayerEnteredSanityAfflictor { get; set; }
        public delegate void PlayerExitSanityAfflictor();
        public PlayerExitSanityAfflictor onPlayerExitSanityAfflictor { get; set; }

        //Quick fix......
        public delegate void PlayerCollectObjectHandler(bool isCoin, bool isFragment);
        public PlayerCollectObjectHandler OnPlayerCollectObject { get; set; }
        #endregion

        #region Initialization
        protected override void MonoAwake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        public override void Init()
        {
            Debugger.LogInit("Init in Player Manager");
            Managers.playerManager = this;
            Player.playerManager = this;
        }
        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in Player Manager");
            _spawnController = new SpawnController();
        }
        public override void MyStart()
        {
            Debugger.LogInit("MyStart in Player Manager");
            Managers.sanityManager.onSanityDepleted += _OnSanityDepleted;
        }
        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<PlayerManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }
        #endregion Initialization


        public override void MyUpdate()
        {
            base.MyUpdate();
            _CheckPlayerHeight();
        }


        #region Player Light and Sanity

        public void PlayerHasEnteredLight()
        {
            onPlayerEnteredLight?.Invoke();
            Player.onLightEntered?.Invoke();
        }
        public void PlayerHasExitLight()
        {
            onPlayerExitLight?.Invoke();
            Player.onLightExit?.Invoke();
        }

        public void PlayerHasEnteredSanityAfflictor()
        {
            onPlayerEnteredSanityAfflictor?.Invoke();
            Player.onSanityAfflictorEntered?.Invoke();
        }
        public void PlayerHasExitSanityAfflictor()
        {
            onPlayerExitSanityAfflictor?.Invoke();
            Player.onSanityAfflictorExit?.Invoke();
        }

        #endregion Player Light and Sanity


        #region Player Spawn
        public void SpawnPlayer()
        {
            _playerDeathSensorTriggered = false;

            //Instantiate player if there is not already one
            if (_activePlayerObject == null)
            {
                _activePlayerObject = Instantiate(_playerPrefab);
            }

            //Make sure the player doesn't stay within a disposable scene
            DontDestroyOnLoad(_activePlayerObject);

            //Teleport the player to the spawn point
            List<SpawnPoint> points = _spawnController.GetValidPoints();
            PlayerSpawnPoint spawnPoint = null;
            foreach (var item in points)
            {
                if (item is PlayerSpawnPoint)
                {
                    var point = (PlayerSpawnPoint)item;
                    if (point.isMainSpawnPoint)
                        spawnPoint = point;
                }
            }

            //Apply the spawn position and rotation to the player
            _spawnController.ApplySpawnPosRotToObject(_activePlayerObject, spawnPoint);
        }

        public IEnumerator Co_RespawnPlayer()
        {
            if (Saver.progression.curseApplied)
                Metrics.player.deathsAmount += 1;
            //Play the sounds and effects from the playercharacter scripts
            _activePlayer.OnPlayerDeathSound();
            _activePlayer.StopHeartbeat();

            //Destroy all temporary objets from other levels
            foreach (var item in FindObjectsOfType<DestroyOnFragmentDeposit>())
            {
                item.DestroyObject();
            }

            yield return StartCoroutine(HUD.blackFader.Co_FadeToBlack());

            Sanity.manager.RestoreSanity(1f);

            SpawnPlayer();

            yield return StartCoroutine(HUD.blackFader.Co_FadeFromBlack());
        }

        public void RegisterPlayerSpawnPoint(PlayerSpawnPoint spawnPoint)
        {
            _spawnController.AddNewSpawnPoint(spawnPoint);
        }
        public void DeregisterPlayerSpawnPoint(PlayerSpawnPoint spawnPoint)
        {
            _spawnController.RemoveNewSpawnPoint(spawnPoint);
        }

        public bool RegisterPlayer(PlayerCharacter player)
        {
            if (_activePlayer != null)
            {
                Debugger.LogWarning($"There is already an active player assigned to the {nameof(PlayerManager)}, deleting the new player now.");
                player.gameObject.DestroyMyObject();
                return false;
            }

            _activePlayer = player;
            _activePlayerObject = player.gameObject;

            DontDestroyOnLoad(_activePlayerObject);

            onNewPlayerSpawned?.Invoke(player);

            return true;
        }

        #endregion Player Spawn

        

        private void _OnSanityDepleted()
        {
            Managers.eventManager.FireGameEvent(GameEvent.PlayerDied, this);
        }


        #region Player death

        bool _playerDeathSensorTriggered = false;
        void _CheckPlayerHeight()
        {
            if (_activePlayerObject == null)
                return;
            
            if (_activePlayerObject.transform.position.y <= _verticalDeathThreshold && _playerDeathSensorTriggered == false)
            {
                _playerDeathSensorTriggered = true;
                Managers.eventManager.FireGameEvent(GameEvent.PlayerDied, this);
            }
        }

        public IEnumerator Co_KillPlayer()
        {
            if (Saver.progression.curseApplied)
                Metrics.player.deathsAmount += 1;
            //Play the sounds and effects from the playercharacter scripts
            _activePlayer.OnPlayerDeathSound();
            _activePlayer.StopHeartbeat();

            //Destroy all temporary objets from other levels
            foreach (var item in FindObjectsOfType<DestroyOnFragmentDeposit>())
            {
                item.DestroyObject();
            }

            yield return StartCoroutine(HUD.blackFader.Co_FadeToBlack());

            _activePlayerObject.DestroyMyObject();

            _activePlayerObject = null;
            _activePlayer = null;
            _playerDeathSensorTriggered = false;

            //Remove the sanity bar
            //Managers.sanityManager.StopAfflictingSanity();
        }

        #endregion Player death


    }
}