using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCollider : MonoBehaviour
{
    
    void Start() => StartCoroutine(DestroyCollder());

    IEnumerator DestroyCollder()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(GetComponent<CircleCollider2D>());
    }
    
}
