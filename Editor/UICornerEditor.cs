using UnityEditor;
using UnityEngine;

namespace IrakliChkuaseli.UI.UICorner
{
    [CustomEditor(typeof(UICorner))]
    public class UICornerEditor : Editor
    {
        private SerializedProperty rProp;
        private SerializedProperty topLeftTypeProp;
        private SerializedProperty topRightTypeProp;
        private SerializedProperty bottomRightTypeProp;
        private SerializedProperty bottomLeftTypeProp;

        private void OnEnable()
        {
            rProp = serializedObject.FindProperty("r");
            topLeftTypeProp = serializedObject.FindProperty("topLeft");
            topRightTypeProp = serializedObject.FindProperty("topRight");
            bottomRightTypeProp = serializedObject.FindProperty("bottomRight");
            bottomLeftTypeProp = serializedObject.FindProperty("bottomLeft");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var component = (UICorner)target;
            var rectTransform = component.GetComponent<RectTransform>();
            float maxSize = 0;

            if (rectTransform != null)
            {
                var rect = rectTransform.rect;
                maxSize = Mathf.Min(rect.width, rect.height) * 0.5f;
            }

            EditorGUILayout.Space(10);

            // Header with info
            var headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("Corner Configuration", headerStyle);

            if (maxSize > 0)
            {
                EditorGUILayout.Space(5);
                var infoStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 11
                };
                EditorGUILayout.LabelField($"Max Size: {maxSize:F1}", infoStyle);
            }

            EditorGUILayout.Space(10);

            DrawCornerSettings("↖ Top Left", rProp.FindPropertyRelative("x"), topLeftTypeProp, maxSize);
            DrawCornerSettings("↗ Top Right", rProp.FindPropertyRelative("y"), topRightTypeProp, maxSize);
            DrawCornerSettings("↘ Bottom Right", rProp.FindPropertyRelative("z"), bottomRightTypeProp, maxSize);
            DrawCornerSettings("↙ Bottom Left", rProp.FindPropertyRelative("w"), bottomLeftTypeProp, maxSize);

            EditorGUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawCornerSettings(string label, SerializedProperty sizeProp, SerializedProperty typeProp,
            float maxSize)
        {
            EditorGUILayout.BeginVertical("box");

            // Horizontal layout: Label + Type dropdown + Slider all in one line
            EditorGUILayout.BeginHorizontal();

            // Corner label with icon (fixed width)
            GUILayout.Label(label, EditorStyles.boldLabel, GUILayout.Width(90));

            // Type dropdown (compact)
            typeProp.enumValueIndex =
                (int)(CornerStyle)EditorGUILayout.EnumPopup((CornerStyle)typeProp.enumValueIndex, GUILayout.Width(80));

            GUILayout.Space(5);

            // Size slider (flexible - takes remaining space)
            GUILayout.Label("Size", GUILayout.Width(35));

            var maxValue = maxSize > 0 ? maxSize : 100f;
            sizeProp.floatValue = GUILayout.HorizontalSlider(sizeProp.floatValue, 0f, maxValue);

            // Editable size input (fixed width)
            var newValue = EditorGUILayout.FloatField(sizeProp.floatValue, GUILayout.Width(40));
            sizeProp.floatValue = Mathf.Clamp(newValue, 0f, maxValue);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(3);
        }
    }
}