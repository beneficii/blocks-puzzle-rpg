using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    const float frameDelay = .1f;

    [SerializeField] List<SpriteRenderer> renders;
    UnitVisualData data;

    System.Action action;
    Unit.AnimType currentAnim;

    float nextFrame = 0f;
    int frameIdx = 0;
    int frameCount = 0;
    int frameOffset = 0;

    public void Init(UnitVisualData data)
    {
        this.data = data;
        Play(Unit.AnimType.Iddle);
    }

    public void Play(Unit.AnimType type, System.Action action = null)
    {
        this.action = action;
        currentAnim = type;
        frameIdx = 0;

        switch (type)
        {
            case Unit.AnimType.Iddle:
                frameCount = data.frIddle;
                frameOffset = 0;
                break;
            case Unit.AnimType.Attack1:
                frameCount = data.frAttack1;
                frameOffset = data.frIddle;
                break;
            case Unit.AnimType.Attack2:
                frameCount = data.frAttack2;
                frameOffset = data.frIddle + data.frAttack1;
                break;
            case Unit.AnimType.Hit:
                frameCount = data.frHit;
                frameOffset = data.frIddle + data.frAttack1 + data.frAttack2;
                break;
            default:
                return;
        }

        nextFrame = Time.time;
        Update();
    }


    private void Update()
    {
        if (currentAnim == Unit.AnimType.None || nextFrame > Time.time) return;
        nextFrame = nextFrame + frameDelay;

        foreach (var render in renders)
        {
            render.sprite = data.frames[frameOffset + frameIdx];
        }

        frameIdx++;
        if (frameIdx >= frameCount)
        {
            if (currentAnim == Unit.AnimType.Iddle)
            {
                frameIdx = 0;
            }
            else
            {
                Play(Unit.AnimType.Iddle);
                action?.Invoke();
            }
        }
    }
}
