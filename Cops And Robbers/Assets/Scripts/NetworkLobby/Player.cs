﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace Me.DerangedSenators.CopsAndRobbers {
    public class Player : NetworkBehaviour
    {

        public static Player localPlayer;
        [SyncVar] public string MatchId;
        [SyncVar] public int playerIndex;

        NetworkMatchChecker networkMatchChecker;

        [SyncVar] public Match currentMatch;

        GameObject playerLobbyUI;

        void Awake()
        {
            networkMatchChecker = GetComponent<NetworkMatchChecker>();

        }

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                localPlayer = this;
            }
            else
            {
                Debug.Log($"Spawning other player UI");
                playerLobbyUI = UILobby.instance.SpawnUIPlayerPrefab(this);
            }
        }

        public override void OnStopClient()
        {
            Debug.Log($"Client stopped");
            ClientDisconnect();
        }

        public override void OnStopServer()
        {
            Debug.Log($"Client stopped on server");
            ServerDisconnect();
        }

        /*
         * HOST GAME
         */

        public void HostGame(bool publicMatch)
        {
            string matchId = MatchMaker.GetRandomMatchId();
            CmdHostGame(matchId, publicMatch);
        }
        
        

        [Command]
        void CmdHostGame(string matchId, bool publicMatch)
        {
            MatchId = matchId;
            if (MatchMaker.instance.HostGame(matchId, gameObject, publicMatch, out playerIndex))
            {
                Debug.Log($"<color=green>Game hosted successfully</color>");

                networkMatchChecker.matchId = matchId.ToGuid();
                TargetHostGame(true, matchId, playerIndex);
            }
            else
            {
                Debug.Log($"<color=red>Game host failed</color>");
                TargetHostGame(false, matchId, playerIndex);
            }
        }

        [TargetRpc]
        void TargetHostGame(bool success, string matchId, int playerIndex)
        {
            MatchId = matchId;
            this.playerIndex = playerIndex;
            Debug.Log($"Match ID: {MatchId} == {matchId}");
            UILobby.instance.HostSuccess(success, matchId);
        }

        /*
         * JOIN GAME
         */

        public void JoinGame(string matchId)
        {
            string matchID = matchId;
            CmdJoinGame(matchID);
        }
        [Command]
        void CmdJoinGame(string matchId)
        {
            MatchId = matchId;
            if (MatchMaker.instance.JoinGame(matchId, gameObject, out playerIndex))
            {
                Debug.Log($"<color=green>Game Joined successfully</color>");

                networkMatchChecker.matchId = matchId.ToGuid();
                TargetJoinGame(true, matchId, playerIndex);
            }
            else
            {
                Debug.Log($"<color=red>Game Join failed</color>");
                TargetJoinGame(false, matchId, playerIndex);
            }
        }

        [TargetRpc]
        void TargetJoinGame(bool success, string matchId, int playerIndex)
        {
            MatchId = matchId;
            this.playerIndex = playerIndex;
            Debug.Log($"Match ID: {MatchId} == {matchId}");
            UILobby.instance.JoinSuccess(success, matchId);
        }

        /*
         * SEARCHING FOR GAME
         */
        public void SearchGame()
        {
            CmdSearchGame();
        }

        [Command]
        void CmdSearchGame()
        {
            if (MatchMaker.instance.SearchGame(gameObject, out playerIndex, out MatchId))
            {
                Debug.Log($"<color=green>Game Found</color>");

                networkMatchChecker.matchId = MatchId.ToGuid();
                TargetSearchGame(true, MatchId, playerIndex);
            }
            else
            {
                Debug.Log($"<color=red>Game not Found</color>");
                TargetSearchGame(false, MatchId, playerIndex);
            }
        }

        [TargetRpc]
        void TargetSearchGame(bool success, string matchId, int playerIndex)
        {
            this.playerIndex = playerIndex;
            MatchId = matchId;
            Debug.Log($"Match ID: {MatchId} == {matchId}");
            UILobby.instance.SearchSuccess(success, matchId);
        }

        /*
         * BEGIN GAME
         */

        public void BeginGame()
        {
            CmdBeginGame();
        }
        [Command]
        void CmdBeginGame()
        {
            MatchMaker.instance.BeginGame(MatchId);
            Debug.Log($"<color=yellow>Game Beginning</color>");
        }

        public void StartGame()
        {
            TargetBeginGame();
        }

        [TargetRpc]
        void TargetBeginGame()
        {
            Debug.Log($"Match ID: {MatchId} | Beginning");
            //Additively load game scene
            SceneManager.LoadScene(3, LoadSceneMode.Additive);
        }

        /*
         * DISCONNECT GAME
         */

        public void DisconnectGame()
        {
            CmdDisconnectGame();
        }

        [Command]
        void CmdDisconnectGame()
        {
            ServerDisconnect();
        }

        void ServerDisconnect()
        {
            MatchMaker.instance.PlayerDisconnected(this, MatchId);
            networkMatchChecker.matchId = string.Empty.ToGuid();
            RpcDisconnectGame();
        }

        [ClientRpc]
        void RpcDisconnectGame()
        {
            ClientDisconnect();
        }

        void ClientDisconnect()
        {

            //destroy UIPlayer
            if (playerLobbyUI != null)
            {
                Destroy(playerLobbyUI);
            }
        }
    }
}