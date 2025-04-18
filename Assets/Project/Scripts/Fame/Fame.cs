namespace Project.Scripts.Fame
{
    public class Fame
    {
        public int Value { get; private set; }

        public delegate void OnChange(int oldValue, int newValue);
        public event OnChange OnChangeEvent;
        
        public void Add(int value)
        {
            Value += value;
            OnChangeEvent?.Invoke(Value - value, Value);
        }

        public void Reset()
        {
            Value = 0;
            OnChangeEvent?.Invoke(Value, Value);
        }
    }
}