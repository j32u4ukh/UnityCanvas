using System.Runtime.InteropServices;

public class OpenFile
{
    public static string OpenFileWin(string title="Open file", string filter= "All Files\0*.*\0\0", string dir=".")
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
        ofn.defExt = "png";

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