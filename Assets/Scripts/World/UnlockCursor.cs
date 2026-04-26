using UnityEngine;

public class UnlockCursor : MonoBehaviour
{
    void Start()
    {
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Time.timeScale = 1f;
    }
}