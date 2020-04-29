using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CPlayerManager : MonoBehaviourPun, IPunObservable
{
    public static CPlayerManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<CPlayerManager>();

            return instance;
        }
    }
    private static CPlayerManager instance;


    public List<string> playerName = new List<string>();

    public List<int> playerHp = new List<int>();
    public List<int> playerKill = new List<int>();
    public List<int> playerDead = new List<int>();

    public List<int> whoPlayerDied = new List<int>();
    public List<int> whoPlayerKilled = new List<int>();

    public List<bool> isPlayerKill = new List<bool>();
    public List<bool> isPlayerDead = new List<bool>();

    public void AddPlayer(int hp)
    {
        playerName.Add(string.Empty);

        playerHp.Add(hp);
        playerKill.Add(0);
        playerDead.Add(0);

        whoPlayerDied.Add(0);
        whoPlayerKilled.Add(0);

        isPlayerKill.Add(false);
        isPlayerDead.Add(false);
    }

    //-----------------------------------------------------------------------------------------------

    public void SetNameRequest(int playerNum, string name)
    {
        photonView.RPC("RpcSetName", RpcTarget.MasterClient, playerNum, name);
    }

    [PunRPC]
    void RpcSetName(int playerNum, string name)
    {
        playerName[playerNum] = name;
    }

    //-----------------------------------------------------------------------------------------------


    //마스터클라이언트에게 playerNum에게 HP 손상을 요청
    public void ReduceHp(int attackPlayer, int hitPlayer)
    {
        photonView.RPC("RpcReduceHpRequest", RpcTarget.MasterClient, attackPlayer, hitPlayer);
    }

    //요청받은 playerNum에 HP 손상을 입힘
    [PunRPC]
    void RpcReduceHpRequest(int attackPlayer, int hitPlayer)
    {
        if (isPlayerKill[hitPlayer].Equals(false))
        {
            playerHp[hitPlayer] -= 10;

            if (playerHp[hitPlayer] <= 0)
            {
                whoPlayerKilled[attackPlayer] = hitPlayer;
                playerKill[attackPlayer]++;
                isPlayerDead[attackPlayer] = true;

                whoPlayerDied[hitPlayer] = attackPlayer;
                playerDead[hitPlayer]++;
                isPlayerKill[hitPlayer] = true;

                playerHp[hitPlayer] = 100;
            }
        }
    }


    //-----------------------------------------------------------------------------------------------

    public void IsKillResetRequest(int playerNum)
    {
        photonView.RPC("IsKillReset", RpcTarget.MasterClient, playerNum);
    }

    [PunRPC]
    void IsKillReset(int playerNum)
    {
        isPlayerDead[playerNum] = false;
    }

    public void ReviveRequest(int playerNum)
    {
        photonView.RPC("RpcRevive", RpcTarget.MasterClient, playerNum);
    }

    [PunRPC]
    void RpcRevive(int playerNum)
    {
        isPlayerKill[playerNum] = false;
    }


    //-----------------------------------------------------------------------------------------------


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //플레이어 이름
            for (int i = 0; i < playerName.Count; i++)
            {
                if (i > playerName.Count - 1) continue;

                stream.SendNext(playerName[i]);
            }

            //HP 동기화
            for (int i = 0; i < playerHp.Count; i++)
            {
                if (i > playerHp.Count - 1) continue;

                stream.SendNext(playerHp[i]);
            }

            //킬수 동기화
            for (int i = 0; i < playerKill.Count; i++)
            {
                if (i > playerKill.Count - 1) continue;

                stream.SendNext(playerKill[i]);
            }

            //죽은 횟수 동기화
            for (int i = 0; i < playerDead.Count; i++)
            {
                if (i > playerDead.Count - 1) continue;

                stream.SendNext(playerDead[i]);
            }

            //나를 누가 죽였는지 동기화
            for (int i = 0; i < whoPlayerDied.Count; i++)
            {
                if (i > whoPlayerDied.Count - 1) continue;

                stream.SendNext(whoPlayerDied[i]);
            }

            //내가 누구를 죽였는지 동기화
            for (int i = 0; i < whoPlayerKilled.Count; i++)
            {
                if (i > whoPlayerKilled.Count - 1) continue;

                stream.SendNext(whoPlayerKilled[i]);
            }

            //Dead True/False Check
            for (int i = 0; i < isPlayerKill.Count; i++)
            {
                if (i > isPlayerKill.Count - 1) continue;

                stream.SendNext(isPlayerKill[i]);
            }

            //Kill True/False Check
            for (int i = 0; i < isPlayerDead.Count; i++)
            {
                if (i > isPlayerDead.Count - 1) continue;

                stream.SendNext(isPlayerDead[i]);
            }
        }
        //동기화한것들 클라이언트들에게 넘겨주기
        else
        {
            for (int i = 0; i < playerName.Count; i++)
            {
                if (i > playerName.Count - 1) continue;

                playerName[i] = (string)stream.ReceiveNext();
            }

            for (int i = 0; i < playerHp.Count; i++)
            {
                if (i > playerHp.Count - 1) continue;

                playerHp[i] = (int)stream.ReceiveNext();
            }

            for (int i = 0; i < playerKill.Count; i++)
            {
                if (i > playerKill.Count - 1) continue;

                playerKill[i] = (int)stream.ReceiveNext();
            }

            for (int i = 0; i < playerDead.Count; i++)
            {
                if (i > playerDead.Count - 1) continue;

                playerDead[i] = (int)stream.ReceiveNext();
            }

            for (int i = 0; i < whoPlayerDied.Count; i++)
            {
                if (i > whoPlayerDied.Count - 1) continue;

                whoPlayerDied[i] = (int)stream.ReceiveNext();
            }

            for (int i = 0; i < whoPlayerKilled.Count; i++)
            {
                if (i > whoPlayerKilled.Count - 1) continue;

                whoPlayerKilled[i] = (int)stream.ReceiveNext();
            }

            for (int i = 0; i < isPlayerKill.Count; i++)
            {
                if (i > isPlayerKill.Count - 1) continue;

                isPlayerKill[i] = (bool)stream.ReceiveNext();
            }

            for (int i = 0; i < isPlayerDead.Count; i++)
            {
                if (i > isPlayerDead.Count - 1) continue;

                isPlayerDead[i] = (bool)stream.ReceiveNext();
            }
        }
    }
}
