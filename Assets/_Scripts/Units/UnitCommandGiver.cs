using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unit_selection_handler = null;
    [SerializeField] private LayerMask layer_mask = new LayerMask();
    private Camera main_camera;

    // Start is called before the first frame update
    void Start()
    {
        main_camera = Camera.main;
        GameOverHandler.onClientGameOver += handleClientGameOver;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            // Input.mousePosition -> Mouse.current.position.ReadValue()
            Ray ray = main_camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer_mask))
            {
                if(hit.collider.TryGetComponent<Targetable>(out Targetable target))
                {
                    if (target.hasAuthority)
                    {
                        tryMove(hit.point);
                    }
                    else
                    {
                        tryTarget(target);
                    }
                }
                else
                {
                    tryMove(hit.point);
                }
            }
        }
    }

    private void OnDestroy()
    {
        GameOverHandler.onClientGameOver -= handleClientGameOver;
    }

    private void tryMove(Vector3 point)
    {
        foreach(Unit unit in unit_selection_handler.selected_units)
        {
            unit.getUnitMovement().cmdMove(point);
        }
    }

    private void tryTarget(Targetable target)
    {
        foreach (Unit unit in unit_selection_handler.selected_units)
        {
            unit.getTargeter().cmdSetTarget(target.gameObject);
        }
    }

    private void handleClientGameOver(string winner_name)
    {
        enabled = false;
    }
}
