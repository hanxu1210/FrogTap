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
		if (keyboard == KeyCode.JoystickButton0)
		{
			KeyCode DPAD_CENTER=(KeyCode)10;

			if (Input.GetKeyDown (DPAD_CENTER))
			{
				buttonMe.onClick.Invoke();
			}
			else if (Input.GetKeyDown(KeyCode.JoystickButton0))
			{
				buttonMe.onClick.Invoke();
			}
		}
		else
		{
			if (Input.GetKeyDown(keyboard))
			{
				buttonMe.onClick.Invoke();
			}
		}
    }

	static public bool IsOkButtonDown()
	{
		if (Input.GetKeyDown (KeyCode.JoystickButton0))
		{
			return true;
		}
		else if (Input.GetKeyDown((KeyCode)10))
		{
			return true;
		}
		return false;
	}
}