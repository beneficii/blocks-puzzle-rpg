using FancyToolkit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class StageCtrl : GenericDataCtrl<StageData>
{
    static StageCtrl _current;
    public static StageCtrl current
    {
        get
        {
            if (_current == null)
            {
                var cur = new StageCtrl();
                _current = cur;
            }

            return _current;
        }
    }

    StageData currentData;
    string currentId = "test";

    public void SetStage(StageData stageData)
    {
        currentData = stageData;
    }

    public void SetStage(string id)
    {
        currentId = id;
        currentData = null;
    }

    public StageData Data
    {
        get
        {
            if (currentData == null)
            {
                currentData = current.Get(currentId);
            }

            return currentData;
        }
    }
}
