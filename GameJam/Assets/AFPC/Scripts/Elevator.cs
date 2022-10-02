using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private bool isActive;
    private bool up = true;
    private bool down = false;

    void Update()
    {
        isActive = ProjectManager.instance.isElevatorActive;


        if (isActive)
        {
            if (up)
            {
                transform.position += new Vector3(0, 5 * Time.deltaTime, 0);
            }
            if (down)
            {
                transform.position -= new Vector3(0, 5 * Time.deltaTime, 0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ElevatorStop"))
        {
            isActive = false;
            ProjectManager.instance.isElevatorActive = isActive;
            if (up)
            {
                up = false;
                down = true;
            }
            else
            {
                up = true;
                down = false;
            }
            Debug.Log(up);
        }
    }
}
