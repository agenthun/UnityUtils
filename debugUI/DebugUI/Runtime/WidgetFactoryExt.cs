using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DebugWidget.UI.Runtime
{
    public static class WidgetFactoryExt
    {
        public static Button CreateButton(this Transform parent, string txt, UnityAction<Button, Text> onClick)
        {
            var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/BtnTest"), parent);
            go.name = $"Btn_{txt}(Clone)";
            var text = go.transform.GetComponentInChildren<Text>();
            if (text) text.text = txt;
            var btn = go.transform.GetComponent<Button>();
            btn.onClick.AddListener(() => { onClick?.Invoke(btn, text); });
            return btn;
        }

        public static GameObject CreateLoading(this Transform parent)
        {
            var go = GameObject.Instantiate(
                Resources.Load<GameObject>(
                    "Animated Loading Icons/Prefabs/Circle half rotating/Circle half rotating 1"),
                parent);
            return go;
        }
    }
}