using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using DialogueEditor;

public class Warden : MonoBehaviour
{
    public static bool isAppear;

    public Transform p1;
    public Transform p2;
    public float length;
    public SpriteRenderer spriteRenderer;
    public GameObject LosePanel;

    private Animator animator;

    public NPCConversation conversation;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("StopWarden"))
        {
            animator.SetTrigger("Resting");
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Resting");
            isAppear = false;
            ConversationManager.Instance.StartConversation(conversation);
        }
    }

    public void PauseWalkSound()
    {
        WardenNoiseController.Instance.audioSource.Pause();
    }

    public void ShowLosePanel()
    {
        LosePanel.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("StopWarden"))
        {
            animator.SetTrigger("Wandering");
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.flipX = Stats.Instance.direction;


        length = Vector3.Distance(p1.position, p2.position);
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (isAppear)
        {
            transform.position = new Vector3(length * Stats.Instance.WardenTimeRatio + p1.position.x, p2.position.y);

            spriteRenderer.flipX = Stats.Instance.direction;
        }
    }
}