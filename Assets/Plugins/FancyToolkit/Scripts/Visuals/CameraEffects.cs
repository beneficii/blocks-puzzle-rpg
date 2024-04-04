using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects current;

    private Vector3 originalPosition;

    private void Awake()
    {
        current = this;
    }

    public float testMagnitued = 0.2f;
    public float testDuration = 0.2f;

    public void TestShake()
    {
        Shake(testMagnitued, testDuration);
    }

    public void Shake(float magnitude, float duration = 0.05f)
    {
        StartCoroutine(RoutineShake(magnitude, duration));
    }

    IEnumerator RoutineShake(float magnitude, float duration)
    {
        originalPosition = transform.localPosition;
        float decrease = (magnitude*0.8f) / duration;
        int direction = 1;

        while (duration > 0)
        {
            //float x = Random.Range(-1f, 1f) * magnitude;
            float y = direction * magnitude;
            direction *= -1;

            transform.localPosition = new Vector3(originalPosition.x, originalPosition.y + y, originalPosition.z);

            duration -= Time.deltaTime;
            magnitude -= Time.deltaTime * decrease;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}