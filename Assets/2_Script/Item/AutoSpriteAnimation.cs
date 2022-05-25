using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpriteAnimation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public List<Sprite> listSprites;
    public float timer;

    private int index = 0;

    public void OnEnable()
    {
        index = 0;
        spriteRenderer.sprite = listSprites[index];
        StartCoroutine(UpdateAnimation());
    }

    // 스프라이트 모양 변화.
    IEnumerator UpdateAnimation()
    {
        while( true )
        {
            index++;
            if(index >= listSprites.Count)
                index = 0;

            yield return new WaitForSeconds(timer);

            spriteRenderer.sprite = listSprites[index];
        }
    }
    
}
