using UnityEngine;

public class CoolScript : MonoBehaviour
{
    public bool isSettingActive = false;
    public CustomObject objectToInteractWith;
    void Start()
    {
        if (isSettingActive)
            Debug.Log($"option is set to {gameObject.name}", gameObject);
    }
}
