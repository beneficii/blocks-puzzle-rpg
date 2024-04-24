using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPanel : MonoBehaviour
{
    [SerializeField] int costHint;
    [SerializeField] int costRewind;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void Close()
    {

    }


    public void BtnShowHint()
    {
        ShapePanel.current.BtnShowHint();
    }

    public void BtnGenerateNewShapes()
    {
        ShapePanel.current.GenerateNew(false);
    }

    public void BtnLoadSave()
    {
        BtSave.Load();
    }

    public void BtnAutoplay()
    {
        ShapePanel.current.BtnAutoplay();
    }
}