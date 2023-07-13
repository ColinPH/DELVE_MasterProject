using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMover : MonoBehaviour
{
    public Vector3 moveVector = new Vector3(1f, 0f, 0f);
    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
    }
}