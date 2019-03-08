using Grpc.Core;
using Primes;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Preview.gRPC.Server.Services
{
    public class PrimeCalculatorService : PrimeCalculator.PrimeCalculatorBase
    {

        public async override Task<IsPrimeReply> IsPrime(NumberRequest request, ServerCallContext context)
        {          
            bool isPrime = true;
            Parallel.For(2, request.Value / 2, (divisor, state) =>
            {
                if (request.Value % divisor == 0)
                {
                    isPrime = false;
                    state.Break();
                }

            });           
           
            return new IsPrimeReply()
            {
                IsPrime = isPrime
            };
        }
    }
}