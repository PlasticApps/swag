using UnityEngine;
using UnityEngine.UI;

namespace SW
{
    public class Person : MonoBehaviour
    {
        public bool isMainHero = false;

        public int person_id;
        public string name;
        public HeroEmotion[] emotions;

        public Image hair;
        public Image head;
        public Image body;

        private float _opacity = 1.0f;

        public float Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;

                Color color = Color.white;
                color.a = _opacity;
                hair.color = color;
                head.color = color;
                body.color = color;
            }
        }

        public void ChangeEmotion(Emotions emotion)
        {
            foreach (HeroEmotion heroEmotion in emotions)
            {
                if (heroEmotion.emotion.Equals(emotion))
                {
                    head.sprite = heroEmotion.sprite;
                    break;
                }

                if (heroEmotion.emotion.Equals(Emotions.NEUTRAL))
                    head.sprite = heroEmotion.sprite;
            }
        }
    }
}