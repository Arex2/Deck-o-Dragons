using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Text;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Opens specific types of files in their Default Windows Application using Assoc Query String.
/// </summary>
// Script by Ruben
public class DefaultAppFileOpener
{
    /// <summary>
    /// This method is called every time Unity tries to open an asset.
    /// </summary>
    // Callback order is set to 0 to make sure no other OnOpenAsset method is called before this one (this one should take priority)
    [UnityEditor.Callbacks.OnOpenAsset(0)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        // Get the asset path of the asset we are trying to open
        string assetPath = AssetDatabase.GetAssetPath(instanceID);

        // Check if the file path ends with one of the specific file types
        if (assetPath.EndsWith(".shader") || assetPath.EndsWith(".hlsl") || assetPath.EndsWith(".txt"))
        {
            // Wrap this all in a try catch block
            try
            {
                // Get the actual path of the file on the disk
                string actualFilePath = Application.dataPath;

                actualFilePath = Path.Combine(actualFilePath, assetPath.Substring(7));

                // Get the file type's default application exe path by using AssocQueryString (code taken from https://stackoverflow.com/a/17773402)
                string executableFilePath = AssocQueryString(AssocStr.Executable, Path.GetExtension(actualFilePath));

                // If there is no default application to open this file type, then just let Unity handle the rest
                if (string.IsNullOrEmpty(executableFilePath))
                {
                    return false;
                }

                // Open the default application
                Process.Start(executableFilePath, $"\"{actualFilePath}\"");
            }
            // If we catch an exception, then we let Unity handle the rest
            catch (Exception)
            {
                return false;
            }

            // Tell Unity that we successfully opened our file
            return true;
        }

        // The file didn't match any of the wanted file types so let Unity handle the rest
        return false;
    }

    // A bunch of alien code that I copied from here: https://stackoverflow.com/a/17773402
    #region Assoc Query String
    [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
    public static extern uint AssocQueryString(
        AssocF flags,
        AssocStr str,
        string pszAssoc,
        string pszExtra,
        [Out] StringBuilder pszOut,
        ref uint pcchOut
    );

    [Flags]
    public enum AssocF
    {
        None = 0,
        Init_NoRemapCLSID = 0x1,
        Init_ByExeName = 0x2,
        Open_ByExeName = 0x2,
        Init_DefaultToStar = 0x4,
        Init_DefaultToFolder = 0x8,
        NoUserSettings = 0x10,
        NoTruncate = 0x20,
        Verify = 0x40,
        RemapRunDll = 0x80,
        NoFixUps = 0x100,
        IgnoreBaseClass = 0x200,
        Init_IgnoreUnknown = 0x400,
        Init_Fixed_ProgId = 0x800,
        Is_Protocol = 0x1000,
        Init_For_File = 0x2000
    }

    public enum AssocStr
    {
        Command = 1,
        Executable,
        FriendlyDocName,
        FriendlyAppName,
        NoOpen,
        ShellNewValue,
        DDECommand,
        DDEIfExec,
        DDEApplication,
        DDETopic,
        InfoTip,
        QuickTip,
        TileInfo,
        ContentType,
        DefaultIcon,
        ShellExtension,
        DropTarget,
        DelegateExecute,
        Supported_Uri_Protocols,
        ProgID,
        AppID,
        AppPublisher,
        AppIconReference,
        Max
    }

    static string AssocQueryString(AssocStr association, string extension)
    {
        const int S_OK = 0;
        const int S_FALSE = 1;

        uint length = 0;
        uint ret = AssocQueryString(AssocF.None, association, extension, null, null, ref length);
        if (ret != S_FALSE)
        {
            throw new InvalidOperationException("Could not determine associated string");
        }

        var sb = new StringBuilder((int)length); // (length-1) will probably work too as the marshaller adds null termination
        ret = AssocQueryString(AssocF.None, association, extension, null, sb, ref length);
        if (ret != S_OK)
        {
            throw new InvalidOperationException("Could not determine associated string");
        }

        return sb.ToString();
    }
    #endregion
}