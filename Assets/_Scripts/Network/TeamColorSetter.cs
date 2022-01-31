using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] color_renderers = new Renderer[0];

    [SyncVar(hook = nameof(handleTeamColorUpdated))] private Color team_color = new Color();

    #region Server
    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
        team_color = player.getTeamColor();
    }
    #endregion

    #region Client
    private void handleTeamColorUpdated(Color origin_color, Color new_color)
    {
        foreach(Renderer renderer in color_renderers)
        {
            renderer.material.SetColor("_BaseColor", new_color);
        }
    }
    #endregion
}
