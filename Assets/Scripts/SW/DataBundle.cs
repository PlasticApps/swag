using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace SW
{
    [System.Serializable]
    public class DataBundle
    {
        public string name;
        public string dialogName => $"{name}.byte";

        public string GetPath()
        {
            return Path.Combine(Application.streamingAssetsPath, "Android/", _dialogueContainer.bundleName);
        }

        AssetBundle _bundle;

        public bool isLoaded = false;

        public AssetBundle bundle => _bundle;
        DialogueContainer _dialogueContainer;
        public DialogueContainer dialog => _dialogueContainer;

        /// <summary>
        /// Load Json file - also can be loaded from asset bundle if need, but by task its imposible
        /// </summary>
        /// <param name="onLoad"></param>
        /// <returns></returns>
        public IEnumerator LoadDialogFile(Action onLoad = null)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", dialogName);

            string data = null;
            if (filePath.Contains("://") || filePath.Contains(":///"))
            {
                using (UnityWebRequest uwr = UnityWebRequest.Get(filePath))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.isNetworkError || uwr.isHttpError)
                    {
                        Debug.LogWarning(uwr.error);
                    }
                    else
                    {
                        data = uwr.downloadHandler.text;
                    }
                }
            }
            else
                data = File.ReadAllText(filePath);

            if (!string.IsNullOrEmpty(data))
            {
                _dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
//                Debug.Log(data);
                JsonUtility.FromJsonOverwrite(data, _dialogueContainer);
            }
            onLoad?.Invoke();
        }

        public IEnumerator LoadBundle(Action onLoad = null)
        {
            while (!Caching.ready)
                yield return null;

            string path = GetPath();
            isLoaded = false;
            if (path.Contains("://") || path.Contains(":///"))
            {
                using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(path, _dialogueContainer.version)
                )
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.isNetworkError || uwr.isHttpError)
                    {
                        Debug.LogWarning(uwr.error);
                    }
                    else
                    {
                        _bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                        isLoaded = true;
                    }
                }
            }
            else
            {
                _bundle = AssetBundle.LoadFromFile(path);
                isLoaded = true;
            }

            onLoad?.Invoke();
        }

        public void Destroy()
        {
            _bundle.Unload(true);
        }
    }
}