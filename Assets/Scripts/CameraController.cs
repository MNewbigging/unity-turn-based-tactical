using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
  private const float MIN_FOLLOW_Y_OFFSET = 2f;
  private const float MAX_FOLLOW_Y_OFFSET = 12f;
  [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
  private CinemachineTransposer cinemachineTransposer;
  private Vector3 targetFollowOffset;

  private void Start()
  {
    cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    targetFollowOffset = cinemachineTransposer.m_FollowOffset;
  }

  private void Update()
  {
    HandleMovement();
    HandleRotation();
    HandleZoom();
  }

  private void HandleMovement()
  {
    // Get move direction from WASD input
    Vector3 inputMoveDir = new Vector3(0, 0, 0);
    if (Input.GetKey(KeyCode.W))
    {
      inputMoveDir.z = +1f;
    }
    if (Input.GetKey(KeyCode.S))
    {
      inputMoveDir.z = -1f;
    }
    if (Input.GetKey(KeyCode.A))
    {
      inputMoveDir.x = -1f;
    }
    if (Input.GetKey(KeyCode.D))
    {
      inputMoveDir.x = +1f;
    }

    // Translate
    float moveSpeed = 10f;
    Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;
    transform.position += moveVector * moveSpeed * Time.deltaTime;
  }

  private void HandleRotation()
  {
    // Get rotation direction from QE input
    Vector3 rotationVector = new Vector3(0, 0, 0);
    if (Input.GetKey(KeyCode.Q))
    {
      rotationVector.y = -1f;
    }
    if (Input.GetKey(KeyCode.E))
    {
      rotationVector.y = +1f;
    }

    // Rotate
    float rotationSpeed = 100f;
    transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
  }

  private void HandleZoom()
  {
    // Get zoom direction from mouse wheel
    float zoomAmount = 1f;
    if (Input.mouseScrollDelta.y > 0f)
    {
      targetFollowOffset.y -= zoomAmount;
    }
    else if (Input.mouseScrollDelta.y < 0f)
    {
      targetFollowOffset.y += zoomAmount;
    }

    // Zoom
    float zoomSpeed = 5f;
    targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
    cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
  }
}
