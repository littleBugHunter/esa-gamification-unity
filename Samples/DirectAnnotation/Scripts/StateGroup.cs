using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Object = UnityEngine.Object;

namespace ImageAnnotation.Samples.DirectAnnotation
{
    public class StateGroup : MonoBehaviour
    {

        [Serializable]
        public class ReferenceValueBase
        {
            [SerializeField]
            public Object m_referenceObject;

            [SerializeField]
            public string m_propertyPath;
            protected PropertyInfo m_propertyInfo;
            public void Initialize()
            {
                if (IsValid())
                {
                    m_propertyInfo = m_referenceObject.GetType().GetProperty(m_propertyPath);
                }
            }

            public bool IsValid()
            {
                return m_referenceObject != null && !string.IsNullOrEmpty(m_propertyPath);
            }
        }

        [Serializable]
        public class ReferenceValue<T> : ReferenceValueBase, ISerializationCallbackReceiver
        {
            public ReferenceValue()
            {
                Initialize();
            }


            public T ReferencedValue
            {
                set { m_propertyInfo.SetValue(m_referenceObject, value); }
                get { return (T) m_propertyInfo.GetValue(m_referenceObject); }
            }


            public void OnBeforeSerialize()
            {
            }

            public void OnAfterDeserialize()
            {
                Initialize();
            }
        }

    
        [Serializable]
        public class BoolReference : ReferenceValue<bool>
        { }
    
        [Serializable]
        public class FloatReference : ReferenceValue<float>
        { }

        [Serializable]
        public class Vector2Reference : ReferenceValue<Vector2>
        { }
    
        [Serializable]
        public class Vector3Reference : ReferenceValue<Vector3>
        { }
    
        [Serializable]
        public class Vector4Reference : ReferenceValue<Vector4>
        { }
    
        [Serializable]
        public class IntReference : ReferenceValue<int>
        { }
    
        [Serializable]
        public class ColorReference : ReferenceValue<Color>
        { }
    
    
        public class StateEntryAttribute : Attribute
        { }

        [Serializable]
        public struct Transition
        {
            public enum Ease
            {
                Linear,
                Constant,
            }
            public float m_duration;
            public Ease  m_ease;
        
            public float Evaluate(float t)
            {
                if (Math.Abs(m_duration) < float.Epsilon)
                    return t < 0 ? 0 : 1;
                t = Mathf.Clamp01(t / m_duration);
                switch (m_ease)
                {
                    case Ease.Constant:
                        return t < 0.5f ? 0 : 1;
                    case Ease.Linear:
                    default:
                        return t;
                }
            }
        }
    
        [Serializable]
        public abstract class EntryBase
        {
            public int m_order;
            public Transition m_transition;
        
            public abstract ReferenceValueBase Reference { get; }
            public abstract void InitValues(int count);
            public abstract Type ValueType { get; }
            public abstract void SwapValues(int index, int index2);
            public abstract void ActivateIndex(int index);
            public abstract void LerpIndices(int indexA, int indexB, float t);
            public abstract void AddValue();
            public abstract void RemoveValue(int index);
            public abstract bool IsReferenceDifferent(int index);
            public abstract void SetAllValuesToReference();
            public abstract void SetValueToReference(int index);
        }
    
        [Serializable]
        public abstract class Entry<TReferenceType, TValueType> : EntryBase where TReferenceType : ReferenceValue<TValueType>, new()
        {
            public TReferenceType m_reference = new TReferenceType();
            public TValueType[]   m_values;
            public override Type ValueType
            {
                get => typeof(TValueType);
            }
        
            public override ReferenceValueBase Reference { get => m_reference; }
            public bool AllValuesTheSame()
            {
                TValueType val = m_values[0];
                foreach (var value in m_values)
                {
                    if (!Equals(value, val))
                        return false;
                }
                return true;
            }

            public override void InitValues(int count)
            {
                m_values = new TValueType[count];
            }
            public override void SetAllValuesToReference()
            {
                for (var index = 0; index < m_values.Length; index++)
                {
                    m_values[index] = m_reference.ReferencedValue;
                }
            }

            public override void SwapValues(int index, int index2)
            {
                TValueType temp = m_values[index];
                m_values[index] = m_values[index2];
                m_values[index2] = temp;
            }

            public override void AddValue()
            {
                Array.Resize(ref m_values, m_values.Length + 1);
                m_values[m_values.Length - 1] = m_reference.ReferencedValue;
            }

