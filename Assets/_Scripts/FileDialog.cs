using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class FileDialog : MonoBehaviour
{
    public Button image_btn;
    public Button music_btn;
    public Button video_btn;

    public RawImage ri;
    public Texture texture;
    public AudioSource _as;
    public VideoPlayer vp;
    public Text path_label;

    // Start is called before the first frame update
    void Start()
    {
        image_btn.onClick.AddListener(()=>{
            

            //string path = GetImagePath();
            //path_label.text = path;

            //if (path.Length != 0)
            //{
            //    Texture2D texture = new Texture2D(360, 130);
            //    var content = File.ReadAllBytes(path);
            //    texture.LoadImage(content);

            //    ri.texture = texture;
            //}
        });

        music_btn.onClick.AddListener(()=> {
            string path = GetMusicPath();
            path_label.text = path;

            AudioClip clip = null;
            UnityWebRequest request = null;

            try
            {
                request = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV);
                request.SendWebRequest();

            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("{0}, {1}", e.Message, e.StackTrace));
            }

            try
            {
                while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

                if (!request.isNetworkError && !request.isHttpError)
                {
                    clip = DownloadHandlerAudioClip.GetContent(request);
                    _as.pitch = 1f;
                    _as.PlayOneShot(clip);
                }
                else
                {
                    Debug.LogError(request.error);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("{0}, {1}", e.Message, e.StackTrace));
            }
        });

        video_btn.onClick.AddListener(()=> {
            ri.texture = texture;

            string path = GetVideoPath();
            path_label.text = path;

            vp.url = path;
            vp.Play();
        });
    }

    public static string GetFilePath(string title, string file_type, string file_filter, string directory = "")
    {
        //string path = EditorUtility.OpenFilePanelWithFilters(title, directory, 
        //    new string[] {file_type, file_filter});

        //return path;
        return "";
    }

    public static string GetImagePath(string file_filter = "png,jpg,jpeg")
    {
        return GetFilePath(
            title: "Choose an image", 
            file_type: "Image files", 
            file_filter: file_filter, 
            directory: "");
    }

    public static string GetMusicPath(string file_filter = "wav")
    {
        return GetFilePath(
            title: "Choose a music",
            file_type: "Music files",
            file_filter: file_filter,
            directory: "");
    }

    public static string GetVideoPath(string file_filter = "avi,flv,wmv,mp4,mov")
    {
        return GetFilePath(
            title: "Choose a video",
            file_type: "Video files",
            file_filter: file_filter,
            directory: "");
    }
}
