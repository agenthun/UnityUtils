using UI.CornerImage.Runtime;
using UnityEditor;
using UnityEditor.UI;

namespace UI.CornerImage.Editor
{
    [CustomEditor(typeof(UICornerImage), true)]
    public class UICornerImageEditor : ImageEditor
    {
        SerializedProperty m_Radius;
        SerializedProperty m_Border;
        SerializedProperty m_TriangleNum;
        SerializedProperty m_Sprite;
        SerializedProperty m_EnableLeftTop;
        SerializedProperty m_EnableRightTop;
        SerializedProperty m_EnableLeftBottom;
        SerializedProperty m_EnableRightBottom;


        protected override void OnEnable()
        {
            base.OnEnable();

            m_Sprite = serializedObject.FindProperty("m_Sprite");
            m_Radius = serializedObject.FindProperty("Radius");
            m_Border = serializedObject.FindProperty("Border");
            m_TriangleNum = serializedObject.FindProperty("TriangleNum");
            m_EnableLeftTop = serializedObject.FindProperty("EnableLeftTop");
            m_EnableRightTop = serializedObject.FindProperty("EnableRightTop");
            m_EnableLeftBottom = serializedObject.FindProperty("EnableLeftBottom");
            m_EnableRightBottom = serializedObject.FindProperty("EnableRightBottom");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(m_Radius);
            EditorGUILayout.PropertyField(m_Border);
            EditorGUILayout.PropertyField(m_TriangleNum);
            EditorGUILayout.PropertyField(m_EnableLeftTop);
            EditorGUILayout.PropertyField(m_EnableRightTop);
            EditorGUILayout.PropertyField(m_EnableLeftBottom);
            EditorGUILayout.PropertyField(m_EnableRightBottom);

            serializedObject.ApplyModifiedProperties();
        }
    }
}