using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtSave
{
    BtGrid.SavedState grid;
    List<BtShapeInfo> hand;

    static BtSave current;

    public static BtSave Create()
    {
        var save = new BtSave()
        {
            grid = new BtGrid.SavedState(BtGrid.current),
            hand = ShapePanel.current.GetCurrentShapes(),
        };

        current = save;
        return save;
    }

    public static bool Load(BtSave save = null)
    {
        if (save == null) save = current;
        if (save == null) return false;

        save.grid.Load(BtGrid.current);
        ShapePanel.current.SetCurrentShapes(save.hand);

        return true;
    }

}