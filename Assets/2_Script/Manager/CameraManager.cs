using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject player;
    public GameObject target;

    [Header("Camera")]
    private float cameraMoveSpeed;
    private Vector2 offset;
    private float cameraWidth;
    private float cameraHeight;
    public float minX, maxX, minY, maxY;

    void Start() 
    {
        target = null;
        cameraWidth = Camera.main.aspect * Camera.main.orthographicSize;
        cameraHeight = Camera.main.orthographicSize;
        cameraMoveSpeed = 5f;
    }
    
    // 카메라의 추적 대상 변경.
    public void ChangeTarget(GameObject _target) 
    {
        target = _target;
        StopCoroutine("ResetTarget");
        StartCoroutine("ResetTarget");
    }

    // 추적 대상이 변경된 경우(미사일을 발사한 경우) 일정 시간 이후 기존 추적 대상으로 타겟 변경.
    IEnumerator ResetTarget()
    {
        yield return new WaitForSeconds(0.5f);
        target = player;
    }

    // 타겟 추적. 맵 제한.
    void LateUpdate() 
    {
        if (target == null)
            return;

        if (gameManager != null && gameManager.photonObject.roomType == "PVP" && gameManager.waveCount < 5)
        {
            minX += Time.deltaTime / 2;
            maxX -= Time.deltaTime / 2;
        }

        if (!target.activeSelf)
            target = player;

        if (target.GetComponent<PlayerObject>() != null)
            offset.y = 4.5f;
        else
            offset.y = 0f;

        Vector3 desiredPosition = new Vector3(
                    Mathf.Clamp(target.transform.position.x + offset.x, minX + cameraWidth, maxX - cameraWidth),
                    Mathf.Clamp(target.transform.position.y + offset.y, minY + cameraHeight, maxY - cameraHeight),
                    -10);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * cameraMoveSpeed);
    }

}
