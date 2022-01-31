using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform player_camera_transform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screen_border_thickness = 10f;
    [SerializeField] private Vector2 screen_x_limits = Vector2.zero;
    [SerializeField] private Vector2 screen_z_limits = Vector2.zero;

    private Controls controls;
    private Vector2 previous_input;

    public override void OnStartAuthority()
    {
        player_camera_transform.gameObject.SetActive(true);
        controls = new Controls();
        controls.Player.MoveCamera.performed += setPreviousInput;
        controls.Player.MoveCamera.canceled += setPreviousInput;
        controls.Enable();
    }

    [ClientCallback]

    private void Update()
    {
        if (hasAuthority && Application.isFocused)
        {
            updateCameraPosition();
        }
    }

    private void updateCameraPosition()
    {
        Vector3 pos = player_camera_transform.position;

        // No keyboard input
        if(previous_input == Vector2.zero)
        {
            Vector3 cursor_movement = Vector3.zero;
            Vector2 cursor_position = Mouse.current.position.ReadValue();

            if(cursor_position.x >= Screen.width - screen_border_thickness)
            {
                cursor_movement.x += 1;
            }
            else if(cursor_position.x <= screen_border_thickness)
            {
                cursor_movement.x -= 1;
            }

            if(cursor_position.y >= Screen.height - screen_border_thickness)
            {
                cursor_movement.z += 1;
            }
            else if(cursor_position.y <= screen_border_thickness)
            {
                cursor_movement.z -= 1;
            }

            pos += cursor_movement.normalized * speed * Time.deltaTime;
        }
        else
        {
            pos += new Vector3(previous_input.x, 0f, previous_input.y) * speed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, screen_x_limits.x, screen_x_limits.y);
        pos.z = Mathf.Clamp(pos.z, screen_z_limits.x, screen_z_limits.y);

        player_camera_transform.position = pos;
    }

    private void setPreviousInput(InputAction.CallbackContext ctx)
    {
        previous_input = ctx.ReadValue<Vector2>();
    }
}
