using System.Collections;
using UnityEngine;

public class UIHudCombat : UIHudBase
{
    public static UIHudCombat _current;
    public static UIHudCombat current
    {
        get
        {
            if (!_current)
            {
                _current = FindFirstObjectByType<UIHudCombat>();
                _current.Init();
            }

            return _current;
        }
    }

    public void Start()
    {
        //Opened();
    }

    void Init()
    {

    }

    public void Show()
    {
        Opened();
    }
}