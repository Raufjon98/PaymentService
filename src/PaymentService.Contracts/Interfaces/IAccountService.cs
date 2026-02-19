using MagicOnion;
using PaymentService.Contracts.Account.Requests;
using PaymentService.Contracts.Account.Responses;
using PaymentService.Contracts.Transaction.Responses;

namespace PaymentService.Contracts.Interfaces;

public interface IAccountService : IService<IAccountService>
{
    UnaryResult<AccountResponse> CreateAccountAsync(Guid customerId);
    UnaryResult<BalanceOperationResponse> TopUpBalanceAsync(TopUpRequest topUpRequest);
    UnaryResult<AccountResponse> UpdateAccountStatusAsync(UpdateAccountStatusRequest updateAccountStatusRequest);
    UnaryResult<BalanceOperationResponse> WithdrawBalanceAsync(WithdrawRequest withdrawRequest);
    UnaryResult<BalanceOperationResponse> GetCustomerBalanceAsync(Guid customerId);
    UnaryResult<List<TransactionResult>> GetCustomerToUpsAsync(Guid customerId);
    UnaryResult<List<TransactionResult>> GetCustomerWithdrawsAsync(Guid customerId);
    UnaryResult<List<TransactionResult>> GetCustomerTransactionsAsync(Guid customerId);
    UnaryResult<bool> DeleteAccountAsync(Guid customerId);
}