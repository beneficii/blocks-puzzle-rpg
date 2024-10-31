using FancyToolkit;
using System.Collections;
using UnityEngine;

public class SkillData : DataWithId
{
    public string idVisual;
    public string name;
    public Sprite sprite;

    public FactoryBuilder<SkillClickCondition> clickCondition;
    public FactoryBuilder<SkillActionBase> onClick;

    public FactoryBuilder<SkillActionBase> onEndTurn;
    public FactoryBuilder<SkillActionBase> onStartCombat;
}