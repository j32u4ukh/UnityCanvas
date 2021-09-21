using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityCanvas.FileLoader;
using System.Threading.Tasks;
using System.IO;
using CSharpExpansion;
using UnityExpansion;

public class FileLoaderTester : MonoBehaviour
{
    public Image image;
    public AudioSource audio_source;

    // Start is called before the first frame update
    void Start()
    {
        //_ = testLoadSprite(path: Path.Combine(Application.streamingAssetsPath, "Image", "Tiger.png"));
        //_ = testLoadAudio(path: Path.Combine(Application.streamingAssetsPath, "Music", "CorrectMusic.wav"));

        // C:\Program Files\Unity\Hub\Editor\2019.4.8f1\Editor\Data\Managed\UnityEngine

        double x = 0.35;
        double sigmoid = CSharpMath.Sigmoid(x);
        Debugging.ELevel = Debugging.Level.Warn;
        Debugging.Log($"sigmoid: {sigmoid}");
        Debugging.Warn($"sigmoid: {sigmoid}");
        Debugging.Error($"sigmoid: {sigmoid}");
        Debugging.Warn($"x: {CSharpMath.Asigmoid(sigmoid)}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async Task testLoadSprite(string path)
    {
        Sprite sprite = await ImageLoader.LoadSprite(path);
        image.sprite = sprite;
    }

    async Task testLoadAudio(string path)
    {
        AudioClip clip = await AudioLoader.LoadAudioWav(path);
        audio_source.pitch = 1f;
        audio_source.PlayOneShot(clip);
    }
}
