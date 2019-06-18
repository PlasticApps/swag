using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SW
{
    [DefaultExecutionOrder(-10)]
    public class EmotionController : MonoBehaviour, ISerializationCallbackReceiver
    {
        public HeroEmotion[] emotions;

        public Dictionary<Emotions, Sprite> _cacheSprites = new Dictionary<Emotions, Sprite>();

        Image _image;

        void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetEmotion(Emotions emotion, bool isHero = false)
        {
            if (_cacheSprites.ContainsKey(emotion))
                _image.sprite = _cacheSprites[emotion];

            transform.localScale = new Vector3(isHero ? -1 : 1, 1, 1);
        }

        [ContextMenu("Test Random Emotion")]
        public void TestEmotion()
        {
            SetEmotion(emotions[Random.Range(0, emotions.Length)].emotion, Random.value > 0.5f);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            foreach (HeroEmotion heroEmotion in emotions)
                _cacheSprites[heroEmotion.emotion] = heroEmotion.sprite;
        }
    }
}