using UnityEngine;

public class LockInteract : MonoBehaviour
{
    public CodeLock codeLock;
    
    void OnMouseDown()
    {
        if (codeLock != null)
            codeLock.OpenPanel();
    }
}