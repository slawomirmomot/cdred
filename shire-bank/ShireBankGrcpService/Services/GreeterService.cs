using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ShireBankGrcpService;

namespace ShireBankGrcpService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<UInt32Value> OpenAccount(OpenAccountRequest request, ServerCallContext context)
        {
            UInt32Value result = new UInt32Value();
            result.Value = 10; //New account number 
            return Task.FromResult(result);
        }

        public override Task<FloatValue> Withdraw(WithdrawRequest request, ServerCallContext context)
        {
            FloatValue result = new FloatValue();
            result.Value = request.Ammount;
            return Task.FromResult(result);
        }
        public override Task<Empty> Deposit(DepositRequest request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }
        public override Task<StringValue> GetHistory(UInt32Value request, ServerCallContext context)
        {
            StringValue result = new StringValue();
            result.Value = "Some history";
            return Task.FromResult(result);
        }
        public override Task<BoolValue> CloseAccount(UInt32Value request, ServerCallContext context)
        {
            Google.Protobuf.WellKnownTypes.BoolValue result = new BoolValue();
            result.Value = true;
            return Task.FromResult(result);
        }
    }
}