using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeyboardHandler : MonoBehaviour
{
    public KeyCode keyboard = KeyCode.JoystickButton0;

    Button buttonMe;
    
    void Start () {
        buttonMe = GetComponent<Button>();
    }
    
    void LateUpdate() {
#if DEBUG_VERSION
        if (Input.GetKeyDown(KeyCode.A))
        {
            buttonMe.onClick.Invoke();
        }
#endif

        if (Input.GetKeyDown(keyboard))
        {
            buttonMe.onClick.Invoke();
        }
    }
}