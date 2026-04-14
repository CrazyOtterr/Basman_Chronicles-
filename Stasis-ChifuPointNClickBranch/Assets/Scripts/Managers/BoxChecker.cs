using UnityEngine;

public class BoxChecker : MonoBehaviour
{
    void Start()
    {
        if (Stats.Instance != null)
            gameObject.SetActive(Stats.Instance.isDocsPicked);
    }

}
