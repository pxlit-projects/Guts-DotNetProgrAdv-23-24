namespace Exercise2
{
    public class MathOperationFactory : IMathOperationFactory
    {
        public Func<int, long> CreateCubicOperation() //3*x³ + 2*x² + x
        {
            return x => (long)(3 * Math.Pow(x, 3) + 2 * Math.Pow(x, 2) + x);
        }

        public Func<int, long> CreateNthPrimeOperation()
        {
            Func<int, long> operation = (n) => {
                long number = 1;
                int primeCount = 0;
                while (primeCount < n)
                {
                    number++;
                    if (IsPrime(number))
                    {
                        primeCount++;
                    }
                }
                return number;
            };
            return operation;
        }

        private bool IsPrime(long number)
        {
            for (long i = 2; i < number; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}