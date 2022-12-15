using System;
using System.Threading.Tasks;
//using UnityAsync;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityCanvas
{
    public struct Size
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    namespace FileLoader
    {

        public class ImageLoader {
            public static async Task<Sprite> LoadSprite(string path)
            {
                Texture2D texture = await LoadTexture2D(path);
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                Sprite sprite = Sprite.Create(texture, rect, new Vector2(0, 0), 100.0f);

                return sprite;
            }

            public static async Task<Texture2D> LoadTexture2D(string path)
            {
                Size size = GetImageSize(path);

                UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
                await request.SendWebRequest();

                while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

                if (!request.isNetworkError && !request.isHttpError)
                {
                    Texture2D texture = new Texture2D(size.Width, size.Height);
                    byte[] bytes = request.downloadHandler.data;
                    texture.LoadImage(bytes);
                    return texture;
                }

                return null;
            }

            public static Size GetImageSize(string path)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(path);
                System.Drawing.Size size = image.Size;

                return new Size()
                {
                    Width = size.Width,
                    Height = size.Height
                };
            }


        }

        public class AudioLoader
        {
            public async static Task<AudioClip> LoadAudioWav(string path)
            {
                AudioClip clip = await LoadAudioClip(path, audio_type: AudioType.WAV);

                return clip;
            }

            public async static Task<AudioClip> LoadAudioClip(string path, AudioType audio_type)
            {
                AudioClip clip = null;
                UnityWebRequest request = null;

                try
                {
                    request = UnityWebRequestMultimedia.GetAudioClip(path, audio_type);
                    await request.SendWebRequest();

                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}, {e.StackTrace}");
                }

                try
                {
                    while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        clip = DownloadHandlerAudioClip.GetContent(request);
                    }
                    else
                    {
                        Debug.LogError(request.error);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}, {e.StackTrace}");
                }

                return clip;
            }
        }
    }
}