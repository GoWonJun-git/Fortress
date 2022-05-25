using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D bulletRigidbody;
    public ParticleManager particleManager;
    private float angle;
    public float speed;
    public float startingSpeed;
    public GameObject explosion;

    // 생성 방향으로 포물선 이동.
    // 자연 효과 적용.
    void FixedUpdate() 
    {
        if (GetComponent<Rigidbody2D>() == null)
            return;
        
        angle = Mathf.Atan2(bulletRigidbody.velocity.y , bulletRigidbody.velocity.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        if (particleManager.particle_Wind.transform.position.x >= 0)
            bulletRigidbody.AddForce(Vector2.left  * 0.05f, ForceMode2D.Impulse);
        else
            bulletRigidbody.AddForce(Vector2.right * 0.05f, ForceMode2D.Impulse);

        float distance1 = Vector2.Distance(new Vector2(particleManager.particle_Up1.transform.position.x, 0), new Vector2(transform.position.x, 0));
        float distance2 = Vector2.Distance(new Vector2(particleManager.particle_Up2.transform.position.x, 0), new Vector2(transform.position.x, 0));
        if ( (distance1 <= 2 || distance2 <= 2) && transform.position.y <= 3)
            bulletRigidbody.AddForce(Vector2.up * 0.15f, ForceMode2D.Impulse);
    }

    // 미사일이 충돌할 시 폭발 로직 호출.
    void OnCollisionEnter2D(Collision2D other) 
    {
        if (!other.collider.CompareTag("Missile"))
            HitBullet(other);
    }

    // 폭포에 접촉시 약간 하강.
    void OnTriggerStay2D(Collider2D other) 
    {
        if (other.CompareTag("Water"))
            bulletRigidbody.AddForce(Vector2.down * 0.1f, ForceMode2D.Impulse);
    }

    // 미사일 폭발 로직.
    void HitBullet(Collision2D other)
    {
        if (other.collider.CompareTag("Ground") && other.gameObject.GetComponent<TerrainDestory>() != null)
            other.gameObject.GetComponent<TerrainDestory>().DestroyTerrain(transform.position, 2.5f);

        Destroy(Instantiate<GameObject>(explosion, transform.position, Quaternion.identity), 1f);
        ObjectPoolManager.Instance.Destroy(gameObject);
    }
    
    // 생성 후 일정 시간이 경과되면 제거.
    public IEnumerator DestroyBullet() 
    {
        yield return new WaitForSeconds(10f);
        ObjectPoolManager.Instance.Destroy(gameObject);
    }

}
