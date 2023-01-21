namespace RockFests.DAL.Types
{
    public class KeyValue<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public KeyValue() {}
        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}