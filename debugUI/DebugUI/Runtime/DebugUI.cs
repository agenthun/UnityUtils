using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace DebugWidget.UI.Runtime
{
    public class DebugUI : MonoBehaviour
    {
        [SerializeField] public float HeightPercent = 0.5f;

        [CanBeNull] protected RectTransform ListRectTf;
        [CanBeNull] private RectTransform _scrollRectTransform;
        private GameObject _loading;

        protected virtual void Start()
        {
            var scrollRect = transform.GetComponentInChildren<ScrollRect>();
            _scrollRectTransform = scrollRect.GetComponent<RectTransform>();
            var scrollHeight = transform.GetComponent<RectTransform>().rect.height * HeightPercent;
            var sizeDelta = _scrollRectTransform.rect;
            _scrollRectTransform.sizeDelta = _scrollRectTransform.sizeDelta
                + new Vector2(sizeDelta.width, scrollHeight) - sizeDelta.size;
            ListRectTf = scrollRect.content;
            InitStaticItems();
        }

        protected virtual void InitStaticItems()
        {
        }

        protected virtual void ControlLoading(bool enable)
        {
            if (_loading == null)
            {
                _loading = transform.CreateLoading();
            }

            _loading?.SetActive(enable);
        }

        protected virtual void ControlPanelUI(bool enable)
        {
            if (_scrollRectTransform)
            {
                _scrollRectTransform.gameObject.SetActive(enable);
            }

            if (!enable && _loading)
            {
                _loading.SetActive(false);
            }
        }

        protected virtual void OnDestroy()
        {
            GameObject.Destroy(_loading);
        }
    }
}