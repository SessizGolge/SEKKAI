using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float sprintMultiplier = 1.5f;
    [SerializeField] float sprintAnimSpeed = 1.5f;

    [Header("Costume & Animator")]
    [SerializeField] Animator playerAnimator;
    [SerializeField] CostumeDatabase database;
    [SerializeField] private SpriteRenderer topVFX;
    [SerializeField] private SpriteRenderer torsoVFX;
    [SerializeField] private SpriteRenderer bottomVFX;

    Rigidbody2D rb;
    Vector2 moveInput;
    AnimatorOverrideController overrideController;
    PhotonView view;

    int topIndex, torsoIndex, bottomIndex;

    bool canAttack = true;
    float attackCooldown = 0.4f;

    Vector3 networkPosition;
    Quaternion networkRotation;
    float netMoveX, netMoveY;
    bool netIsMoving;

    IEnumerator Start()
    {
        playerAnimator.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        overrideController = new AnimatorOverrideController(playerAnimator.runtimeAnimatorController);
        playerAnimator.runtimeAnimatorController = overrideController;

        if (view.IsMine)
        {
            var props = PhotonNetwork.LocalPlayer.CustomProperties;
            topIndex = props.ContainsKey("TopIndex") ? (int)props["TopIndex"] : 0;
            torsoIndex = props.ContainsKey("TorsoIndex") ? (int)props["TorsoIndex"] : 0;
            bottomIndex = props.ContainsKey("BottomIndex") ? (int)props["BottomIndex"] : 0;
        }
        else
        {
            var ownerProps = view.Owner.CustomProperties;
            topIndex = ownerProps.ContainsKey("TopIndex") ? (int)ownerProps["TopIndex"] : 0;
            torsoIndex = ownerProps.ContainsKey("TorsoIndex") ? (int)ownerProps["TorsoIndex"] : 0;
            bottomIndex = ownerProps.ContainsKey("BottomIndex") ? (int)ownerProps["BottomIndex"] : 0;
        }

        gameObject.name = view.Owner.NickName;

        ApplyCostumeAnimationsAndSprites();

        networkPosition = transform.position;
        networkRotation = transform.rotation;

        yield return null;
    }

    void Update()
    {
        if (view.IsMine)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            bool isMoving = moveInput != Vector2.zero;
            bool isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift);

            playerAnimator.SetBool("Walking", isMoving);
            playerAnimator.SetBool("Idle", !isMoving);
            playerAnimator.speed = isSprinting ? sprintAnimSpeed : 1f;

            if (moveInput.x > 0.01f)
                transform.localScale = new Vector3(3, 3, 1);
            else if (moveInput.x < -0.01f)
                transform.localScale = new Vector3(-3, 3, 1);

            if (Input.GetMouseButtonDown(0) && canAttack)
                Attack();

            // Kostüm değiştirme tuşları
            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeCostumeIndex(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeCostumeIndex(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeCostumeIndex(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeCostumeIndex(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeCostumeIndex(4);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);

            playerAnimator.SetBool("Walking", netIsMoving);
            playerAnimator.SetBool("Idle", !netIsMoving);
            playerAnimator.SetFloat("MoveX", netMoveX);
            playerAnimator.SetFloat("MoveY", netMoveY);
        }
    }

    void FixedUpdate()
    {
        if (!view.IsMine) return;
        float currentSpeed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        rb.velocity = moveInput * currentSpeed;
    }

    private void ChangeCostumeIndex(int newIndex)
    {
        PlayerPrefs.SetInt("TopIndex", newIndex);
        PlayerPrefs.SetInt("TorsoIndex", newIndex);
        PlayerPrefs.SetInt("BottomIndex", newIndex);

        ExitGames.Client.Photon.Hashtable costumeProps = new ExitGames.Client.Photon.Hashtable();
        costumeProps["TopIndex"] = newIndex;
        costumeProps["TorsoIndex"] = newIndex;
        costumeProps["BottomIndex"] = newIndex;
        PhotonNetwork.LocalPlayer.SetCustomProperties(costumeProps);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("TopIndex") || changedProps.ContainsKey("TorsoIndex") || changedProps.ContainsKey("BottomIndex"))
        {
            if (targetPlayer == view.Owner)
            {
                int newTop = changedProps.ContainsKey("TopIndex") ? (int)changedProps["TopIndex"] : topIndex;
                int newTorso = changedProps.ContainsKey("TorsoIndex") ? (int)changedProps["TorsoIndex"] : torsoIndex;
                int newBottom = changedProps.ContainsKey("BottomIndex") ? (int)changedProps["BottomIndex"] : bottomIndex;

                topIndex = newTop;
                torsoIndex = newTorso;
                bottomIndex = newBottom;

                StartCoroutine(ApplyCostumeNextFrame(newTop, newTorso, newBottom));
            }
        }
    }

    private IEnumerator ApplyCostumeNextFrame(int newTop, int newTorso, int newBottom)
    {
        yield return null;
        var cm = GetComponent<PlayerCostumeManager>();
        if(cm != null)
            cm.SetCostume(newTop, newTorso, newBottom);
    }

    void ApplyCostumeAnimationsAndSprites()
    {
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(database.topWalkClips[0], database.topWalkClips[topIndex]));
        overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(database.torsoWalkClips[0], database.torsoWalkClips[torsoIndex]));
        overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(database.bottomWalkClips[0], database.bottomWalkClips[bottomIndex]));

        if (database.topAttackClips.Count > topIndex)
            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(database.topAttackClips[0], database.topAttackClips[topIndex]));
        if (database.torsoAttackClips.Count > torsoIndex)
            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(database.torsoAttackClips[0], database.torsoAttackClips[torsoIndex]));

        overrideController.ApplyOverrides(overrides);

        StartCoroutine(ApplySpritesNextFrame());
    }

    IEnumerator ApplySpritesNextFrame()
    {
        topVFX.sprite = database.tops[topIndex];
        torsoVFX.sprite = database.torsos[torsoIndex];
        bottomVFX.sprite = database.bottoms[bottomIndex];
        yield return null;
        playerAnimator.enabled = true;
    }

    void Attack()
    {
        playerAnimator.SetTrigger("Attack");
        view.RPC("RPC_PlayAttack", RpcTarget.Others);
        StartCoroutine(AttackCooldownRoutine());
    }

    [PunRPC]
    void RPC_PlayAttack()
    {
        playerAnimator.SetTrigger("Attack");
    }

    IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && view.IsMine)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale.x);

            stream.SendNext(moveInput.x);
            stream.SendNext(moveInput.y);
            stream.SendNext(moveInput.sqrMagnitude > 0.1f);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            float scaleX = (float)stream.ReceiveNext();
            transform.localScale = new Vector3(scaleX, 3f, 1f);

            netMoveX = (float)stream.ReceiveNext();
            netMoveY = (float)stream.ReceiveNext();
            netIsMoving = (bool)stream.ReceiveNext();
        }
    }
}
