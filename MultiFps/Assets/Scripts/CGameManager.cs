using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CGameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    private bool cursorFixed = true;

    public GameObject OptionPanel;
    public static bool isOptionOpen = false;
    public static float mouseSensitivity = 1;
    public Slider sensitiveSlider;

    void Awake()
    {
        SpawnPlayer();
        CursorSet();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CursorSet();

        mouseSensitivity = sensitiveSlider.value;
    }

    void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(5,5,5), Quaternion.identity);
    }

    void CursorSet()
    {
        if (cursorFixed)
        {
            isOptionOpen = false;
            OptionPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            isOptionOpen = true;
            OptionPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        cursorFixed = !cursorFixed;
    }
}
