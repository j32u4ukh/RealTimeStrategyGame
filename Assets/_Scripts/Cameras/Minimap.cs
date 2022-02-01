using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform mimimap_rect = null;
    [SerializeField] private float map_scale = 10f;
    [SerializeField] private float offset = -6f;

    private Transform camera_transform;

    private void Update()
    {
        if(camera_transform == null)
        {
            // May cause "NullReferenceException: Object reference not set to an instance of an object"
            //NetworkIdentity identity = NetworkClient.connection.identity;

            if (NetworkClient.connection.identity != null)
            {
                camera_transform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().getCameraTransform();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        moveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        moveCamera();
    }

    private void moveCamera()
    {
        Vector2 mouse_pos = Mouse.current.position.ReadValue();

        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(mimimap_rect,
                                                                   mouse_pos,
                                                                   null,
                                                                   out Vector2 local_point))
        {
            Vector2 lerp = new Vector2((local_point.x - mimimap_rect.rect.x) / mimimap_rect.rect.width,
                                       (local_point.y - mimimap_rect.rect.y) / mimimap_rect.rect.height);

            Vector3 new_camera_pos = new Vector3(
                Mathf.Lerp(-map_scale, map_scale, lerp.x),
                camera_transform.position.y,
                Mathf.Lerp(-map_scale, map_scale, lerp.y) + offset
            );

            camera_transform.position = new_camera_pos;
        }
    }
}
