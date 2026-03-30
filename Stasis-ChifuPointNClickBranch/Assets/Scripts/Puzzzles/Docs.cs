using UnityEngine;

public class Docs : MonoBehaviour
{
    void Start()
    {
        if (Stats.Instance != null)
            gameObject.SetActive(!Stats.Instance.isDocsPicked);
    }

    public void PutDocs()
    {
        if (Stats.Instance != null)
            Stats.Instance.isDocsPicked = true;
        Destroy(gameObject);
    }
}
