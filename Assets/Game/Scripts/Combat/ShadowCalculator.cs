using UnityEngine;

public class ShadowCalculator
{
    public static Vector3 Calculate(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        Rect rect = sprite.textureRect;
        Color[] pixels = texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        int width = (int)rect.width;
        int height = (int)rect.height;

        int minX = width;
        int maxX = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (pixels[y * width + x].a > 0.01f)
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                }
            }
        }

        return new(minX, maxX, width);
    }
}