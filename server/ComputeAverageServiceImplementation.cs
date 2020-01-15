using ComputeAverage;
using Grpc.Core;
using System.Threading.Tasks;
using static ComputeAverage.ComputeAverageService;

namespace server
{
    public class ComputeAverageServiceImplementation : ComputeAverageServiceBase
    {
        public override async Task<ComputeAverageResponse> ComputeAverage(IAsyncStreamReader<ComputeAverageRequest> requestStream, ServerCallContext context)
        {
            try
            {
                var sum = 0.0;
                var total = 0;

                while (await requestStream.MoveNext())
                {
                    sum += requestStream.Current.Number;
                    total++;
                }

                var avg = sum / total;

                return new ComputeAverageResponse() {
                    Result = avg,
                    Total = total,
                    FriendlyMessage = $"You typed {total} numbers and the average of these numbers are: {avg}"
                };
            }
            catch (System.Exception e)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Something went wrong - Exception: {e}"));
            }
        }
    }
}
