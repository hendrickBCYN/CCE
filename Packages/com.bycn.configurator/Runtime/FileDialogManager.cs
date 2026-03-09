using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public static class FileDialogManager
{
    /*[DllImport("user32.dll")]
    private static extern void SaveFileDialog(); //in your case : OpenFileDialog

    [DllImport("user32.dll")]
    private static extern void OpenFileDialog(); //in your case : OpenFileDialog

    [DllImport("user32.dll")]
    private static extern void FolderBrowserDialog(); //in your case : OpenFileDialog*/

    public static string OpenFolder()
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);

        return Application.streamingAssetsPath;
        
        /*string l_folderPath = null;
        System.Windows.Forms.FolderBrowserDialog sfd = new System.Windows.Forms.FolderBrowserDialog();
        if(sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            l_folderPath = sfd.SelectedPath;
        }
        sfd.Dispose();
        return l_folderPath;*/
    }
    
    public static string OpenFile(string p_filename)
    {
        string l_filePath = Application.streamingAssetsPath + "/" + p_filename;
        return l_filePath;

        /*string l_filePath = null;
        System.Windows.Forms.OpenFileDialog sfd = new System.Windows.Forms.OpenFileDialog();
        if(sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            l_filePath = sfd.FileName;
        }
        sfd.Dispose();
        return l_filePath;*/
    }
}
