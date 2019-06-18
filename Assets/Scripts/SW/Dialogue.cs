using System;
using System.Collections.Generic;
using UnityEngine;

namespace SW
{
    [Serializable]
    public class Dialogue
    {
        /// <summary>
        /// Dialogue Id - used as relation between steps in dialogues
        /// </summary>
        public int id;

        /// <summary>
        /// Character Id
        /// -1 - Dictor
        /// 0 - Main Hero
        /// 1 - Character 2
        /// 2 - Character 3
        /// </summary>
        public int person;

        public bool isDictor => person == -1;

        public bool isHero => person == 0;

        public bool hasSteps => steps.Count > 0;

        /// <summary>
        /// Autoskip in delay
        /// </summary>
        public float delay = 5.0f;

        public int music;
        public int background;

        /// <summary>
        /// Force change emotion if need.
        /// </summary>
        public bool forceEmotion;

        public Emotions emotion;

        [System.Serializable]
        public class DialogueStep
        {
            /// <summary>
            /// Dialogue Id - relation
            /// </summary>
            public int dialogue_id;

            /// <summary>
            /// Answer block message
            /// </summary>
            public string content;
        }

        public string title;

        /// <summary>
        /// Dialogue question - header or title
        /// </summary>
        public string message;

        /// <summary>
        /// Answers list
        /// </summary>
        public List<DialogueStep> steps;

        public bool isComplete = false;
    }
}