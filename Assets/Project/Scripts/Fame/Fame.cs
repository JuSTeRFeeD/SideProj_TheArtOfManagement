namespace Project.Scripts.Fame
{
    public class Fame
    {
        public int Value { get; private set; }

        public int RightAnswersCount { get; private set; }
        public int WrongAnswersCount { get; private set; }
        
        public delegate void OnChange(int oldValue, int newValue);
        public event OnChange OnChangeEvent;
        
        public void Add(int value)
        {
            if (value > 0) RightAnswersCount++;
            else WrongAnswersCount++;
            
            Value += value;
            OnChangeEvent?.Invoke(Value - value, Value);
        }

        public void Reset()
        {
            RightAnswersCount = 0;
            WrongAnswersCount = 0;
            Value = 0;
            OnChangeEvent?.Invoke(Value, Value);
        }
    }
}