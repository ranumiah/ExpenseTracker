using System.Collections.Generic;

namespace ExpenseTracker.DTO
{

    public class ExpenseGroup
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int ExpenseGroupStatusId { get; set; }

        public ICollection<Expense> Expenses { get; set; } 
    }
}
