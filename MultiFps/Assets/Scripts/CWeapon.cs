using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CWeapon : MonoBehaviourPun
{
    public GameObject weaponEffect;
    private List<GameObject> effctPool = new List<GameObject>();

    public AudioClip shotSound;

    public float shotRateTime = 0.1f;
    private float shotRateTimer = 0;

    private Image crossHairHit;
    public float decreaseTransparency = 0.1f;
    public float currentTransparency = 0;

    void Start()
    {
        if (!photonView.IsMine)
            return;

        //총알 이펙트 풀링
        for (int i = 0; i < 25; i++)
        {
            GameObject tempObj = Instantiate(weaponEffect, Vector3.zero, Quaternion.identity);
            tempObj.SetActive(false);

            effctPool.Add(tempObj);
        }

        crossHairHit = GameObject.FindGameObjectWithTag("CrossHairHit").GetComponent<Image>();

        //첫 총알은 바로 나갈수 있도록 함
        shotRateTimer = shotRateTime;
    }
    void Update()
    {
        if (!photonView.IsMine || CGameManager.isOptionOpen)
            return;

        //연사시간 제한 함
        shotRateTimer += Time.deltaTime;

        RaycastCheck();
        CrossHairHitSet();
    }
    void LateUpdate()
    {
        if (!photonView.IsMine || CGameManager.isOptionOpen)
            return;

        //Quaternion rotateAmount = Camera.main.transform.rotation;
        //transform.localRotation = rotateAmount;
    }

    void RaycastCheck()
    {
        if (shotRateTimer >= shotRateTime)
        {
            if (Input.GetMouseButton(0))
            {
                photonView.RPC("RPCShootSound", RpcTarget.All);

                shotRateTimer = 0;

                //if (Camera.main.GetComponent<CCamera>().isOverlapGrowingCamera.Equals(false))
                //    Camera.main.GetComponent<CCamera>().doGrowingCamera = true;

                transform.root.GetComponent<Animator>().SetTrigger("Shoot");

                Ray cameraCenter = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

                RaycastHit hit;
                if (Physics.Raycast(cameraCenter, out hit))
                {
                    if (hit.Equals(null)) return;

                    photonView.RPC("RPCShootBullet", RpcTarget.All, hit.point, hit.normal);

                    if (hit.collider.CompareTag("Player"))
                    {
                        currentTransparency = 1;

                        int attackPlayer = transform.root.GetComponent<CPlayer>().playerNum;
                        CPlayer hitPlayer = hit.collider.GetComponent<CPlayer>();

                        CPlayerManager.Instance.ReduceHp(attackPlayer, hitPlayer.playerNum);
                    }
                }
            }
        }
    }
    void CrossHairHitSet()
    {
        Color setColor = crossHairHit.color;

        if (currentTransparency >= 0.0f)
            currentTransparency -= decreaseTransparency * Time.deltaTime;

        setColor.a = currentTransparency;
        crossHairHit.color = setColor;
    }

    [PunRPC]
    void RPCShootBullet(Vector3 hitPosition, Vector3 hitNormal)
    {
        int i = 0;
        foreach (GameObject effect in effctPool)
        {
            if (effect.activeSelf.Equals(false))
            {
                effect.SetActive(true);

                effect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hitNormal);
                effect.transform.position = hitPosition + transform.up * 0.1f;

                effect.GetComponent<ParticleSystem>().Play();
                StartCoroutine(DeleteEffect(effect));
                break;
            }
            else
            {
                i++;
            }
        }

        if (i.Equals(effctPool.Count))
        {
            GameObject tempObj = Instantiate(weaponEffect, Vector3.zero, Quaternion.identity);

            tempObj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hitNormal);
            tempObj.transform.position = hitPosition + transform.up * 0.1f;

            effctPool.Add(tempObj);
            StartCoroutine(DeleteEffect(tempObj));
        }
    }
    IEnumerator DeleteEffect(GameObject deleteEffect)
    {
        yield return new WaitForSeconds(2.5f);

        deleteEffect.SetActive(false);
    }

    [PunRPC]
    void RPCShootSound()
    {
        AudioSource audioRpc = gameObject.AddComponent<AudioSource>();
        audioRpc.clip = shotSound;
        audioRpc.spatialBlend = 1;
        audioRpc.minDistance = 25;
        audioRpc.maxDistance = 100;
        audioRpc.Play();
        Destroy(audioRpc, audioRpc.clip.length);
    }
}