            public override void RemoveValue(int index)
            {
                TValueType[] newValues = new TValueType[m_values.Length-1];
                Array.Copy(m_values, newValues, index);
                Array.Copy(m_values, index + 1, newValues, index, newValues.Length - index);
            }
            public override void ActivateIndex(int index)
            {
                if (m_reference.IsValid())
                    m_reference.ReferencedValue = m_values[index];
            }

            public override void LerpIndices(int indexA, int indexB, float t)
            {
                if (m_reference.IsValid())
                    m_reference.ReferencedValue = Lerp(m_values[indexA], m_values[indexB], t);
            }

            protected virtual TValueType Lerp(TValueType a, TValueType b, float t)
            {
                return t < 0.5f ? a : b;
            }

            public override bool IsReferenceDifferent(int index)
            {
                if (!m_reference.IsValid())
                    return false;
                return !Equals(m_reference.ReferencedValue, m_values[index]);
            }

            public override void SetValueToReference(int index)
            {
                if (!m_reference.IsValid())
                    return ;
                m_values[index] = m_reference.ReferencedValue;
            }
        }

        [StateEntry]
        [Serializable]
        public class BoolEntry : Entry<BoolReference, bool>
        {
            protected override bool Lerp(bool a, bool b, float t)
            {
                if (b)
                {
                    return t <= 0 ? a : b;
                }
                else
                {
                    return t < 1 ? a : b;
                }
            }
        }

        [StateEntry]
        [Serializable]
        public class FloatEntry : Entry<FloatReference, float>
        {
            public override bool IsReferenceDifferent(int index)
            {
                if (!m_reference.IsValid())
                    return false;
                return Math.Abs(m_reference.ReferencedValue - m_values[index]) > 0.0001;
            }

            protected override float Lerp(float a, float b, float t)
            {
                return Mathf.Lerp(a, b, t);
            }
        }

        [StateEntry]
        [Serializable]
        public class Vector2Entry : Entry<Vector2Reference, Vector2>
        {
            protected override Vector2 Lerp(Vector2 a, Vector2 b, float t)
            {
                return Vector2.Lerp(a, b, t);
            }
        }

        [StateEntry]
        [Serializable]
        public class Vector3Entry : Entry<Vector3Reference, Vector3>
        { 
            protected override Vector3 Lerp(Vector3 a, Vector3 b, float t)
            {
                return Vector3.Lerp(a, b, t);
            }}

        [StateEntry]
        [Serializable]
        public class Vector4Entry : Entry<Vector4Reference, Vector4>
        {
            protected override Vector4 Lerp(Vector4 a, Vector4 b, float t)
            {
                return Vector4.Lerp(a, b, t);
            }
        }

        [StateEntry]
        [Serializable]
        public class ColorEntry : Entry<ColorReference, Color>
        {
            protected override Color Lerp(Color a, Color b, float t)
            {
                return Color.Lerp(a, b, t);
            }
        }
    
        [StateEntry]
        [Serializable]
        public class IntEntry : Entry<IntReference, int>
        { }

        public void RemoveEntry(EntryBase e)
        {
            Entries.SetDirty();
            if (e is BoolEntry)
                RemoveFromArray(ref m_boolEntries, (BoolEntry)e);
            else if (e is FloatEntry)
                RemoveFromArray(ref m_floatEntries, (FloatEntry)e);
            else if (e is Vector2Entry)
                RemoveFromArray(ref m_vector2Entries, (Vector2Entry)e);
            else if (e is Vector3Entry)
                RemoveFromArray(ref m_vector3Entries, (Vector3Entry)e);
            else if (e is Vector4Entry)
                RemoveFromArray(ref m_vector4Entries, (Vector4Entry)e);
            else if (e is ColorEntry)
                RemoveFromArray(ref m_colorEntries, (ColorEntry)e);
            else if (e is IntEntry)
                RemoveFromArray(ref m_intEntries, (IntEntry)e);
            else throw  new Exception("An Array for Entry Type: " + e.GetType().Name + " was not found!");
            var allEntries = Entries.GetValue();
            foreach (var entry in allEntries)
            {
                if (entry.m_order > e.m_order)
                    entry.m_order--;
            }
        }

        private void RemoveFromArray<T>(ref T[] array, T e) where T : EntryBase
        {
            Entries.SetDirty();
            int index = Array.IndexOf(array, e);
            if (index < 0)
                return;
            for (var i = index; i < array.Length-1; i++)
            {
                array[i] = array[i + 1];
            }
            Array.Resize(ref array, array.Length - 1);
        }

