
using System.ComponentModel.DataAnnotations;

namespace EMK.Save.BL.Models
{
    /// <summary>
    /// Represents a user-defined budget category (e.g. Groceries, Rent, Entertainment).
    /// Maps to tblBudgetCategory in the PL.
    /// </summary>
    public class BudgetCategory
    {
        public Guid Id { get; set; }

        [DisplayName("Category Name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Icon")]
        public string Icon { get; set; } = string.Empty;          // e.g. "ti-shopping-cart"

        [DisplayName("Color")]
        public string Color { get; set; } = string.Empty;         // hex or CSS var name

        [DisplayName("Category Type")]
        public CategoryType CategoryType { get; set; } = CategoryType.Expense;

        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;

        // ── Computed / denormalized ──────────────────────────────────────────
        [DisplayName("Type Label")]
        public string CategoryTypeLabel => CategoryType.ToString();
    }
}
