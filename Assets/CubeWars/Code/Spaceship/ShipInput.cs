using UnityEngine;

public class ShipInput : MonoBehaviour
{
    public bool isPlayer = false;

    public Vector3 stickAndRudder;
    [Range(0.0f, 1.0f)]
    public float throttle;

    public bool fire = false;

    private void Update()
    {
        if (isPlayer)
        {
            ProcessStickInputs();
            ProcessThrottleInputs();
            ProcessWeaponInputs();
        }
        else
        {
            throttle = 0.33f;
        }
    }

    private void ProcessWeaponInputs()
    {
        fire = Input.GetButton("Fire1");
    }

    private void ProcessStickInputs()
    {
        stickAndRudder.x = Input.GetAxis("Vertical");
        stickAndRudder.y = Input.GetAxis("Horizontal");
        stickAndRudder.z = -Input.GetAxis("Horizontal");
    }

    private void ProcessThrottleInputs()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
            throttle = 1.0f;
        else if (Input.GetKeyDown(KeyCode.Backslash))
            throttle = 0.0f;
        else if (Input.GetKeyDown(KeyCode.LeftBracket))
            throttle = 0.33f;
        else if (Input.GetKeyDown(KeyCode.RightBracket))
            throttle = 0.66f;
    }
}