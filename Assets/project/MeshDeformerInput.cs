using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float force = 1000f;
    public float forceOffset = 0.1f;
    public float local_radius_min = 0.002f;
    public float local_radius_max = 0.004f;

    public float sensitivity = 3;
    [SerializeField] private Transform targetedObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput(1);
        }
        if (Input.GetMouseButtonDown(1))
        {
            GrabTarget();
        }

        if (Input.GetMouseButtonUp(1))
        {
            targetedObject = null;
        }

        if (targetedObject == null) return;
        targetedObject.transform.rotation *= Quaternion.Euler(Input.GetAxisRaw("Mouse Y") * sensitivity,Input.GetAxisRaw("Mouse X") * -sensitivity, 0);
    }

    void VolumeCheck()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            meshDeformer deformer = hit.collider.GetComponent<meshDeformer>();
            deformer.TellMeVolume();
        }
    }

    void HandleInput(int direction)
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            meshDeformer deformer = hit.collider.GetComponent<meshDeformer>();
            if (deformer)
            {
                Vector3 point = hit.point;
                point += hit.normal * (forceOffset * direction);
                deformer.AddDeformingForce(point, force, Random.Range(local_radius_min,  local_radius_max));
                //deformer.AddDeformingForce();
            }
        }
    }

    void GrabTarget()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            targetedObject = hit.transform;
            Debug.Log("got " + targetedObject.name);
        }
    }
}
