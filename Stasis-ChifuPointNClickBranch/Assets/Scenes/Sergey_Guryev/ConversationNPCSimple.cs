using UnityEngine;

public class ConversationNPCSimple : MonoBehaviour //спрайт собеседника появляется во время диалога
{
    [Header("Appearance")]
    public Sprite npcSprite;
    public float disappearDelay;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        if (npcSprite != null)
            spriteRenderer.sprite = npcSprite;

        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;
    }

    public void Appear()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
    }

    public void Disappear()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;
    }

    public void DisappearWithDelay()
    {
        Invoke(nameof(Disappear), disappearDelay);
    }
}
