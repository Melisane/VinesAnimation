using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFlyCam : MonoBehaviour
{

    public float flySpeed = 2;
    public float turnSpeed = 4;
    public bool freeCam = false;

    GameObject defaultCam;
    GameObject playerObject;
    bool isEnabled;

    bool shift;
    bool ctrl;
    float accelerationAmount = 30;
    float slowDownRatio = 3;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            freeCam = !freeCam;
        }

        if (freeCam)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                flySpeed /= slowDownRatio;

            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
                flySpeed *= slowDownRatio;

            //
            if (Input.GetAxis("Vertical") != 0)
            {
                transform.Translate(Vector3.forward * flySpeed * Input.GetAxis("Vertical"));
            }


            if (Input.GetAxis("Horizontal") != 0)
            {
                transform.Translate(Vector3.right * flySpeed * Input.GetAxis("Horizontal"));
            }

            if (Input.GetAxis("Mouse X") != 0)
            {
                transform.Rotate(Vector3.up * turnSpeed * Input.GetAxis("Mouse X"));
            }

            if (Input.GetAxis("Mouse Y") != 0)
            {
                transform.Rotate(Vector3.left * turnSpeed * Input.GetAxis("Mouse Y"));
            }


            if (Input.GetKey(KeyCode.E))
            {
                transform.Translate(Vector3.up * flySpeed);
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                transform.Translate(Vector3.down * flySpeed);
            }
        }
        

    }
}
