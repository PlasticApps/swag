using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SW
{
    public class DialogueController : MonoBehaviour
    {
        public delegate void DialogueInited(DialogueController ctrl);

        public delegate void DialogueCompleted();

        public event DialogueCompleted OnCompleted;

        public AudioClip[] backgroAudioClips;
        public Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        public event DialogueInited OnDialogueInit;

        [Serializable]
        public struct ToolTip
        {
            public GameObject container;
            public Text title;
            public Text message;
        }

        public ToolTip toolTipDictor;
        public ToolTip toolTipPerson;

        [Serializable]
        public struct StepRow
        {
            public GameObject row;
            public Button step;
            public Text stepTxt;
        }

        public bool IsInited { get; set; }

        public DialogueContainer DialogueContainer
        {
            get { return _dialogueContainer; }
            set
            {
                _dialogueContainer = value;
                IsInited = true;
                OnDialogueInit?.Invoke(this);
            }
        }

        [Serializable]
        public struct AnimationUI
        {
            public float bgFadeTime;

            public float layoutSpeed;
            public Tween.EaseType layoutEase;

            public float stepTime;
            public float stepDelay;
            public Tween.EaseType stepEase;
        }

        public AnimationUI anim;

        public GameManager manager { get; set; }
        public EmotionController emotionController;
        public Person[] persons;
        public StepRow[] steps;

        DialogueContainer _dialogueContainer;
        VerticalLayoutGroup _layout;
        private RectTransform _layoutRectTransform;

        private Dialogue current;
        private Dialogue last;

        private int _layoutPositionX;
        private int _layoutPositionY;

        void Awake()
        {
            IsInited = false;
            _layout = GetComponent<VerticalLayoutGroup>();
            _layoutRectTransform = _layout.GetComponent<RectTransform>();

            foreach (AudioClip audioClip in backgroAudioClips)
                clips[audioClip.name] = audioClip;

            UpdateLayout(3000);
            HideAll();
        }

        private void OnEnable()
        {
            for (int i = 0; i < steps.Length; i++)
            {
                int stepId = i;
                steps[i].step.onClick.AddListener(() => { OnStepChoosed(stepId); });
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < steps.Length; i++)
                steps[i].step.onClick.RemoveAllListeners();
        }

        public void OnStepChoosed(int stepId)
        {
            if (stepId < 0)
                Debug.Log("Finished");
            else
                ShowDialogue(current.steps[stepId].dialogue_id);
        }

        void HideAllPersons(int showIndex = -1)
        {
            for (int i = 0; i < persons.Length; i++)
                persons[i].gameObject.SetActive(showIndex == i);

            emotionController.gameObject.SetActive(showIndex >= 0);
        }

        public void HideAllSteps()
        {
            foreach (StepRow stepRow in steps)
                stepRow.step.gameObject.SetActive(false);
        }

        [ContextMenu("Hide All")]
        public void HideAll()
        {
            HideAllPersons();
            HideAllSteps();
            toolTipDictor.container.SetActive(false);
            toolTipPerson.container.SetActive(false);
        }

        void ToggleToolTip(Dialogue dialogue)
        {
            bool isDictor = dialogue.isDictor;
            toolTipDictor.container.SetActive(isDictor);
            toolTipPerson.container.SetActive(!isDictor);

            if (isDictor) // Dictor
            {
                _layout.childAlignment = TextAnchor.LowerCenter;
                toolTipDictor.title.text = string.IsNullOrEmpty(dialogue.title) ? "Dictor" : dialogue.title;
                toolTipDictor.message.text = dialogue.message;
            }
            else
            {
                if (dialogue.isHero) // Player
                    _layout.childAlignment = TextAnchor.LowerLeft;
                else
                    _layout.childAlignment = TextAnchor.LowerRight;

                toolTipPerson.title.text = string.IsNullOrEmpty(dialogue.title)
                    ? persons[dialogue.person].name
                    : dialogue
                        .title;
                toolTipPerson.message.text = dialogue.message;
            }
        }

        public void ShowDialogue(int index = 0)
        {
            ShowDialogue(_dialogueContainer.dialogues[index]);
        }

        public void ShowDialogue(Dialogue dialogue)
        {
            last = current;
            current = dialogue;

            manager.audio.Play(manager.GetAudio(_dialogueContainer.GetMusicName(dialogue.music)));

            if (last == null)
                MoveLayout(0, current.isDictor ? -3000 : 3000, !current.isDictor, BgTransition);
            else if (last != null)
            {
                if (current.person != last.person)
                {
                    if (_layoutPositionX == 0 && !last.isDictor)
                        MoveLayout(0, 3000, true, BgTransition);
                    else if (_layoutPositionY == 0 && last.isDictor)
                        MoveLayout(0, -3000, false, BgTransition);
                    else
                        BgTransition();
                }
                else
                {
                    ToggleSteps(true);
                    Tween.Delay((steps.Length + 1) * anim.stepDelay + anim.stepTime, BgTransition).Start();
                }
            }
            else
                BgTransition();
        }

        void ToggleSteps(bool hide, float delta = 1)
        {
            for (int i = 0; i < steps.Length; i++)
                ShowStep(i * anim.stepDelay * delta, i, hide);
        }

        void BgTransition()
        {
            manager.bg.FadeInOut(anim.bgFadeTime, _dialogueContainer.GetBackgroundName(current.background), OnBgFaded);
        }

        void MoveLayout(float from, float to, bool vertical, Action onComplete)
        {
            if (to <= 0)
            {
                foreach (StepRow stepRow in steps)
                    stepRow.step.transform.localScale = Vector3.zero;
            }

            Tween.Value(anim.layoutSpeed).From(from).To(to)
                .OnUpdate(f => UpdateLayout(vertical ? (int) f : 0, vertical ? 0 : (int) f))
                .OnComplete(onComplete)
                .Ease(anim.layoutEase)
                .Start();
        }

        void ShowStep(float delay, int stepId, bool isReversed = false)
        {
            Tween.Scale(steps[stepId].step.transform, anim.stepTime, delay)
                .From(isReversed ? Vector3.one : Vector3.zero)
                .To(!isReversed ? Vector3.one : Vector3.zero)
                .Ease(anim.stepEase)
                .Start();
        }

        void UpdateLayout(int x, int y = 0)
        {
            _layoutPositionX = x;
            _layoutPositionY = y;
            RectOffset padding = _layout.padding;
            padding.left = x;
            padding.right = x;
            padding.bottom = y;
            _layout.padding = padding;
            LayoutRebuilder.MarkLayoutForRebuild(_layoutRectTransform);
        }

        void OnBgFaded()
        {
            ToggleToolTip(current);
            HideAllPersons(current.person);

            OnDialogueTransition();
            if (last == null || current.person != last.person)
                MoveLayout(current.isDictor ? -3000 : 3000, 0, !current.isDictor, () => { });

            ToggleSteps(false);

            {
                for (int i = 0, step = current.steps.Count; i < steps.Length; i++)
                {
                    bool isVisible = i < step;
                    StepRow stepRow = steps[i];
                    stepRow.step.gameObject.SetActive(isVisible);
                    if (i < step)
                        stepRow.stepTxt.text = current.steps[i].content;
                }

                if (!current.hasSteps)
                {
                    if (current.isComplete)
                    {
                        DelayAction(current.delay > 0 ? current.delay : 5, () =>
                        {
                            HideAll();
                            manager.audio.Play(null);
                            manager.bg.FadeIn(1.5f, () =>
                            {
                                OnCompleted?.Invoke();
                            });
                        });
                    }
                    else if (current.delay > 0)
                    {
                        DelayAction(current.delay, () => { ShowDialogue(current.id + 1); });
                    }
                }
            }
        }

        void DelayAction(float delay, Action action)
        {
            Tween
                .Delay(delay, action)
                .Start();
        }

        void OnDialogueTransition()
        {
            if (current.isDictor) return;
            if (last == null || current.person == last.person || current.forceEmotion ||
                current.emotion != last.emotion)
            {
                emotionController.SetEmotion(current.emotion, current.isHero);
                persons[current.person].ChangeEmotion(current.emotion);
            }
        }
    }
}