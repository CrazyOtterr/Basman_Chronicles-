using UnityEngine;

public class MapAppear : MonoBehaviour
{
    void Start()
    {
        if (Stats.Instance != null)
            gameObject.SetActive(!Stats.Instance.isMapSolved);
    }

}
