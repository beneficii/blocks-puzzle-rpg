using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GridBoard;

public class MyTile : Tile
{
    public MyTileData myData => data as MyTileData;

    public override string GetDescription()
    {
        if (myData == null) return "";

        var descr = data.GetDescription();
        if (!string.IsNullOrWhiteSpace(descr))
        {
            return descr;
        }

        var lines = new List<string>();

        if (!string.IsNullOrWhiteSpace(data.description))
        {
            lines.Add(data.description);
        }

        var clearAction = myData.clearAction?.Build().GetDescription(this);
        if (!string.IsNullOrWhiteSpace(clearAction))
        {
            lines.Add($"Clear: {clearAction}");
        }

        return string.Join(". ", clearAction);
    }
}