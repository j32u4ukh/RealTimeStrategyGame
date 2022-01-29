using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building = null;
    [SerializeField] private Image icon_image = null;
    [SerializeField] private TMP_Text price_text = null;
    [SerializeField] private LayerMask floor_layer = new LayerMask();

    private Camera main_camera;
    private RTSPlayer player;
    private GameObject building_preview_instance;
    private Renderer building_render_instance;

    private void Start()
    {
        main_camera = Camera.main;
        icon_image.sprite = building.getIcon();
        price_text.text = building.getPrice().ToString();
    }

    private void Update()
    {
        // TODO: Build player object at the scene before current scene to avoid NullReferenceException.
        if (player == null)
        {
            try
            {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            }
            catch (NullReferenceException)
            {

            }
        }

        if (building_preview_instance != null)
        {
            updateBuildingPreview();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            building_preview_instance = Instantiate(building.getBuildingPreview());
            building_render_instance = building_preview_instance.GetComponentInChildren<Renderer>();
            building_preview_instance.SetActive(false);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (building_preview_instance != null)
        {
            Ray ray = main_camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask: floor_layer))
            {
                // place building
                player.cmdTryPlaceBuilding(building_id: building.getId(), location: hit.point);
            }

            Destroy(building_preview_instance);
        }
    }

    private void updateBuildingPreview()
    {
        Ray ray = main_camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask: floor_layer))
        {
            building_preview_instance.transform.position = hit.point;

            if (!building_preview_instance.activeSelf)
            {
                building_preview_instance.SetActive(true);
            }
        }
    }
}
