using UnityEngine;

public class WardenNoiseController : MonoBehaviour
{
    public static WardenNoiseController Instance { get; private set; }

    public Transform t1;
    public Transform t2;
    private Vector3 p1;
    private Vector3 p2;
    public float length;

    public AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        p1 = t1.position;
        p2 = t2.position;
        length = Vector3.Distance(p1, p2);

        if (Warden.isAppear)
        {
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (Warden.isAppear)
        {
            transform.position = new Vector3(length * Stats.Instance.WardenTimeRatio + p1.x, p2.y);
        }
    }
}
