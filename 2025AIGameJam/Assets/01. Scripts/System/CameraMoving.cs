using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public float dragSpeed = 2f;     // 드래그 민감도
    public float minX = -10f;        // 카메라 이동 최소 x
    public float maxX = 10f;         // 카메라 이동 최대 x

    public bool canDrag = true;

    private Vector3 lastMousePos;

    private void Awake()
    {
        Camera.main.transform.position = new Vector3(maxX, 0, -10);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 누를 때
        {
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && canDrag) // 마우스를 누르고 있는 동안
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            float moveX = -delta.x * dragSpeed * Time.deltaTime;

            Vector3 newPos = transform.position + new Vector3(moveX, 0, 0);
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX); // 카메라 이동 범위 제한

            transform.position = newPos;
            lastMousePos = Input.mousePosition;
        }
    }
}
