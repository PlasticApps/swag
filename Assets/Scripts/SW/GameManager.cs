using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SW
{
    [DefaultExecutionOrder(-9)]
    public class GameManager : MonoBehaviour
    {
        public RectTransform dialogueRoot;
        public DataBundle[] bundles;

        public int bundleIndex = 0;

        public DataBundle next
        {
            get
            {
                if (bundleIndex < bundles.Length)
                    return bundles[bundleIndex];
                return null;
            }
        }

        public DataBundle current { get; private set; }

        public BackgroundController bg;
        public AudioManager audio;
        private Dictionary<string, Sprite> _textures;
        private Dictionary<string, AudioClip> _audios;

        void Awake()
        {
            LoadBundle();
        }

        /// <summary>
        /// Autoload dialogue in queue by bundleIndex
        /// </summary>
        public void LoadBundle()
        {
            if (next != null)
                StartCoroutine(next.LoadDialogFile(OnDialogLoaded));
        }

        void OnDialogLoaded()
        {
            StartCoroutine(next.LoadBundle(OnBundleLoaded));
        }


        public Sprite GetTexture(string name)
        {
            if (_textures.ContainsKey(name))
                return _textures[name];
            return null;
        }

        public AudioClip GetAudio(string name)
        {
            if (_audios.ContainsKey(name))
                return _audios[name];
            return null;
        }

        private DialogueController _dc;

        void OnBundleLoaded()
        {
            current = next;
            Object[] objects = current.bundle.LoadAllAssets();

            _textures = new Dictionary<string, Sprite>();
            _audios = new Dictionary<string, AudioClip>();

            foreach (Object o in objects)
            {
                // load prefabs
                if (o is GameObject)
                {
                    GameObject item = Instantiate(o) as GameObject;
                    RectTransform container = item.GetComponent<RectTransform>();
                    container.SetParent(dialogueRoot, false);

                    DialogueController dc = item.GetComponent<DialogueController>();
                    if (dc != null)
                    {
                        dc.manager = this;
                        dc.DialogueContainer = current.dialog;
                        dc.ShowDialogue();
                        _dc = dc;
                        _dc.OnCompleted += OnComplete;
                    }
                }
                else if (o is Sprite)
                    _textures.Add(o.name, o as Sprite);

                else if (o is AudioClip)
                    _audios.Add(o.name, o as AudioClip);
            }
            bundleIndex++;
        }

        void OnComplete()
        {
            _dc.OnCompleted -= OnComplete;
            Debug.Log("Here we can load next quest!");
            Destroy(_dc.gameObject);

            current.Destroy();
            _textures?.Clear();
            _audios?.Clear();
        }
    }
}