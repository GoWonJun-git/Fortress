using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController_Tutorial : MonoBehaviour
{
    public Image shieldCoolTime;
    public Image posionCoolTime;
    public Image freezingCoolTime;
    public Image moveBarHandleImage;
    public TutorialManager tutorialManager;
    public TutorialPlayerObject tutorialPlayerObject;

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
        if (!tutorialManager.isGameStart || tutorialPlayerObject.freezingImage.gameObject.activeSelf)
            return;

        moveBar.gameObject.SetActive(true);
        moveHandle.gameObject.SetActive(true);

        tutorialPlayerObject.isMoving = true;
        tutorialPlayerObject.playerSound.move.Play();

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
        if (!tutorialManager.isGameStart || tutorialPlayerObject.freezingImage.gameObject.activeSelf)
            return;

        moveBar.gameObject.SetActive(false);
        moveHandle.gameObject.SetActive(false);

        tutorialPlayerObject.deg_Move = 0f;
        tutorialPlayerObject.isMoving = false;
        tutorialPlayerObject.playerSound.move.Stop();

        if (tutorialPlayerObject.transform.position.x <= 0)
            tutorialPlayerObject.rigidbody.velocity = new Vector2(0f, tutorialPlayerObject.rigidbody.velocity.y);
    }

    // 이동 버튼 드래그 시.
    public void MoveButtonDrag()
    {
        if (!tutorialManager.isGameStart || tutorialPlayerObject.freezingImage.gameObject.activeSelf)
            return;

        if (Application.platform == RuntimePlatform.Android && Input.touchCount > 0)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTr, Input.touches[moveTouchCount].position, uiCamera, out moveDragPosition);
        else
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTr, Input.mousePosition, uiCamera, out moveDragPosition);

        if (moveDragPosition.x >= moveTauchPosition.x + 150)
            moveDragPosition = new Vector2(moveTauchPosition.x + 150, moveTauchPosition.y);
        else if (moveDragPosition.x <= moveTauchPosition.x - 150)
            moveDragPosition = new Vector2(moveTauchPosition.x - 150, moveTauchPosition.y);

        moveHandle.localPosition = new Vector2(moveDragPosition.x, moveTauchPosition.y);
        tutorialPlayerObject.deg_Move = moveDragPosition.x - moveTauchPosition.x;
    }

    // 점프 버튼 터치 시.
    public void JumpButton()
    {
        if (!tutorialManager.isGameStart || tutorialPlayerObject.freezingImage.gameObject.activeSelf)
            return;

        tutorialPlayerObject.Jump();
    }

    // 공격 버튼 터치 시.
    public void AttackButtonDown()
    {
        if (!tutorialManager.isGameStart || tutorialPlayerObject.freezingImage.gameObject.activeSelf)
            return;

        attackBar.gameObject.SetActive(true);
        attackHandle.gameObject.SetActive(true);

        tutorialPlayerObject.isCharging = true;
        tutorialPlayerObject.turret.SetActive(true);
        tutorialPlayerObject.chargeBar.gameObject.SetActive(true);
        tutorialPlayerObject.chargeArea.gameObject.SetActive(true);

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
        if (!tutorialManager.isGameStart || tutorialPlayerObject.freezingImage.gameObject.activeSelf)
            return;

        tutorialPlayerObject.Shooting();
        attackBar.gameObject.SetActive(false);
        attackHandle.gameObject.SetActive(false);
    }

    // 공격 버튼 드래그 시.
    public void AttackButtonDrag()
    {
        if (!tutorialManager.isGameStart || tutorialPlayerObject.freezingImage.gameObject.activeSelf)
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
        tutorialPlayerObject.deg_Attack = (AttactDragPosition.y - AttactTauchPosition.y) / 2;

        if (AttactDragPosition.x < AttactTauchPosition.x && !tutorialPlayerObject.spriteRenderer.flipX)
            tutorialPlayerObject.ChangeFlipX();
        else if (AttactDragPosition.x > AttactTauchPosition.x && tutorialPlayerObject.spriteRenderer.flipX)
            tutorialPlayerObject.ChangeFlipX();
    }
    
    // 쉴드 스킬 사용 시.
    public void ShieldSkill()
    {
        if (!tutorialManager.isGameStart)
            return;

        if (shieldCoolTime.fillAmount < 1f)
        {
            tutorialManager.soundManager.buttonFail.Play();
            return;
        }

        shieldCoolTime.fillAmount = 0f;
        tutorialPlayerObject.ItemType1();
        tutorialManager.soundManager.buttonTouch.Play();
    }

    // 포션 스킬 사용 시.
    public void PosionSkill()
    {
        if (!tutorialManager.isGameStart)
            return;

        if (posionCoolTime.fillAmount < 1f)
        {
            tutorialManager.soundManager.buttonFail.Play();
            return;
        }

        posionCoolTime.fillAmount = 0f;
        tutorialPlayerObject.UpGradeType3(0.3f);
        tutorialManager.soundManager.buttonTouch.Play();
    }

    // 무적 스킬 사용 시.
    public void FreezingSkill()
    {
        if (!tutorialManager.isGameStart)
            return;
    
        if (freezingCoolTime.fillAmount < 1f)
        {
            tutorialManager.soundManager.buttonFail.Play();
            return;
        }

        StartCoroutine(FreezingTimer());
        freezingCoolTime.fillAmount = 0f;
        tutorialPlayerObject.isMoving = false;
        tutorialManager.soundManager.buttonTouch.Play();
    }

    // 10초간 무적 부여.
    IEnumerator FreezingTimer()
    {
        tutorialPlayerObject.freezingImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(10f);
        tutorialPlayerObject.freezingImage.gameObject.SetActive(false);
    }

}