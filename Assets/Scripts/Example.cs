using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace FrostweepGames.Plugins.WebGLFileBrowser.Examples
{
    public class Example : MonoBehaviour
    {
        public Image contentImage;

        public Button openFileDialogButton;

        public Button saveOpenedFileButton;

        public Button cleanupButton;

        public InputField filterOfTypesField;

        public Text fileNameText,
            fileInfoText;

        public CreateVK createVk;
        public FormsController formsController;
        public GameObject handler;

        private string _enteredFileExtensions;

        private File[] _loadedFiles;




        private void Start()
        {
            handler = GameObject.Find("GameHandler");
            createVk = handler.GetComponent<CreateVK>();
            formsController = handler.GetComponent<FormsController>();

            openFileDialogButton.onClick.AddListener(OpenFileDialogButtonOnClickHandler);
            saveOpenedFileButton.onClick.AddListener(SaveOpenedFileButtonOnClickHandler);
            cleanupButton.onClick.AddListener(CleanupButtonOnClickHandler);
            filterOfTypesField.onValueChanged.AddListener(FilterOfTypesFieldOnValueChangedHandler);

            WebGLFileBrowser.FilesWereOpenedEvent += FilesWereOpenedEventHandler;
            WebGLFileBrowser.FilePopupWasClosedEvent += FilePopupWasClosedEventHandler;
            WebGLFileBrowser.FileOpenFailedEvent += FileOpenFailedEventHandler;
            WebGLFileBrowser.FileWasSavedEvent += FileWasSavedEventHandler;
            WebGLFileBrowser.FileSaveFailedEvent += FileSaveFailedEventHandler;

            // if you want to set custom localization for file browser popup -> use that function:
            // WebGLFileBrowser.SetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select file for loading:");
        }

        private void OnDestroy()
        {
            WebGLFileBrowser.FilesWereOpenedEvent -= FilesWereOpenedEventHandler;
            WebGLFileBrowser.FilePopupWasClosedEvent -= FilePopupWasClosedEventHandler;
            WebGLFileBrowser.FileOpenFailedEvent -= FileOpenFailedEventHandler;
            WebGLFileBrowser.FileWasSavedEvent -= FileWasSavedEventHandler;
            WebGLFileBrowser.FileSaveFailedEvent -= FileSaveFailedEventHandler;
        }

        private void SaveOpenedFileButtonOnClickHandler()
        {
            if (_loadedFiles != null && _loadedFiles.Length > 0)
                WebGLFileBrowser.SaveFile(_loadedFiles[0]);

            // if you want to save custom file use this flow:
            //File file = new File()
            //{
            //    fileInfo = new FileInfo()
            //    {
            //        fullName = "Myfile.txt"
            //    },
            //    data = System.Text.Encoding.UTF8.GetBytes("my text content!")
            //};
            //WebGLFileBrowser.SaveFile(file);
        }

        private void OpenFileDialogButtonOnClickHandler()
        {
            // you could paste types like: ".png,.jpg,.pdf,.txt,.json"
            // WebGLFileBrowser.OpenFilePanelWithFilters(".png,.jpg,.heif");
            WebGLFileBrowser.OpenFilePanelWithFilters(
                WebGLFileBrowser.GetFilteredFileExtensions(_enteredFileExtensions));
        }

        private void CleanupButtonOnClickHandler()
        {
            _loadedFiles =
                null; // you have to remove link to file and then GarbageCollector will think that there no links to that object
            saveOpenedFileButton.gameObject.SetActive(false);
            cleanupButton.gameObject.SetActive(false);

            fileInfoText.text = string.Empty;
            fileNameText.text = string.Empty;
            contentImage.color = new Color(1, 1, 1, 0);
            contentImage.sprite = null;

            WebGLFileBrowser.FreeMemory(); // free used memory and destroy created content
        }

        private void FilesWereOpenedEventHandler(File[] files)
        {
            _loadedFiles = files;

            if (_loadedFiles != null && _loadedFiles.Length > 0)
            {
                var file = _loadedFiles[0];
                var currentIndex = createVk.QuestionDictionary[formsController.idButton].currentIndex;

                fileNameText.text = file.fileInfo.name;
                fileInfoText.text =
                    $"File name: {file.fileInfo.name}\nFile extension: {file.fileInfo.extension}\nFile size: {file.fileInfo.SizeToString()}";
                fileInfoText.text += $"\nLoaded files amount: {files.Length}";

                saveOpenedFileButton.gameObject.SetActive(true);
                cleanupButton.gameObject.SetActive(true);

                if (file.IsImage())
                {
                    contentImage.color = new Color(1, 1, 1, 1);

                    var imageTexture = GetTextureCopy(file.ToSprite().texture);
                    var textureBytes = imageTexture.EncodeToPNG();
                    
                    createVk.QuestionDictionary[formsController.idButton].questionList[currentIndex].uploaded_image =
                        Convert.ToBase64String(textureBytes);
                    
                    // contentImage.sprite = file.ToSprite(); // dont forget to delete unused objects to free memory!

                    createVk.QuestionDictionary[formsController.idButton].answerList[currentIndex] = 4;

                    formsController.saveMsg.SetActive(true);

                    formsController.savedImage.SetActive(true);

                    //WebGLFileBrowser.RegisterFileObject(contentImage
                    //.sprite); // add sprite with texture to cache list. should be used with  WebGLFileBrowser.FreeMemory() when its no need anymore
                }
                else
                {
                    contentImage.color = new Color(1, 1, 1, 0);
                }

                if (file.IsText())
                {
                    var content = file.ToStringContent();
                    fileInfoText.text += $"\nFile content: {content.Substring(0, Mathf.Min(30, content.Length))}";
                }
            }

            _loadedFiles = null;
        }

        private void FilePopupWasClosedEventHandler()
        {
            if (_loadedFiles == null)
                saveOpenedFileButton.gameObject.SetActive(false);
        }

        private void FileWasSavedEventHandler(File file)
        {
            Debug.Log($"file {file.fileInfo.fullName} was saved");
        }

        private void FileSaveFailedEventHandler(string error)
        {
            Debug.Log(error);
        }

        private void FileOpenFailedEventHandler(string error)
        {
            Debug.Log(error);
        }

        private void FilterOfTypesFieldOnValueChangedHandler(string value)
        {
            _enteredFileExtensions = value;
        }

        private Texture2D GetTextureCopy(Texture2D source)
        {
            //Create a RenderTexture
            // Debug.Log("Antes " + source.Size() + "  " + source.Size());
            // Debug.Log(source);
            source.Compress(true);
            // Debug.Log("Después " + source.Size() + "  " + source.Size());
            var rt = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear
            );

            //Copy source texture to the new render (RenderTexture) 
            Graphics.Blit(source, rt);

            //Store the active RenderTexture & activate new created one (rt)
            var previous = RenderTexture.active;
            RenderTexture.active = rt;

            //Create new Texture2D and fill its pixels from rt and apply changes.
            var readableTexture = new Texture2D(source.width, source.height);
            readableTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            readableTexture.Apply();

            //activate the (previous) RenderTexture and release texture created with (GetTemporary( ) ..)
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);

            return readableTexture;
        }
        
        
    }
}
    
    
    