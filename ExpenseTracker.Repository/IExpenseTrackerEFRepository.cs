namespace ExpenseTracker.Repository
{
    public interface IExpenseTrackerRepository
    {
        RepositoryActionResult<Entities.Expense> DeleteExpense(int id);
        RepositoryActionResult<Entities.ExpenseGroup> DeleteExpenseGroup(int id);
        Entities.Expense GetExpense(int id, int? expenseGroupId = null);
        Entities.ExpenseGroup GetExpenseGroup(int id);
        Entities.ExpenseGroup GetExpenseGroup(int id, string userId);
        System.Linq.IQueryable<Entities.ExpenseGroup> GetExpenseGroups();
        System.Linq.IQueryable<Entities.ExpenseGroup> GetExpenseGroups(string userId);
        Entities.ExpenseGroupStatus GetExpenseGroupStatus(int id);
        System.Linq.IQueryable<Entities.ExpenseGroupStatus> GetExpenseGroupStatusses();
        System.Linq.IQueryable<Entities.ExpenseGroup> GetExpenseGroupsWithExpenses();
        Entities.ExpenseGroup GetExpenseGroupWithExpenses(int id);
        Entities.ExpenseGroup GetExpenseGroupWithExpenses(int id, string userId);
        System.Linq.IQueryable<Entities.Expense> GetExpenses();
        System.Linq.IQueryable<Entities.Expense> GetExpenses(int expenseGroupId);
    
        RepositoryActionResult<Entities.Expense> InsertExpense(Entities.Expense e);
        RepositoryActionResult<Entities.ExpenseGroup> InsertExpenseGroup(Entities.ExpenseGroup eg);
        RepositoryActionResult<Entities.Expense> UpdateExpense(Entities.Expense e);
        RepositoryActionResult<Entities.ExpenseGroup> UpdateExpenseGroup(Entities.ExpenseGroup eg);
    }
}
