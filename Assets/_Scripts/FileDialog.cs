using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FileDialog : MonoBehaviour
{
    public Button button;
    public RawImage raw_image;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(()=>{
            Apply();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MenuItem("Example/Overwrite Texture")]
    void Apply()
    {
        //Texture2D texture = Selection.activeObject as Texture2D;
        //if (texture == null)
        //{
        //    EditorUtility.DisplayDialog("Select Texture", "You must select a texture first!", "OK");
        //    return;
        //}

        Texture2D texture = new Texture2D(100, 100);

        //string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
        string path = EditorUtility.OpenFilePanelWithFilters("Overwrite with png", "", new string[] { "Image files", "png,jpg,jpeg" });
        print($"path: {path}");

        if (path.Length != 0)
        {
            var fileContent = File.ReadAllBytes(path);
            texture.LoadImage(fileContent);
        }

        raw_image.texture = texture;
    }
}