        public void AddEntry(EntryBase e)
        {
            e.m_order = GetAllEntries().Length;
            Entries.SetDirty();
            if (e is BoolEntry)
                AddToArray(ref m_boolEntries, (BoolEntry)e);
            else if (e is FloatEntry)
                AddToArray(ref m_floatEntries, (FloatEntry)e);
            else if (e is Vector2Entry)
                AddToArray(ref m_vector2Entries, (Vector2Entry)e);
            else if (e is Vector3Entry)
                AddToArray(ref m_vector3Entries, (Vector3Entry)e);
            else if (e is Vector4Entry)
                AddToArray(ref m_vector4Entries, (Vector4Entry)e);
            else if (e is ColorEntry)
                AddToArray(ref m_colorEntries, (ColorEntry)e);
            else if (e is IntEntry)
                AddToArray(ref m_intEntries, (IntEntry)e);
            else throw  new Exception("An Array for Entry Type: " + e.GetType().Name + " was not found!");
        }

        private void AddToArray<T>(ref T[] array, T e) where T : EntryBase
        {
            Entries.SetDirty();
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = e;
        }

        public EntryBase[] GetAllEntries()
        {
            var ret =  new List<EntryBase>(m_boolEntries.Length + 
                                                      m_floatEntries.Length + 
                                                      m_vector2Entries.Length + 
                                                      m_vector3Entries.Length + 
                                                      m_vector4Entries.Length + 
                                                      m_colorEntries.Length + 
                                                      m_intEntries.Length);
            ret.AddRange(m_boolEntries);
            ret.AddRange(m_floatEntries);
            ret.AddRange(m_vector2Entries);
            ret.AddRange(m_vector3Entries);
            ret.AddRange(m_vector4Entries);
            ret.AddRange(m_colorEntries);
            ret.AddRange(m_intEntries);
            ret.Sort((a, b) => a.m_order.CompareTo(b.m_order));
            return ret.ToArray();
        }

        public void SelectState(int index)
        {
            if(m_runningTransitionCoroutine != null)
                StopCoroutine(m_runningTransitionCoroutine);
            m_runningTransitionCoroutine = StartCoroutine(TransitionCoroutine(index));
        }

        public void SelectState(string name)
        {
            var index = m_states.FindIndex(s => s == name);
            if (index < 0)
            {
                Debug.LogError("Unable to find State " + name);
                return;
            }
            SelectState(index);
        }

        public void SelectStateImmediately(int index)
        {
            m_selectedState = index;
            var entries = Entries.GetValue();
            foreach (var entryBase in entries)
            {
                entryBase.ActivateIndex(index);
            }
        }

        public void SelectStateImmediately(string name)
        {
            var index = m_states.FindIndex(s => s == name);
            if (index < 0)
            {
                Debug.LogError("Unable to find State " + name);
                return;
            }
            SelectState(index);
        }

        private IEnumerator TransitionCoroutine(int toIndex)
        {
            int fromIndex = m_selectedState;
            m_selectedState = toIndex;
            var entries = Entries.GetValue();
            float totalDuration = 0;
            foreach (var entry in entries)
            {
                totalDuration = Mathf.Max(totalDuration, entry.m_transition.m_duration);
            }

            float time = 0;
            while (time < totalDuration)
            {
                foreach (var entry in entries)
                {
                    float t = entry.m_transition.Evaluate(time);
                    entry.LerpIndices(fromIndex, toIndex, t);
                }

                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            foreach (var entryBase in entries)
            {
                entryBase.ActivateIndex(toIndex);
            }

            m_runningTransitionCoroutine = null;
        }
    
        public BoolEntry[]    m_boolEntries    = new BoolEntry[0];
        public FloatEntry[]   m_floatEntries   = new FloatEntry[0];
        public Vector2Entry[] m_vector2Entries = new Vector2Entry[0];
        public Vector3Entry[] m_vector3Entries = new Vector3Entry[0];
        public Vector4Entry[] m_vector4Entries = new Vector4Entry[0];
        public ColorEntry[]   m_colorEntries   = new ColorEntry[0];
        public IntEntry[]     m_intEntries     = new IntEntry[0];
        public List<string> m_states = new List<string>(new []{"Default"});
        public int m_selectedState = 0;
        public bool IsTransitioning => m_runningTransitionCoroutine != null;
        private CachedValue<EntryBase[]> Entries
        {
            get
            {
                if(!m_entries.IsValid)
                    m_entries = new CachedValue<EntryBase[]>(GetAllEntries);
                return m_entries;
            }
        }
    
        private CachedValue<EntryBase[]> m_entries;
        private Coroutine m_runningTransitionCoroutine;
    }



}