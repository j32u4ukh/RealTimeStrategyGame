using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] RectTransform unit_selection_area = null;
    [SerializeField] private LayerMask layer_mask = new LayerMask();

    public List<Unit> selected_units { get; } = new List<Unit>();

    private RTSPlayer player;
    private Camera main_camera;
    private Vector2 start_position;

    // Start is called before the first frame update
    void Start()
    {
        main_camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
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

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            startSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            updateSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            clearSelectionArea();
        }
    }

    private void startSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selected_unit in selected_units)
            {
                selected_unit.deselect();
            }

            selected_units.Clear();
        }

        unit_selection_area.gameObject.SetActive(true);
        start_position = Mouse.current.position.ReadValue();
        updateSelectionArea();
    }

    private void updateSelectionArea()
    {
        Vector2 mouse_position = Mouse.current.position.ReadValue();

        float x_offset = mouse_position.x - start_position.x;
        float y_offset = mouse_position.y - start_position.y;
        
        unit_selection_area.sizeDelta = new Vector2(Mathf.Abs(x_offset), Mathf.Abs(y_offset));
        unit_selection_area.anchoredPosition = start_position + new Vector2(x_offset / 2f, y_offset / 2f);
    }

    private void clearSelectionArea()
    {
        unit_selection_area.gameObject.SetActive(false);

        if(unit_selection_area.sizeDelta.magnitude == 0)
        {
            // Input.mousePosition -> Mouse.current.position.ReadValue()
            Ray ray = main_camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer_mask))
            {
                if (!hit.collider.TryGetComponent<Unit>(out Unit unit))
                {
                    return;
                }

                if (!unit.hasAuthority)
                {
                    return;
                }

                if (selected_units.Contains(unit))
                {
                    return;
                }

                selected_units.Add(unit);
            }
        }
        else
        {
            Vector2 min = unit_selection_area.anchoredPosition - (unit_selection_area.sizeDelta / 2f);
            Vector2 max = unit_selection_area.anchoredPosition + (unit_selection_area.sizeDelta / 2f);

            foreach(Unit unit in player.getUnits())
            {
                if (selected_units.Contains(unit))
                {
                    continue;
                }

                Vector3 pos = main_camera.WorldToScreenPoint(unit.transform.position);

                if ((min.x <= pos.x) && (pos.x <= max.x) && (min.y <= pos.y) && (pos.y <= max.y))
                {
                    selected_units.Add(unit);
                }
            }
        }

        foreach (Unit selected_unit in selected_units)
        {
            selected_unit.select();
        }
    }
}
