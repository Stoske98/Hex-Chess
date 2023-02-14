using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Camera cam;
    public float rotate_speed;
    float X;
    public float rotate_min;
    public float rotate_max;
    public float zoom_min;
    public float zoom_max;
    public float scrollSensitivity = 1f;
    private Transform root;
    private Transform pivot;
    private Transform target;
    private float angle;

    void Awake()
    {
        root = new GameObject("CameraHelper").transform;
        pivot = new GameObject("CameraPivot").transform;
        target = new GameObject("CameraTarget").transform;
    }
    private void Start()
    {
        Initialized(Vector3.zero, 90);
    }
    private void Update()
    {
        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt))
        {
            pivot.transform.Rotate(Input.GetAxis("Mouse Y") * rotate_speed, 0,0);
            X = pivot.transform.rotation.eulerAngles.x;
            if (X > rotate_max)
                X = rotate_max;
            else if (X < rotate_min)
                X = rotate_min;
            pivot.transform.rotation = Quaternion.Euler(X, 0, 0);

            // GameUiManager.Instance.UpdateHeroBarPosition();
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (target.localPosition.z + scrollSensitivity < zoom_min)
                target.localPosition = new Vector3(0, 0, target.localPosition.z + scrollSensitivity);

            // GameUiManager.Instance.UpdateHeroBarPosition();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (target.localPosition.z - scrollSensitivity > zoom_max)
                target.localPosition = new Vector3(0, 0, target.localPosition.z - scrollSensitivity);
        }
    }
    private void Initialized(Vector3 centar, float _angle)
    {
        angle = _angle;

        pivot.SetParent(root);
        target.SetParent(pivot);
        cam.transform.SetParent(target);

        root.position = centar;
        root.localEulerAngles = Vector3.zero;

        pivot.localPosition = Vector3.zero;
        pivot.localEulerAngles = new Vector3(angle,0,0);

        target.localPosition = new Vector3(0,0,-20);
        target.localEulerAngles = Vector3.zero;

        cam.transform.localPosition = Vector3.zero;
        cam.transform.localEulerAngles = Vector3.zero;
    }
    public void SetCamera(ClassType class_type)
    {
        if(class_type == ClassType.Light)
        {
            root.position = new Vector3(0,0,-2.0f);
            root.localEulerAngles = Vector3.zero;

            pivot.localPosition = Vector3.zero;
            pivot.localEulerAngles = new Vector3(60, 0, 0);

            target.localPosition = new Vector3(0, 0, -17);
            target.localEulerAngles = Vector3.zero;

            cam.transform.localPosition = Vector3.zero;
            cam.transform.localEulerAngles = Vector3.zero;
        }
        else
        {

            root.position = new Vector3(0, 0, 2.0f);
            root.localEulerAngles = Vector3.zero;

            pivot.localPosition = Vector3.zero;
            pivot.localEulerAngles = new Vector3(60, 180, 0);

            target.localPosition = new Vector3(0, 0, -17);
            target.localEulerAngles = Vector3.zero;

            cam.transform.localPosition = Vector3.zero;
            cam.transform.localEulerAngles = Vector3.zero;
        }
    }
}
