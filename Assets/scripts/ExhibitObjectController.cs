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
            string dataName = exhibitData != null ? exhibitData.name : "SIN ExhibitData";
            Debug.Log($"[ExhibitObjectController] Awake en '{gameObject.name}' | Data: {dataName}", this);
        }

        public void PlaySpawnSequence()
        {
            if (_animator == null)
            {
                Debug.LogWarning($"[ExhibitObjectController] _animator es null en '{gameObject.name}'", this);
                return;
            }

            string dataName = exhibitData != null ? exhibitData.name : "SIN ExhibitData";
            Debug.Log($"[ExhibitObjectController] PlaySpawnSequence() llamado en '{gameObject.name}' | Data: {dataName} | ActiveInHierarchy: {gameObject.activeInHierarchy}", this);

            _animator.ResetTrigger(SpawnTrigger);
            _animator.SetTrigger(SpawnTrigger);
        }
    }
}