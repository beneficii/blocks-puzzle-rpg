using System;
using UnityEngine;

public class FxAnimator : MonoBehaviour
{
    const float frameDelay = 0.1f;

    [SerializeField] private SpriteRenderer render;
    private FxData fxData;
    private Action onActionFrameReached;

    private float nextFrame = 0f;
    private int frameIdx = 0;

    public void Init(FxData fxData, Action onActionFrameReached = null)
    {
        this.fxData = fxData;
        this.onActionFrameReached = onActionFrameReached;
        frameIdx = 0;
        nextFrame = Time.time;
        UpdateFrame();
    }

    private void Update()
    {
        if (fxData == null || nextFrame > Time.time) return;

        nextFrame += frameDelay;
        UpdateFrame();
    }

    private void UpdateFrame()
    {
        render.sprite = fxData.frames[frameIdx];

        if (frameIdx == fxData.actionFrame)
        {
            onActionFrameReached?.Invoke();
        }

        frameIdx++;

        if (frameIdx >= fxData.frames.Count)
        {
            if (fxData.actionFrame == -1) onActionFrameReached?.Invoke();
            Destroy(gameObject);
        }
    }
}
