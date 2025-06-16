using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyFlowClient.Client.Models
{
    public class Budget
    {
        public int BudgetId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }

        [Column(TypeName = "decimal(10, 2)")] 
        public decimal Amount { get; set; }

        public string period { get; set; }
    }
}
