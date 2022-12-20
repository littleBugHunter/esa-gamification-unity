using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ImageAnnotation.Samples.DirectAnnotation.Editor
{
    [CustomEditor(typeof(StateGroup))]
    public class StateGroupEditor : UnityEditor.Editor
    {
        private StateGroup m_stateGroup;
        private List<StateGroup.EntryBase> m_entryList;
        private static bool s_showReorder;
        
        private void OnEnable()
        {
            m_stateGroup = target as StateGroup;
        }


        public override void OnInspectorGUI()
        {
            m_entryList = new List<StateGroup.EntryBase>(m_stateGroup.GetAllEntries());
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.DisabledScope(m_stateGroup.IsTransitioning))
            {
                DrawStatesOverview();
                DrawValues();
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawValues()
        {
            GUILayout.Label("Values");
            for (var index = 0; index < m_entryList.Count; index++)
            {
                var entry = m_entryList[index];
                GUILayout.BeginHorizontal();
                DrawEntry(entry);
                GUILayout.EndHorizontal();
            }

            if(EditorGUILayout.DropdownButton(new GUIContent("Add"), FocusType.Keyboard))
            {
                GenericMenu menu = new GenericMenu();
                Assembly assembly = typeof(StateGroup).Assembly;
                var referenceTypes =
                    // Partition on the type list initially.
                    from t in assembly.GetTypes().AsParallel()
                    let attributes = t.GetCustomAttributes(typeof(StateGroup.StateEntryAttribute), true)
                    where attributes != null && attributes.Length > 0
                    select new { Type = t, Attributes = attributes.Cast<StateGroup.StateEntryAttribute>() };

                foreach (var type in referenceTypes)
                {
                    menu.AddItem(new GUIContent(type.Type.Name), false, () =>
                    {
                        Undo.RecordObject(target, "Added Value");
                        var types = type.Type.GetGenericArguments();
                        var e = (StateGroup.EntryBase)Activator.CreateInstance(type.Type);
                        e.InitValues(m_stateGroup.m_states.Count);
                        m_entryList.Add(e);
                        m_stateGroup.AddEntry(e);
                        EditorUtility.SetDirty(target);
                    });
                }
                menu.ShowAsContext();
            }
        }

        private void DrawEntry(StateGroup.EntryBase entry)
        {
            DrawReferenceSelector(entry.Reference, entry.ValueType, entry);
            float valueWidth = Mathf.Max(40, EditorGUIUtility.currentViewWidth - 43 - 120 - 150 - 120 - 50);
            if (s_showReorder)
            {
                valueWidth -= 60 + 21 + 21;
            }
            else
            {
                valueWidth += 12;
            }
            if (entry.Reference.IsValid())
            {
                if (entry is StateGroup.BoolEntry)
                {
                    var e = (StateGroup.BoolEntry) entry;
                    var r = (StateGroup.BoolReference)entry.Reference;
                    var newValue = EditorGUILayout.Toggle(r.ReferencedValue, GUILayout.Width(valueWidth));
                    if (newValue != r.ReferencedValue)
                    {
                        Undo.RecordObject(target, "Changed Value");
                        r.ReferencedValue            = newValue;
                        e.m_values[m_stateGroup.m_selectedState] = newValue;
                    }
                }
                else if (entry is StateGroup.FloatEntry)
                {
                    var  e        = (StateGroup.FloatEntry) entry;
                    var  r        = (StateGroup.FloatReference)entry.Reference;
                    var newValue = EditorGUILayout.FloatField(r.ReferencedValue, GUILayout.Width(valueWidth));
                    if (Math.Abs(newValue - r.ReferencedValue) > 0.0001f)
                    {
                        Undo.RecordObject(target, "Changed Value");
                        r.ReferencedValue                        = newValue;
                        e.m_values[m_stateGroup.m_selectedState] = newValue;
                    }
                }
                else if (entry is StateGroup.Vector2Entry)
                {
                    var e        = (StateGroup.Vector2Entry) entry;
                    var r        = (StateGroup.Vector2Reference)entry.Reference;
                    var newValue = EditorGUILayout.Vector2Field(GUIContent.none, r.ReferencedValue, GUILayout.Width(valueWidth));
                    if (!newValue.Equals(r.ReferencedValue))
                    {
                        Undo.RecordObject(target, "Changed Value");
                        r.ReferencedValue                        = newValue;
                        e.m_values[m_stateGroup.m_selectedState] = newValue;
                    }
                }
                else if (entry is StateGroup.Vector3Entry)
                {
                    var e        = (StateGroup.Vector3Entry) entry;
                    var r        = (StateGroup.Vector3Reference)entry.Reference;
                    var newValue = EditorGUILayout.Vector3Field(GUIContent.none, r.ReferencedValue, GUILayout.Width(valueWidth));
                    if (!newValue.Equals(r.ReferencedValue))
                    {
                        Undo.RecordObject(target, "Changed Value");
                        r.ReferencedValue                        = newValue;
                        e.m_values[m_stateGroup.m_selectedState] = newValue;
                    }
                }
                else if (entry is StateGroup.Vector4Entry)
                {
                    var e        = (StateGroup.Vector4Entry) entry;
                    var r        = (StateGroup.Vector4Reference)entry.Reference;
                    var newValue = EditorGUILayout.Vector4Field(GUIContent.none, r.ReferencedValue, GUILayout.Width(valueWidth));
                    if (!newValue.Equals(r.ReferencedValue))
                    {
                        Undo.RecordObject(target, "Changed Value");
                        r.ReferencedValue                        = newValue;
                        e.m_values[m_stateGroup.m_selectedState] = newValue;
                    }
                }
                else if (entry is StateGroup.ColorEntry)
                {
                    var e        = (StateGroup.ColorEntry) entry;
                    var r        = (StateGroup.ColorReference)entry.Reference;
                    var newValue = EditorGUILayout.ColorField(GUIContent.none, r.ReferencedValue, GUILayout.Width(valueWidth));
                    GUILayout.FlexibleSpace();
                    if (!newValue.Equals(r.ReferencedValue))
                    {
                        Undo.RecordObject(target, "Changed Value");
                        r.ReferencedValue                        = newValue;
                        e.m_values[m_stateGroup.m_selectedState] = newValue;
                    }
                }
                else if (entry is StateGroup.IntEntry)
                {
                    var e        = (StateGroup.IntEntry) entry;
                    var r        = (StateGroup.IntReference)entry.Reference;
                    var newValue = EditorGUILayout.IntField(GUIContent.none, r.ReferencedValue, GUILayout.Width(valueWidth));
                    if (!newValue.Equals(r.ReferencedValue))
                    {
                        Undo.RecordObject(target, "Changed Value");
                        r.ReferencedValue                        = newValue;
                        e.m_values[m_stateGroup.m_selectedState] = newValue;
                    }
                }
            }
            else
            {
                GUILayout.Space(valueWidth);
            }

            {
                var newDuration = EditorGUILayout.FloatField(entry.m_transition.m_duration);
                if (Math.Abs(newDuration - entry.m_transition.m_duration) > 0.0001f)
                {
                    Undo.RecordObject(target, "Changed Easing");
                    entry.m_transition.m_duration = newDuration;
                }
                var newEase = (StateGroup.Transition.Ease) EditorGUILayout.EnumPopup(entry.m_transition.m_ease);
                if (newEase != entry.m_transition.m_ease)
                {
                    Undo.RecordObject(target, "Changed Easing");
                    entry.m_transition.m_ease = newEase;
                }
            }

            EditorGUI.BeginDisabledGroup(!entry.IsReferenceDifferent(m_stateGroup.m_selectedState));
            if (GUILayout.Button("Apply", GUILayout.Width(50)))
            {
                Undo.RecordObject(target, "Changed Value");
                entry.SetValueToReference(m_stateGroup.m_selectedState);
            }
            EditorGUI.EndDisabledGroup();

            if (s_showReorder)
            {
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    Undo.RecordObject(target, "Removed Entry");
                    m_stateGroup.RemoveEntry(entry);
                    EditorUtility.SetDirty(target);
                }
                if (entry.m_order > 0)
                {
                    if (GUILayout.Button("▲", GUILayout.Width(21)))
                    {
                        Undo.RecordObject(target, "Moved Entry");
                        entry.m_order--;
                        m_entryList[entry.m_order].m_order++;
                        EditorUtility.SetDirty(target);
                    }
                }
                else
                {
                    GUILayout.Space(25);
                }
                if (entry.m_order < m_entryList.Count - 1)
                {
                    if (GUILayout.Button("▼", GUILayout.Width(21)))
                    {
                        Undo.RecordObject(target, "Moved Entry");
                        entry.m_order++;
                        m_entryList[entry.m_order].m_order--;
                        EditorUtility.SetDirty(target);
                    }
                }
                else
                {
                    GUILayout.Space(25);
                }
            }
        }

        private void DrawReferenceSelector(StateGroup.ReferenceValueBase reference, Type type, StateGroup.EntryBase entry)
        {
            var newReference = EditorGUILayout.ObjectField(reference.m_referenceObject, typeof(Object), GUILayout.Width(120));
            if (newReference != reference.m_referenceObject)
            {
                Undo.RecordObject(target, "Changed Reference");
                reference.m_referenceObject = newReference;
                reference.m_propertyPath = null;
            }

            if (reference.m_referenceObject != null)
            {
                
                string dropdownLabel;
                if (reference.IsValid())
                {
                    dropdownLabel = reference.m_referenceObject.GetType().Name + "/" + reference.m_propertyPath;
                }
                else
                {
                    dropdownLabel = "Select";
                }
                
                if (EditorGUILayout.DropdownButton(new GUIContent(dropdownLabel), FocusType.Keyboard, GUILayout.Width(150)))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.allowDuplicateNames = true;
                    GameObject go;
                    if (reference.m_referenceObject is Component)
                    {
                        go = ((Component) reference.m_referenceObject).gameObject;
                    }
                    else if (reference.m_referenceObject is GameObject)
                    {
                        go = (GameObject) reference.m_referenceObject;
                    }
                    else
                    {
                        return;
                    }

                    void CreateMenuEntriesForObject(Object obj)
                    {
                        var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var property in properties)
                        {
                            if (property.PropertyType == type && property.SetMethod != null)
                            {
                                menu.AddItem(new GUIContent(obj.GetType().Name + "/" + property.Name), false, () =>
                                {
                                    Undo.RecordObject(target, "Changed Reference");
                                    reference.m_referenceObject = obj;
                                    reference.m_propertyPath    = property.Name;
                                    reference.Initialize();
                                    entry.SetAllValuesToReference();
                                });
                            }
                        }
                    }
                    CreateMenuEntriesForObject(go);
                    List<Component> components = new List<Component>();
                    go.GetComponents(components);
                    foreach (var component in components)
                    {
                        CreateMenuEntriesForObject(component);
                    }
                    menu.ShowAsContext();
                }
            }
            else
            {
                GUILayout.Space(158);
            }
        }

        private void DrawStatesOverview()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("States");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(s_showReorder ? "▼" : "◀", GUILayout.Width(21)))
            {
                s_showReorder = !s_showReorder;
            }

            if (s_showReorder)
            {
                GUILayout.Space(63 + 25);
            }
            GUILayout.EndHorizontal();
            for (var index = 0; index < m_stateGroup.m_states.Count; index++)
            {
                var state = m_stateGroup.m_states[index];
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(index == m_stateGroup.m_selectedState, GUIContent.none, GUILayout.Width(30)) && index != m_stateGroup.m_selectedState)
                {
                    ChangeState(index);
                }
                
                m_stateGroup.m_states[index] = EditorGUILayout.TextField(state);
                if (s_showReorder)
                {
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        RemoveState(index);
                        GUILayout.EndHorizontal();
                        break;
                    }

                    if (index > 0)
                    {
                        if (GUILayout.Button("▲", GUILayout.Width(21)))
                        {
                            SwapStates(index, index - 1);
                        }
                    }
                    else
                    {
                        GUILayout.Space(25);
                    }
                    if (index < m_stateGroup.m_states.Count - 1)
                    {
                        if (GUILayout.Button("▼", GUILayout.Width(21)))
                        {
                            SwapStates(index, index + 1);
                        }
                    }
                    else
                    {
                        GUILayout.Space(25);
                    }
                }
                GUILayout.EndHorizontal();
            }
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", GUILayout.Width(24+24)))
            {
                AddState();
            }
            GUILayout.EndHorizontal();
        }

        private void AddState()
        {
            Undo.RecordObject(target, "Added State");
            m_stateGroup.m_states.Add("State (" + (m_stateGroup.m_states.Count + 1) + ")");
            foreach (var entryBase in m_entryList)
            {
                entryBase.AddValue();
            }
        }

        private void SwapStates(int index, int index2)
        {
            Undo.RecordObject(target, "Moved State");
            var temp = m_stateGroup.m_states[index];
            m_stateGroup.m_states[index]  = m_stateGroup.m_states[index2];
            m_stateGroup.m_states[index2] = temp;
            foreach (var entryBase in m_entryList)
            {
                entryBase.SwapValues(index, index2);
            }
        }

        private void ChangeState(int index)
        {
            Undo.RecordObject(target, "Changed State");
            m_stateGroup.SelectStateImmediately(index);
        }

        private void RemoveState(int index)
        {
            Undo.RecordObject(target, "Removed State");
            m_stateGroup.m_states.RemoveAt(index);
            if (index < m_stateGroup.m_selectedState)
            {
                m_stateGroup.m_selectedState--;
            }
            foreach (var entryBase in m_entryList)
            {
                entryBase.RemoveValue(index);
            }
        }
    }
}