using UnityEngine;

namespace MuseumAR.Data
{
    [CreateAssetMenu(fileName = "Exhibit_New", menuName = "Museum AR/Exhibit Data")]
    public class ExhibitData : ScriptableObject
    {
        [Header("Información obligatoria")]
        public string exhibitName;

        [TextArea(3, 6)]
        public string description;

        public string category;
        public string periodOrYear;
        public Sprite icon;

        [TextArea(2, 5)]
        public string extraInfoOrFunFact;

        [Header("Audio de la ficha técnica")]
        public AudioClip themeTrack;
    }
}
