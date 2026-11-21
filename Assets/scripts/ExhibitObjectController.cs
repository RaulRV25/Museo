using MuseumAR.Data;
using UnityEngine;

namespace MuseumAR.AR
{
    [RequireComponent(typeof(Animator))]
    public class ExhibitObjectController : MonoBehaviour
    {
        private static readonly int SpawnTrigger = Animator.StringToHash("Spawn");

        [SerializeField] private ExhibitData exhibitData;
        private Animator _animator;

        public ExhibitData Data => exhibitData;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void PlaySpawnSequence()
        {
            if (_animator == null) return;
            _animator.ResetTrigger(SpawnTrigger);
            _animator.SetTrigger(SpawnTrigger);
        }
    }
}