using System.IO;
using System.Threading.Tasks;
using UnityCanvas.FileLoader;
using UnityEngine;

public class TestCircle : MonoBehaviour
{
    public Circle circle;
    Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        _ = loadSprite();
        circle.SetTexture(width: 300, height: 300);        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            circle.SetCircle(sprite.texture, radius: 100);
        }   
    }

    async Task loadSprite()
    {
        string path = Path.Combine(Application.dataPath, "Resources", "Elephant.png");
        print($"path: {path}");
        sprite = await ImageLoader.LoadSprite(path);

        Rect rect = sprite.rect;
        print($"width: {rect.width}, height: {rect.height}");
    }
}
