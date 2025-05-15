using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    private Transform lookAt;
    public float boundX = 0.15f;
    public float boundY = 0.05f;

    private void Start()
    {
        lookAt = GameObject.Find("Player").transform;
    }


    /*private void LateUpdate()
    {
        // Automatically find the player if the reference is lost after scene load
        if (lookAt == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                lookAt = player.transform;
            }
            else
            {
                return; // No player found, skip camera update
            }
        }

        Vector3 delta = Vector3.zero;

        // X axis bounds
        float deltaX = lookAt.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            delta.x = (transform.position.x < lookAt.position.x)
                ? deltaX - boundX
                : deltaX + boundX;
        }

        // Y axis bounds
        float deltaY = lookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            delta.y = (transform.position.y < lookAt.position.y)
                ? deltaY - boundY
                : deltaY + boundY;
        }

        transform.position += new Vector3(delta.x, delta.y, 0);
    }*/

    private void LateUpdate()
    {
        if (lookAt == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                lookAt = player.transform;
            else
                return;
        }

        Vector3 delta = Vector3.zero;

        float deltaX = lookAt.position.x - transform.position.x;
        if (Mathf.Abs(deltaX) > boundX)
        {
            delta.x = (deltaX > 0) ? deltaX - boundX : deltaX + boundX;
        }

        float deltaY = lookAt.position.y - transform.position.y;
        if (Mathf.Abs(deltaY) > boundY)
        {
            delta.y = (deltaY > 0) ? deltaY - boundY : deltaY + boundY;
        }

        transform.position += new Vector3(delta.x, delta.y, 0);
    }





















    /*private void LateUpdate()
    {
        Vector3 delta = Vector3.zero;

        //This is to check if we're inside the bounds on the X axis
        float deltaX = lookAt.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            if (transform.position.x < lookAt.position.x)
            {
                delta.x = deltaX - boundX;
            }

            else
            {
                delta.x = deltaX + boundX;
            }
        }

        //This is to check if we're inside the bounds on the X axis
        float deltaY = lookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            if (transform.position.y < lookAt.position.y)
            {
                delta.y = deltaY - boundY;
            }

            else
            {
                delta.y = deltaY + boundY;
            }
        }

        transform.position += new Vector3(delta.x, delta.y, 0);
    }*/
}
