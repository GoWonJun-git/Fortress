using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Image shieldCoolTime;
    public Image posionCoolTime;
    public Image freezingCoolTime;
    public GameManager gameManager;
    public Image moveBarHandleImage;
    public PlayerManager playerManager;

    private int moveTouchCount;
    private int attackTouchCount;
    private Vector2 moveTauchPosition;
    private Vector2 moveDragPosition;
    private Vector2 AttactTauchPosition;
    private Vector2 AttactDragPosition;

    public Camera uiCamera;
    public RectTransform moveBar;
    public RectTransform moveHandle;
    public RectTransform attackBar;
    public RectTransform attackHandle;
    public RectTransform targetRectTr;

    // 스킬 쿨타임 관리.
    void Update()
    {
        if (playerManager.myPlayerObject == null)
            return; 

        if (shieldCoolTime.fillAmount <= 1)
            shieldCoolTime.fillAmount += Time.deltaTime / 20;
        if (posionCoolTime.fillAmount <= 1)
            posionCoolTime.fillAmount += Time.deltaTime / 20;
        if (freezingCoolTime.fillAmount <= 1)
            freezingCoolTime.fillAmount += Time.deltaTime / 30;
    }
    
    // 이동 버튼 터치 시.
    public void MoveButtonDown() 
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd || playerManager.myPlayerObject.freezingImage.gameObject.activeSelf)
            return;
    
        moveBar.gameObject.SetActive(true);
        moveHandle.gameObject.SetActive(true);

        playerManager.myPlayerObject.isMoving = true;
        playerManager.myPlayerObject.playerSound.move.Play();

        if (Application.platform.Equals(RuntimePlatform.Android))
        {
            moveTouchCount = Input.touches.Length - 1;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTr, Input.touches[moveTouchCount].position, uiCamera, out moveTauchPosition);
        }
        else
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTr, Input.mousePosition, uiCamera, out moveTauchPosition);

        moveBar.localPosition = moveTauchPosition;
        moveHandle.localPosition = moveTauchPosition;
    }

    // 이동 버튼 터치 해제 시.
    public void MoveButtonUp() 
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd || playerManager.myPlayerObject.freezingImage.gameObject.activeSelf)
            return;
    
        moveBar.gameObject.SetActive(false);
        moveHandle.gameObject.SetActive(false);

        playerManager.myPlayerObject.deg_Move = 0f;
        playerManager.myPlayerObject.isMoving = false;
        playerManager.myPlayerObject.playerSound.move.Stop();
        
        if (playerManager.myPlayerObject.transform.position.x <= 0)
            playerManager.myPlayerObject.rigidbody.velocity = new Vector2(0f, playerManager.myPlayerObject.rigidbody.velocity.y);
    }

    // 이동 버튼 드래그 시.
    public void MoveButtonDrag()
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd || playerManager.myPlayerObject.freezingImage.gameObject.activeSelf)
            return;

        if (Application.platform.Equals(RuntimePlatform.Android) && Input.touchCount > 0)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTr, Input.touches[moveTouchCount].position, uiCamera, out moveDragPosition);
        else
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTr, Input.mousePosition, uiCamera, out moveDragPosition);

        if (moveDragPosition.x >= moveTauchPosition.x + 150)
            moveDragPosition = new Vector2(moveTauchPosition.x + 150, moveTauchPosition.y);
        else if (moveDragPosition.x <= moveTauchPosition.x - 150)
            moveDragPosition = new Vector2(moveTauchPosition.x - 150, moveTauchPosition.y);

        moveHandle.localPosition = new Vector2(moveDragPosition.x, moveTauchPosition.y);
        playerManager.myPlayerObject.deg_Move = moveDragPosition.x - moveTauchPosition.x;
    }

    // 점프 버튼 터치 시.
    public void JumpButton()
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd || playerManager.myPlayerObject.freezingImage.gameObject.activeSelf)
            return;
    
        playerManager.myPlayerObject.Jump();
    }

    // 공격 버튼 터치 시.
    public void AttackButtonDown()
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd || playerManager.myPlayerObject.freezingImage.gameObject.activeSelf)
            return;
    
        attackBar.gameObject.SetActive(true);
        attackHandle.gameObject.SetActive(true);
        playerManager.myPlayerObject.isCharging = true;

        if (Application.platform.Equals(RuntimePlatform.Android))
        {
            attackTouchCount = Input.touches.Length - 1;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTr, Input.touches[attackTouchCount].position, uiCamera, out AttactTauchPosition);
        }
        else
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTr, Input.mousePosition, uiCamera, out AttactTauchPosition);

        attackBar.localPosition = AttactTauchPosition;
        attackHandle.localPosition = AttactTauchPosition;
    }

    // 공격 버튼 터치 해제 시.
    public void AttackButtonUp()
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd || playerManager.myPlayerObject.freezingImage.gameObject.activeSelf)
            return;
    
        attackBar.gameObject.SetActive(false);
        attackHandle.gameObject.SetActive(false);
        playerManager.myPlayerObject.Shooting();
    }

    // 공격 버튼 드래그 시.
    public void AttackButtonDrag()
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd || playerManager.myPlayerObject.freezingImage.gameObject.activeSelf)
            return;
    
        if (Application.platform == RuntimePlatform.Android && Input.touchCount > 0)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTr, Input.touches[attackTouchCount].position, uiCamera, out AttactDragPosition);
        else
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTr, Input.mousePosition, uiCamera, out AttactDragPosition);

        if (AttactDragPosition.y >= AttactTauchPosition.y + 100)
            AttactDragPosition = new Vector2(AttactTauchPosition.x, AttactTauchPosition.y + 100);
        else if (AttactDragPosition.y <= AttactTauchPosition.y - 100)
            AttactDragPosition = new Vector2(AttactTauchPosition.x, AttactTauchPosition.y - 100);

        attackHandle.localPosition = new Vector2(AttactTauchPosition.x, AttactDragPosition.y);
        playerManager.myPlayerObject.deg_Attack = (AttactDragPosition.y - AttactTauchPosition.y) / 2;

        if (AttactDragPosition.x < AttactTauchPosition.x && !playerManager.myPlayerObject.spriteRenderer.flipX)
            playerManager.myPlayerObject.ChangeFlipX();
        else if (AttactDragPosition.x > AttactTauchPosition.x && playerManager.myPlayerObject.spriteRenderer.flipX)
            playerManager.myPlayerObject.ChangeFlipX();
    }
    
    // 쉴드 스킬 사용 시.
    public void ShieldSkill()
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd)
            return;
    
        if (shieldCoolTime.fillAmount < 1f)
        {
            gameManager.soundManager.buttonFail.Play();
            return;
        }

        shieldCoolTime.fillAmount = 0f;
        playerManager.myPlayerObject.GetItem(1);
        gameManager.soundManager.buttonTouch.Play();
    }

    // 포션 스킬 사용 시.
    public void PosionSkill()
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd)
            return;
    
        if (posionCoolTime.fillAmount < 1f)
        {
            gameManager.soundManager.buttonFail.Play();
            return;
        }

        posionCoolTime.fillAmount = 0f;
        playerManager.myPlayerObject.UpGrade(3, 0);
        gameManager.soundManager.buttonTouch.Play();
    }
    
    // 무적 스킬 사용 시.
    public void FreezingSkill()
    {
        if (!gameManager.isGameStart || gameManager.isGameEnd)
            return;
    
        if (freezingCoolTime.fillAmount < 1f)
        {
            gameManager.soundManager.buttonFail.Play();
            return;
        }

        StartCoroutine(FreezingTimer());
        freezingCoolTime.fillAmount = 0f;
        gameManager.soundManager.buttonTouch.Play();
        playerManager.myPlayerObject.isMoving = false;
    }

    // 10초간 무적 부여.
    IEnumerator FreezingTimer()
    {
        playerManager.myPlayerObject.ChangeFreezing();

        yield return new WaitForSeconds(10f);
        playerManager.myPlayerObject.ChangeFreezing();
    }


}
