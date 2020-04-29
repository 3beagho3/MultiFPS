using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class CPlayer : MonoBehaviourPun
{
    private CharacterController playerCc;
    private Animator playerAni;

    public int playerNum = 0;

    private bool isGround = false;

    private float maxSlopeAngle = 75;

    public Image hpSlider;
    public int maxHp = 100;
    private float currentHp = 0;

    public Text KD;
    public Text noticeText;
    private string noticeTextSet;
    private bool doNoticeSet = false;

    [Space]

    public float gravity = -9.81f;
    private float currentGravity = 0;

    public float additionSpeed = 1.5f;
    private float currentAdditionSpeed = 1.0f;

    public float moveSpeed = 3.5f;
    public float jumpSpeed = 1.5f;

    private void Awake()
    {
        string tempPlayerNum = photonView.ViewID.ToString();
        tempPlayerNum = tempPlayerNum.Substring(0, 1);
        playerNum = int.Parse(tempPlayerNum) - 1;

        while (playerNum >= CPlayerManager.Instance.playerHp.Count)
        {
            CPlayerManager.Instance.AddPlayer(maxHp);
        }

        if (!photonView.IsMine)
            return;

        playerCc = GetComponent<CharacterController>();
        playerAni = GetComponent<Animator>();

        currentHp = maxHp;

        hpSlider = GameObject.FindGameObjectWithTag("HpSlider").GetComponent<Image>();

        KD = GameObject.Find("UI").transform.Find("KD").GetComponent<Text>();

        noticeText = GameObject.FindGameObjectWithTag("NoticeText").GetComponent<Text>();
        noticeText.gameObject.SetActive(false);
    }

    void Start()
    {
        if (!photonView.IsMine)
            return;

        StartCoroutine(NoticeSet());
        CPlayerManager.Instance.SetNameRequest(playerNum, CLobbyManager.playerName);
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        KD.text = $"K : {CPlayerManager.Instance.playerKill[playerNum]} / D : {CPlayerManager.Instance.playerDead[playerNum]}";

        PlayerStateCheck();
        PlayerHpSet();

    }

    private void LateUpdate()
    {
        if (!photonView.IsMine || CGameManager.isOptionOpen)
            return;

        float rotateAmount = Camera.main.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, rotateAmount, 0);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        PlayerMove();
    }

    private void PlayerStateCheck()
    {
        if (CPlayerManager.Instance.isPlayerKill.Count - 1 < playerNum &&
            CPlayerManager.Instance.isPlayerDead.Count - 1 < playerNum)
        {
            return;
        }

        if (CPlayerManager.Instance.isPlayerKill[playerNum].Equals(true))
        {
            playerCc.Move(new Vector3(Random.Range(-40, 40), 40, Random.Range(-50, 50)) - transform.position);

            noticeTextSet = $"{CPlayerManager.Instance.playerName[CPlayerManager.Instance.whoPlayerDied[playerNum]]} 에게 처치당함";
            doNoticeSet = true;

            CPlayerManager.Instance.ReviveRequest(playerNum);
        }

        if (CPlayerManager.Instance.isPlayerDead[playerNum].Equals(true))
        {
            noticeTextSet = $"{CPlayerManager.Instance.playerName[CPlayerManager.Instance.whoPlayerKilled[playerNum]]} (을)를 처치";
            doNoticeSet = true;

            CPlayerManager.Instance.IsKillResetRequest(playerNum);
        }
    }

    private void PlayerHpSet()
    {
        if (CPlayerManager.Instance.playerHp.Count - 1 < playerNum) return;

        currentHp = CPlayerManager.Instance.playerHp[playerNum];
        hpSlider.fillAmount = currentHp / maxHp;
    }

    private void PlayerMove()
    {
        if (!isGround)
            currentGravity += gravity * Time.deltaTime;
        else
            currentGravity = 0;

        currentGravity += gravity * Time.deltaTime;

        currentAdditionSpeed = Input.GetKey(KeyCode.LeftShift) ? additionSpeed : 1;
        playerAni.SetBool("IsRun", Input.GetKey(KeyCode.LeftShift));

        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        if(CGameManager.isOptionOpen.Equals(true))
        {
            xAxis = 0;
            zAxis = 0;
        }

        playerAni.SetFloat("xAxis", xAxis);
        playerAni.SetFloat("zAxis", zAxis);

        if((xAxis * xAxis + zAxis * zAxis) > 0)
            playerAni.SetBool("IsWalk", true);
        else
            playerAni.SetBool("IsWalk", false);

        Vector3 movemet = new Vector3(xAxis, 0, zAxis) * moveSpeed * currentAdditionSpeed * Time.deltaTime;
        movemet.y = currentGravity;

        playerCc.Move(transform.rotation * movemet);


        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            currentGravity = jumpSpeed;
        }
        isGround = false;
    }

    public IEnumerator NoticeSet()
    {
        while (true)
        {
            if (doNoticeSet)
            {
                doNoticeSet = false;
                
                noticeText.gameObject.SetActive(true);
                noticeText.text = noticeTextSet;

                yield return new WaitForSeconds(1.5f);

                noticeText.text = string.Empty;
                noticeText.gameObject.SetActive(false);
            }

            yield return null;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsFloor(hit.normal))
        {
            isGround = true;
        }
    }

    bool IsFloor(Vector3 normal)
    {
        float angle = Vector3.Angle(Vector3.up, normal);
        return angle < maxSlopeAngle;
    }
}
