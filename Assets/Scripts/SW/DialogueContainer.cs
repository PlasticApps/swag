using System.Collections.Generic;
using UnityEngine;

namespace SW
{
    /// <summary>
    /// Helper Container for generating JSON file
    /// Look -> Editor/DialagueContainerEditor.cs
    /// </summary>
    [CreateAssetMenu(fileName = "Dialogues", menuName = "Dialogues", order = 0)]
    public class DialogueContainer : ScriptableObject
    {
        public string bundleName;
        public uint version;
        public DialoguesSettings settings;
        public List<Dialogue> dialogues;

        public string GetBackgroundName(int index)
        {
            if (settings.backgrounds.Length > index && index >= 0)
                return settings.backgrounds[index];
            return null;
        }

        public string GetMusicName(int index)
        {
            if (settings.musics.Length > index && index >= 0)
                return settings.musics[index];
            return null;
        }
    }
}