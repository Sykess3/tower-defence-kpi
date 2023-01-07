using System;

namespace DefaultNamespace
{
    public static class PlayerWallet
    {
        public static int Amount { get; private set; } = 200;
        public static event Action Updated;
        public static event Action<int, int> UpdatedWithValues;

        public static void Increase(int amount)
        {
            int oldValue = Amount;
            Amount += amount;
            
            UpdatedWithValues?.Invoke(oldValue, Amount);
            Updated?.Invoke();
        }

        public static void Decrease(int amount)
        {
            int oldValue = Amount;
            Amount -= amount;
            
            UpdatedWithValues?.Invoke(oldValue, Amount);
            Updated?.Invoke();
        }
    }
}