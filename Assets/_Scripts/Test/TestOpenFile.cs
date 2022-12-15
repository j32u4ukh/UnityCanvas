using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityExpansion;

public class TestOpenFile : MonoBehaviour
{
    public Button image_btn;
    public Button music_btn;
    public Button video_btn;

    // Start is called before the first frame update
    void Start()
    {
        image_btn.onClick.AddListener(() =>
        {
            string path = OpenFile.GetImagePath(dir: Path.Combine(Application.streamingAssetsPath, "Image"));
            print(path);

            _ = copy(path, Path.Combine(Application.streamingAssetsPath, "Image", "copy1.png"));
        });

        music_btn.onClick.AddListener(() =>
        {
            string path = OpenFile.GetMusicPath(dir: Path.Combine(Application.streamingAssetsPath, "Music"));
            print(path);
            bool is_success = FastCopy(path, Path.Combine(Application.streamingAssetsPath, "Music", "copy1.wav"));
            Debugging.Log($"Copy: {is_success} {path}");
        });

        video_btn.onClick.AddListener(() =>
        {
            string path = OpenFile.GetVideoPath(dir: Path.Combine(Application.streamingAssetsPath, "Video"));
            print(path);
            _ = copy(path, Path.Combine(Application.streamingAssetsPath, "Video", "copy1.mp4"));
        });
    }

    public async Task copy(string source, string destination)
    {
        bool is_success = await Copy(source, destination);
        Debugging.Log($"Copy: {is_success} {source}");
    }

    public static async Task<bool> Copy(string source, string destination)
    {
        bool is_success = await CopyTask(source, destination);

        return is_success;
    }

    public static bool FastCopy(string source, string destination)
    {
        if (File.Exists(source))
        {
            if (File.Exists(destination))
            {
                return false;
            }
            else
            {
                File.Copy(source, destination);
                return true;
            }
        }

        return false;
    }

    public static async Task<bool> CopyTask(string source, string destination)
    {
        if (File.Exists(source))
        {
            if (File.Exists(destination))
            {
                return false;
            }
            else
            {
                using (FileStream reader = File.Open(source, FileMode.Open))
                {
                    using (Stream writer = File.Create(destination))
                    {
                        await reader.CopyToAsync(writer);
                    }
                }

                return true;
            }
        }

        return false;
    }
}
