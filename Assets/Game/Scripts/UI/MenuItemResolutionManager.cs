using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemResolutionManager : MonoBehaviour
{
    public Image ImageA; // Reference to the first image
    public Image ImageB; // Reference to the second image (replacement)
    public Image ImageC; // Reference to the image to check collision with

    private RectTransform rectA;
    private RectTransform rectC;

    private IEnumerator Start()
    {
        yield return null;
        if (ImageA && ImageB && ImageC)
        {
            rectA = ImageA.GetComponent<RectTransform>();
            rectC = ImageC.GetComponent<RectTransform>();

            ImageB.gameObject.SetActive(false); // Ensure ImageB is inactive at start
            CheckOverlap(); // Check on game start
        }
    }

    private void Update()
    {
        // Detects window resize automatically
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            CheckOverlap();
        }
    }

    private int lastScreenWidth, lastScreenHeight;

    private void CheckOverlap()
    {
        if (rectA == null || rectC == null) return;

        if (IsOverlapping(rectA, rectC))
        {
            ImageA.gameObject.SetActive(false);
            ImageB.gameObject.SetActive(true);
        }
        else
        {
            ImageA.gameObject.SetActive(true);
            ImageB.gameObject.SetActive(false);
        }
    }

    private bool IsOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Vector3[] corners1 = new Vector3[4];
        Vector3[] corners2 = new Vector3[4];

        rect1.GetWorldCorners(corners1);
        rect2.GetWorldCorners(corners2);

        Rect rectA = new Rect(corners1[0].x, corners1[0].y, corners1[2].x - corners1[0].x, corners1[2].y - corners1[0].y);
        Rect rectB = new Rect(corners2[0].x, corners2[0].y, corners2[2].x - corners2[0].x, corners2[2].y - corners2[0].y);

        return rectA.Overlaps(rectB);
    }
}
