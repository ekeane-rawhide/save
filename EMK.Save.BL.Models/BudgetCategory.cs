namespace EMK.Save.BL.Models
{
    public class BudgetCategory
    {
        public Guid Id { get; set; }

        [DisplayName("Shared Budget")]
        public Guid SharedBudgetId { get; set; }

        [DisplayName("Category Name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Icon")]
        public string Icon { get; set; } = string.Empty;

        [DisplayName("Color")]
        public string Color { get; set; } = string.Empty;

        [DisplayName("Category Type")]
        public CategoryType CategoryType { get; set; } = CategoryType.Expense;

        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;

        [DisplayName("Type Label")]
        public string CategoryTypeLabel => CategoryType.ToString();
    }
}
