using System.Text.Json.Serialization;

namespace MoneyFlowClient.Client.Models
{
    public class Budget
    {
        [JsonPropertyName("budgetId")]
        public int BudgetId { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [JsonPropertyName("amount")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Amount { get; set; }

        [JsonPropertyName("period")]
        public string Period { get; set; }
    }
}
