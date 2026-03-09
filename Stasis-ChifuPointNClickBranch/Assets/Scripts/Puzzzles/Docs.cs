using UnityEngine;

public class Docs : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(!Stats.Instance.isDocsPicked);
    }

    public void PutDocs()
    {
        Stats.Instance.isDocsPicked = true;
        Destroy(gameObject);
    }
}
