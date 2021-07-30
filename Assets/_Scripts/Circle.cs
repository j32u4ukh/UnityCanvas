using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Circle : MonoBehaviour
{
    RawImage raw;
    Texture2D texture;
    Color[] transparent_canvas;
    Color transparent;

    int WIDTH, HEIGHT, SIZE, RADIUS;
    Vector2 CENTER;

    // Start is called before the first frame update
    void Start()
    {
        transparent = new Color(0f, 0f, 0f, 0f);        
    }

    public void SetTexture(int width, int height)
    {
        WIDTH = width;
        HEIGHT = height;
        CENTER = new Vector2(WIDTH / 2, HEIGHT / 2);
        SIZE = WIDTH * HEIGHT;

        raw = GetComponent<RawImage>();
        texture = new Texture2D(WIDTH, HEIGHT);
        raw.texture = texture;

        transparent_canvas = new Color[SIZE];

        for(int i = 0; i < SIZE; i++)
        {
            transparent_canvas[i] = transparent;
        }

        texture.SetPixels(transparent_canvas);
        texture.Apply();
    }

    public void SetCircle(Texture2D texture, int radius)
    {
        int h, w;
        RADIUS = radius;
        texture = Resize(texture, WIDTH, HEIGHT);

        for (h = 0; h < HEIGHT; h++)
        {
            for(w = 0; w < WIDTH; w++)
            {
                if(InCircle(h, w))
                {
                    this.texture.SetPixel(w, h, texture.GetPixel(w, h));
                }
            }
        }

        this.texture.Apply();
    }

    Texture2D Resize(Texture2D texture, int width, int height)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture, rt);
        Texture2D result = new Texture2D(width, height);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();
        return result;
    }

    // 是否要設置為透明
    bool InCircle(int h, int w)
    {
        if (IsAvailable(h, w))
        {
            float distance = Vector2.Distance(CENTER, new Vector2(w, h));

            return distance <= RADIUS;
        }

        return false;
    }

    // 是否在圖片範圍內
    bool IsAvailable(int h, int w)
    {
        if (h < 0 || HEIGHT <= h || w < 0 || WIDTH <= w)
        {
            return false;
        }

        return true;
    }
}
