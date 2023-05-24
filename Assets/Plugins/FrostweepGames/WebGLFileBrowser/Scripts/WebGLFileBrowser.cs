using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
using System.Linq;
#endif
#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace FrostweepGames.Plugins.WebGLFileBrowser
{
    public class WebGLFileBrowser : MonoBehaviour
    {
        /// <summary>
        ///     Will fire when file will successfully be loaded
        /// </summary>
        public static event Action<File[]> FilesWereOpenedEvent;

        /// <summary>
        ///     Will fire when native file loading popup was closed
        /// </summary>
        public static event Action FilePopupWasClosedEvent;

        /// <summary>
        ///     Will fire when error received during file loading
        /// </summary>
        public static event Action<string> FileOpenFailedEvent;

        /// <summary>
        ///     Will fire when file was successfully saved
        /// </summary>
        public static event Action<File> FileWasSavedEvent;

        /// <summary>
        ///     Will fire when error received during file saving
        /// </summary>
        public static event Action<string> FileSaveFailedEvent;

        private static WebGLFileBrowser _Instance;

#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
        private static bool _FileBrowserActive;
        private static bool _IsWasInFullScreen;

        [DllImport("__Internal")]
        private static extern void initialize(double version);

        [DllImport("__Internal")]
        private static extern void openFileBrowserForLoad(string typesFilter, int isMultipleSelection);

        [DllImport("__Internal")]
        private static extern void closeFileBrowserForOpen();

        [DllImport("__Internal")]
        private static extern void saveFile(string fileName, string data);

        [DllImport("__Internal")]
        private static extern void setLocalization(string key, string value);     

        [DllImport("__Internal")]
        private static extern void cleanup();

        [DllImport("__Internal")]
        private static extern IntPtr loadFileData(string fileName);
#endif
        private static readonly List<Object> _unityObjects = new();
        private static readonly List<File> _files = new();
        private static readonly Dictionary<string, File> _savingFilesCache = new();
        private static int _filesToBeLoaded;
        private static int _filesWereLoaded;

        private void
            Awake() // could be potencial issue with static constructors. set loading of this script with better execution order in project settings
        {
            if (_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _Instance = this;
            DontDestroyOnLoad(gameObject);

            gameObject.name = "[FGFileBrowser]";

#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
#if UNITY_2020_3_OR_NEWER
            initialize(2021.1);
#else
            initialize(0);
#endif

#endif
        }

        private void OnDestroy()
        {
            if (_Instance != this)
                return;
            _Instance = null;
        }

        /// <summary>
        ///     Opens Native File Browser Dialog
        /// </summary>
        public static void OpenFilePanelWithFilters(string typesFilter, bool isMultipleSelection = false)
        {
            _filesWereLoaded = 0;
            _filesToBeLoaded = 0;

#if UNITY_EDITOR
            var path = EditorUtility.OpenFilePanelWithFilters("Editor File Browser", Directory.GetLogicalDrives()[0],
                new[] { "User Files", typesFilter.Replace(".", string.Empty) });

            if (path.Length != 0)
            {
                var fileContent = System.IO.File.ReadAllBytes(path);

                var file = new File
                {
                    fileInfo = new FileInfo
                    {
                        extension = Path.GetExtension(path),
                        name = Path.GetFileNameWithoutExtension(path),
                        fullName = Path.GetFileName(path),
                        length = fileContent.Length,
                        path = path,
                        size = fileContent.Length
                    },
                    data = fileContent
                };

                _files.Add(file);

                if (FilesWereOpenedEvent != null)
                    FilesWereOpenedEvent(new[] { file });
            }
            else
            {
                if (FileOpenFailedEvent != null)
                    FileOpenFailedEvent("Open file failed.");
            }
#else
#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
            if (_FileBrowserActive)
                return;

            if (Screen.fullScreen)
            {
                Screen.fullScreen = false;
                _IsWasInFullScreen = true;
            }
            else _IsWasInFullScreen = false;

            _FileBrowserActive = true;
#endif

#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
            openFileBrowserForLoad(typesFilter, isMultipleSelection ? 1 : 0);
#endif

#endif
        }

        /// <summary>
        ///     Hides Native File Browser Dialog
        /// </summary>
        public static void HideFileDialog()
        {
#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
            closeFileBrowserForOpen();
#endif
#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
            _FileBrowserActive = false;
            if (_IsWasInFullScreen)
                Screen.fullScreen = true;
#endif
        }

        /// <summary>
        ///     Saves file
        /// </summary>
        /// <param name="file"></param>
        public static void SaveFile(File file)
        {
            if (file == null)
            {
                if (FileSaveFailedEvent != null)
                    FileSaveFailedEvent("Save file failed due to: file is null.");
                return;
            }

#if UNITY_EDITOR
            var path = EditorUtility.SaveFilePanel("Editor File Browser", Directory.GetLogicalDrives()[0],
                file.fileInfo.name, file.fileInfo.extension.Replace(".", string.Empty));

            if (path.Length != 0)
            {
                System.IO.File.WriteAllBytes(path, file.data);

                if (FileWasSavedEvent != null)
                    FileWasSavedEvent.Invoke(file);
            }
            else
            {
                if (FileSaveFailedEvent != null)
                    FileSaveFailedEvent("Save file failed.");
            }
#else
#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
            if (!_savingFilesCache.ContainsKey(file.fileInfo.fullName))
                _savingFilesCache.Add(file.fileInfo.fullName, file);

            saveFile(file.fileInfo.fullName, Convert.ToBase64String(file.data));
#endif

#endif
        }

        /// <summary>
        ///     Filters string by extensions and prepare for file browser
        /// </summary>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static string GetFilteredFileExtensions(string extensions)
        {
            string[] types = { string.Empty };
            if (!string.IsNullOrEmpty(extensions))
            {
                extensions = extensions.Replace(" ", string.Empty);
                types = extensions.Split(',');
            }

            if (!string.IsNullOrEmpty(types[0])) types[0] = types[0].Insert(0, ".");

            return string.Join(",.", types);
        }

        /// <summary>
        ///     Set localization of popup components
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetLocalization(LocalizationKey key, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;
#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
            setLocalization(key.ToString(), value);
#endif
        }

        /// <summary>
        ///     Will add unity object to cache list
        /// </summary>
        /// <param name="unityObject"></param>
        public static void RegisterFileObject(Object unityObject)
        {
            if (unityObject == null)
                return;

            _unityObjects.Add(unityObject);
        }

        /// <summary>
        ///     Will clean all cached unity objects, files and will use GC.Collect with Resources.UnloadUnusedAssets
        /// </summary>
        public static void FreeMemory()
        {
            for (var i = 0; i < _unityObjects.Count; i++)
                if (_unityObjects[i] != null && _unityObjects[i])
                {
                    if (_unityObjects[i] is Sprite sprite)
                    {
                        Destroy(sprite.texture);
                        Destroy(sprite);
                    }
                    else
                    {
                        Destroy(_unityObjects[i]);
                    }
                }

            for (var i = 0; i < _files.Count; i++)
            {
                _files[i].data = null;
                _files[i].fileInfo = null;
            }

            _files.Clear();
            _unityObjects.Clear();
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        ///     Event handler from Native code
        /// </summary>
        private void HandleLoadedFile(string jsonData)
        {
            _filesWereLoaded++;

            var fileInfo = JsonUtility.FromJson<FileInfo>(jsonData);

            var fileLoaded = false;

#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
            var length = fileInfo.length;
            var dataPointer = loadFileData(fileInfo.fullName);
            var data = new byte[length];
            try
            {
                Marshal.Copy(dataPointer, data, 0, data.Length);
                fileLoaded = true;
            }
            catch(Exception ex)
			{
                Debug.LogException(ex);

                if (FileOpenFailedEvent != null)
                    FileOpenFailedEvent(ex.Message);
			}
#endif
            HideFileDialog();

            if (fileLoaded)
            {
                var file = new File
                {
                    fileInfo = fileInfo,
#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
                    data = data
#endif
                };

#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
                cleanup();
#endif

                _files.Add(file);

                if (_filesWereLoaded >= _filesToBeLoaded)
                    if (FilesWereOpenedEvent != null)
                        FilesWereOpenedEvent(_files.GetRange(_files.Count - _filesToBeLoaded, _filesToBeLoaded)
                            .ToArray());
            }
        }

        /// <summary>
        ///     Event handler from Native code
        /// </summary>
        private void CloseFileBrowserForOpen()
        {
            HideFileDialog();

            if (FilePopupWasClosedEvent != null)
                FilePopupWasClosedEvent();
        }

        /// <summary>
        ///     Event handler from Native code
        /// </summary>
        private void HandleFileSaved(string jsonData)
        {
            var fileSaveInfo = JsonUtility.FromJson<FileSaveInfo>(jsonData);

            if (fileSaveInfo.status)
            {
                if (FileWasSavedEvent != null)
                {
                    if (_savingFilesCache.ContainsKey(fileSaveInfo.name))
                    {
                        FileWasSavedEvent?.Invoke(_savingFilesCache[fileSaveInfo.name]);
                    }
                    else
                    {
                        Debug.LogWarning($"Saved file {fileSaveInfo.name} not found in cache to be in an event!");
                        FileWasSavedEvent?.Invoke(null);
                    }
                }
            }
            else
            {
                if (FileSaveFailedEvent != null)
                    FileSaveFailedEvent?.Invoke(fileSaveInfo.message);
            }

            if (_savingFilesCache.ContainsKey(fileSaveInfo.name))
                _savingFilesCache.Remove(fileSaveInfo.name);
        }

        /// <summary>
        ///     Event handler from Native code
        /// </summary>
        private void SetAmountOfFilesToBeLoaded(int amount)
        {
            _filesToBeLoaded = amount;
        }
    }

    public enum LocalizationKey
    {
        HEADER_TITLE,
        DESCRIPTION_TEXT,
        SELECT_BUTTON_CONTENT,
        CLOSE_BUTTON_CONTENT
    }

    public class FileInfo
    {
        public string extension;
        public string fullName;
        public long length;
        public string name;
        public string path;
        public long size;


        /// <summary>
        ///     Returns size of file converted to formates string
        /// </summary>
        /// <returns></returns>
        public string SizeToString()
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (size == 0)
                return "0" + suf[0];
            var bytes = Math.Abs(size);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return Math.Sign(size) * num + suf[place];
        }
    }

    public class FileSaveInfo
    {
        public string message;
        public string name;
        public bool status;
    }

    public class File
    {
        public byte[] data;
        public FileInfo fileInfo;
    }

    public static class FileExtension
    {
        /// <summary>
        ///     Will convert file content to TExture2D
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Texture2D ToTexture2D(this File file)
        {
            if (file.data == null)
                return null;

            var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);

            texture.name = file.fileInfo.name;
            texture.LoadImage(file.data);

            return texture;
        }

        /// <summary>
        ///     Will convert file content to Sprite
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Sprite ToSprite(this File file)
        {
            var texture = file.ToTexture2D();

            if (texture == null)
                return null;

            var sprite = Sprite.Create(texture,
                new Rect(Vector2.zero, new Vector2(texture.width, texture.height)),
                Vector2.one / 2f,
                100,
                1,
                SpriteMeshType.FullRect,
                Vector4.zero);
            return sprite;
        }

        /// <summary>
        ///     Will convert Texture2D to base64 data
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="isJPEG"></param>
        /// <returns></returns>
        public static string TextureToBase64(this Texture2D texture, bool isJPEG = false)
        {
            if (texture == null)
                return string.Empty;

            if (isJPEG)
                return Convert.ToBase64String(texture.EncodeToJPG());
            return Convert.ToBase64String(texture.EncodeToPNG());
        }

        /// <summary>
        ///     Will convert file content to string
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string ToStringContent(this File file)
        {
            return Encoding.UTF8.GetString(file.data);
        }

        /// <summary>
        ///     Doesnt supported yet! Will return empty audio clip.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static AudioClip ToWavAudioClip(this File file)
        {
            var clip = AudioClip.Create(file.fileInfo.name, 0, 1, 48000, false);
            return clip;
        }

        public static bool IsImage(this File file)
        {
            return file.fileInfo.extension.ToLower().EndsWith("png") ||
                   file.fileInfo.extension.ToLower().EndsWith("jpeg") ||
                   file.fileInfo.extension.ToLower().EndsWith("jpg");
        }

        public static bool IsText(this File file)
        {
            return file.fileInfo.extension.ToLower().EndsWith("txt") ||
                   file.fileInfo.extension.ToLower().EndsWith("json");
        }

        public static bool IsWavSound(this File file)
        {
            return file.fileInfo.extension.ToLower().EndsWith("wav");
        }
    }
}