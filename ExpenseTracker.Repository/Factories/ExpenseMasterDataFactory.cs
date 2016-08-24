using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Repository.Factories
{
    public class ExpenseMasterDataFactory
    {
        public ExpenseGroupStatus CreateExpenseGroupStatus(DTO.ExpenseGroupStatus expenseGroupStatus)
        {
            return new ExpenseGroupStatus
            {
                Description = expenseGroupStatus.Description,
                Id = expenseGroupStatus.Id
            };
        }


        public DTO.ExpenseGroupStatus CreateExpenseGroupStatus(ExpenseGroupStatus expenseGroupStatus)
        {
            return new DTO.ExpenseGroupStatus
            {
                Description = expenseGroupStatus.Description,
                Id = expenseGroupStatus.Id
            };
        }
    }
}