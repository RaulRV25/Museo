using MuseumAR.AR;
using MuseumAR.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MuseumAR.UI
{
    public class ExhibitSelectionAndCardUI : MonoBehaviour
    {
        [Header("Cámara y selección")]
        [SerializeField] private Camera arCamera;
        [SerializeField] private LayerMask exhibitLayerMask = ~0;

        [Header("UI de ficha técnica")]
        [SerializeField] private GameObject cardPanel;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text categoryText;
        [SerializeField] private TMP_Text periodOrYearText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Button toggleFunFactButton;
        [SerializeField] private GameObject funFactContainer;
        [SerializeField] private TMP_Text funFactText;
        [SerializeField] private Button closeButton;

        [Header("Audio")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource exhibitAudioSource;
        [SerializeField, Range(0f, 1f)] private float bgmNormalVolume = 0.2f;
        [SerializeField, Range(0f, 1f)] private float bgmDuckVolume = 0.06f;

        private bool _isFunFactVisible;

        private void Awake()
        {
            if (arCamera == null) arCamera = Camera.main;
            if (cardPanel != null) cardPanel.SetActive(false);
            if (funFactContainer != null) funFactContainer.SetActive(false);

            if (toggleFunFactButton != null) toggleFunFactButton.onClick.AddListener(ToggleFunFact);
            if (closeButton != null) closeButton.onClick.AddListener(CloseCard);

            if (bgmSource != null)
            {
                bgmSource.loop = true;
                bgmSource.volume = bgmNormalVolume;
                if (!bgmSource.isPlaying && bgmSource.clip != null) bgmSource.Play();
            }
        }

        private void Update()
        {
            if (!TryGetPointerPosition(out Vector2 screenPos)) return;
            if (cardPanel != null && cardPanel.activeSelf) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if (arCamera == null) arCamera = Camera.main;
            if (arCamera == null) return;

            Ray ray = arCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, exhibitLayerMask))
            {
                var controller = hit.collider.GetComponentInParent<ExhibitObjectController>();
                if (controller != null && controller.Data != null)
                {
                    OpenCard(controller.Data);
                }
            }
        }

        private static bool TryGetPointerPosition(out Vector2 position)
        {
            position = Vector2.zero;

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                position = Touchscreen.current.primaryTouch.position.ReadValue();
                return true;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                position = Mouse.current.position.ReadValue();
                return true;
            }

            return false;
        }

        public void OpenCard(ExhibitData data)
        {
            if (data == null || cardPanel == null) return;

            if (iconImage != null) iconImage.sprite = data.icon;
            if (nameText != null) nameText.text = data.exhibitName;
            if (categoryText != null) categoryText.text = data.category;
            if (periodOrYearText != null) periodOrYearText.text = data.periodOrYear;
            if (descriptionText != null) descriptionText.text = data.description;
            if (funFactText != null) funFactText.text = data.extraInfoOrFunFact;

            _isFunFactVisible = false;
            if (funFactContainer != null) funFactContainer.SetActive(false);

            cardPanel.SetActive(true);

            if (bgmSource != null) bgmSource.volume = bgmDuckVolume;

            if (exhibitAudioSource != null)
            {
                exhibitAudioSource.Stop();
                if (data.themeTrack != null)
                {
                    exhibitAudioSource.clip = data.themeTrack;
                    exhibitAudioSource.loop = true;
                    exhibitAudioSource.Play();
                }
            }
        }

        private void ToggleFunFact()
        {
            _isFunFactVisible = !_isFunFactVisible;
            if (funFactContainer != null) funFactContainer.SetActive(_isFunFactVisible);
        }

        public void CloseCard()
        {
            if (cardPanel != null) cardPanel.SetActive(false);

            if (exhibitAudioSource != null)
            {
                exhibitAudioSource.Stop();
                exhibitAudioSource.clip = null;
            }

            if (bgmSource != null) bgmSource.volume = bgmNormalVolume;
        }
    }
}