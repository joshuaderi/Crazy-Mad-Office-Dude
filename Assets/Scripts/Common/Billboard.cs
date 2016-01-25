using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    private Transform thisTransform = null;

    void Start()
    {
        //Cache transform
        thisTransform = transform;
    }

    void LateUpdate()
    {
        //Billboard sprite
        Vector3 cameraPos = Camera.main.transform.position;
        
        float deltaX = thisTransform.position.x - cameraPos.x;
        float deltaZ = thisTransform.position.z - cameraPos.z;

        Vector3 lookAtDir = new Vector3(deltaX, 0, deltaZ).normalized;
        thisTransform.rotation = Quaternion.LookRotation(lookAtDir, Vector3.up);
    }
}
