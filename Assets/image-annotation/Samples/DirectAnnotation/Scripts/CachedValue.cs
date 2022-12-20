namespace Util
{
    public struct CachedValue<T>
    {
        public delegate T GetValueDelegate();
        private T m_value;
        private bool m_isDirty;
        private GetValueDelegate m_getValue;
        public readonly bool IsValid;
        

        public CachedValue( GetValueDelegate getValue)
        {
            m_getValue = getValue;
            m_value = getValue.Invoke();
            m_isDirty = false;
            IsValid = true;
        }

        public void SetDirty()
        {
            m_isDirty = true;
        }

        public T GetValue()
        {
            if (m_isDirty)
            {
                m_value   = m_getValue.Invoke();
                m_isDirty = false;
            }
            return m_value;
        }
        
        public static implicit operator T(CachedValue<T> v)
        {
            return v.GetValue();
        }
    }
}