using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnC_Player : MonoBehaviour
{
    public static PnC_Player inst { get; private set; }
    [field: SerializeField] public PnC_PlayerController controller { get; private set; }

    private void Awake() 
    {
        if (inst != null && inst != this) 
        {
            Destroy(this);
        } 
        else 
        {
            inst = this;
        }
    }
}
