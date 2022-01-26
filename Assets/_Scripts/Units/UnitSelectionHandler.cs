using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layer_mask = new LayerMask();
    private Camera main_camera;
    public List<Unit> selected_units { get; } = new List<Unit>();

    // Start is called before the first frame update
    void Start()
    {
        main_camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Start selection area
            foreach (Unit selected_unit in selected_units)
            {
                selected_unit.deselect();
            }

            selected_units.Clear();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            clearSelectionArea();
        }
    }

    private void clearSelectionArea()
    {
        // Input.mousePosition -> Mouse.current.position.ReadValue()
        Ray ray = main_camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer_mask))
        {
            if(!hit.collider.TryGetComponent<Unit>(out Unit unit))
            {
                return;
            }

            if (!unit.hasAuthority)
            {
                return;
            }

            selected_units.Add(unit);

            foreach(Unit selected_unit in selected_units)
            {
                selected_unit.select();
            }
        }
    }
}
