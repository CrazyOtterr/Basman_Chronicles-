using UnityEngine;

public class ConversationNPCSimple : MonoBehaviour //появление нпс для диалога
{
    [Header("Appearance")]
    public Sprite npcSprite;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        if (npcSprite != null)
            spriteRenderer.sprite = npcSprite;

        gameObject.SetActive(false);
    }

    public void Appear()
    {
        gameObject.SetActive(true);
    }


    public void Disappear()
    {
        gameObject.SetActive(false);
    }
}
