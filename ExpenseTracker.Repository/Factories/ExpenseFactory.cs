using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using ExpenseTracker.DTO;

namespace ExpenseTracker.Repository.Factories
{
    public class ExpenseFactory
    {
        public Expense CreateExpense(Entities.Expense expense)
        {
            return new Expense
            {
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                ExpenseGroupId = expense.ExpenseGroupId,
                Id = expense.Id
            };
        }

        public Entities.Expense CreateExpense(Expense expense)
        {
            return new Entities.Expense
            {
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                ExpenseGroupId = expense.ExpenseGroupId,
                Id = expense.Id
            };
        }
        
        public object CreateDataShapedObject(Entities.Expense expense, List<string> lstOfFields)
        {
            return CreateDataShapedObject(CreateExpense(expense), lstOfFields);
        }
        
        public object CreateDataShapedObject(Expense expense, List<string> lstOfFields)
        {
            if (!lstOfFields.Any())
            {
                return expense;
            }
            // create a new ExpandoObject & dynamically create the properties for this object

            var objectToReturn = new ExpandoObject();
            foreach (string field in lstOfFields)
            {
                // need to include public and instance, b/c specifying a binding flag overwrites the
                // already-existing binding flags.

                object fieldValue = expense.GetType()
                    .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                    .GetValue(expense, null);

                // add the field to the ExpandoObject
                ((IDictionary<string, object>) objectToReturn).Add(field, fieldValue);
            }

            return objectToReturn;
        }
    }
}