using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

namespace UnityExpansion
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

    public class OpenFileName
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    public class WindowDll
    {
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

        public static bool GetOpenFileNameExtern([In, Out] OpenFileName ofn)
        {
            return GetOpenFileName(ofn);
        }
    }

    public class OpenFile
    {
        public static string GetImagePath(string dir="")
        {
            string path = GetFilePath(filter: "Images\0*.jpg;*.png\0\0", dir: dir);
            return path;
        }

        public static string GetMusicPath(string dir = "")
        {
            string path = GetFilePath(filter: "Musics\0*.wav\0\0", dir: dir);
            return path;
        }

        public static string GetVideoPath(string dir = "")
        {
            string path = GetFilePath(filter: "Videos\0*.mp4\0\0", dir: dir);
            return path;
        }

        private static string GetFilePath(string filter = "All Files\0*.*\0\0", string dir = "")
        {
            if (string.IsNullOrEmpty(dir))
            {
                dir = Application.streamingAssetsPath;
            }

            dir = dir.Replace('/', '\\');

            string path = OpenFileWin(filter: filter, dir: dir);

            return path;
        }

        private static string OpenFileWin(string title = "Open file", string filter = "All Files\0*.*\0\0", string dir = ".")
        {
            OpenFileName ofn = new OpenFileName();

            ofn.structSize = Marshal.SizeOf(ofn);

            ofn.filter = filter;

            ofn.file = new string(new char[256]);

            ofn.maxFile = ofn.file.Length;

            ofn.fileTitle = new string(new char[64]);

            ofn.maxFileTitle = ofn.fileTitle.Length;

            // 默認路徑  
            ofn.initialDir = dir;

            ofn.title = title;

            // 顯示文件的類型 
            //ofn.defExt = "png";

            // 注意 一下項目不一定要全選 但是 0x00000008 項不要缺少  
            // OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
            //ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
            ofn.flags = 0x00080000 | 0x00001000 | 0x00000008;

            if (WindowDll.GetOpenFileNameExtern(ofn))
            {
                return ofn.file;
            }
            else
            {
                return "";
            }
        }
    }
}