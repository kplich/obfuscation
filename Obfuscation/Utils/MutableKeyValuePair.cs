namespace Obfuscation.Utils
{
    public class MutableKeyValuePair<TKey, TValue>
    {
        public MutableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; set; }

        public TValue Value { get; set; }
    }
}