using Grid;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NeighborPatternRegistry.PatternInstance))]
public class PatternInstanceDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 18f + 64f + 12f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            SerializedProperty nameProp = property.FindPropertyRelative("name");
            SerializedProperty textureProp = property.FindPropertyRelative("texture");

            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

            Rect nameRect = new(position.x + 6f, position.y + 6f, position.width - 12f, 18f);
            EditorGUI.PropertyField(nameRect, nameProp, new GUIContent("Pattern Name"));
            float structureWidth = (position.width - 18f) * 0.65f;
            float previewWidth = (position.width - 18f) * 0.35f;

            Rect objectFieldRect = new(
                position.x + 6f, 
                nameRect.yMax + 14f, 
                structureWidth, 
                18f
            );

            Rect texturePreviewRect = new(
                objectFieldRect.xMax + 6f, 
                nameRect.yMax + 4f, 
                previewWidth, 
                64f
            );

            textureProp.objectReferenceValue = EditorGUI.ObjectField(
                objectFieldRect, 
                GUIContent.none, 
                textureProp.objectReferenceValue, 
                typeof(Texture2D), 
                false
            );

            Texture2D tex = textureProp.objectReferenceValue as Texture2D;
            if (tex)
            {
                if (Event.current.type != EventType.Repaint) return;
                EditorGUI.DrawRect(texturePreviewRect, new Color(0.15f, 0.15f, 0.15f, 1f));
                FilterMode oldFilter = tex.filterMode;
                tex.filterMode = FilterMode.Point; 

                GUI.DrawTexture(
                    texturePreviewRect, 
                    tex, 
                    ScaleMode.ScaleToFit, 
                    true
                );

                tex.filterMode = oldFilter;
            }
            else
                EditorGUI.LabelField(texturePreviewRect, "No Texture", EditorStyles.centeredGreyMiniLabel);
        }
    }
}