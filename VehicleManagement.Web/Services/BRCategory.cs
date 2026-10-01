using System.Globalization;
using VehicleManagement.Web.Models;

namespace VehicleManagement.Web.Services
{
    public class BRCategory
    {

        public const decimal MinWeightKg = 0m;

        public IReadOnlyList<VehicleCategoryModel> Sort(IEnumerable<VehicleCategoryModel> categories) =>
            categories.OrderBy(c => c.MinWeightKg).ThenBy(c => c.Id).ToList();

        // Validation rules
        public string? ValidateCategory(IEnumerable<VehicleCategoryModel> categories)
        {
            var ordered = Sort(categories).ToList();

            if (ordered.Count == 0)
                return "At least one category is required.";

            if (ordered.Any(c => string.IsNullOrWhiteSpace(c.Name)))
                return "Category must have a name.";

            if (ordered.Any(c => string.IsNullOrWhiteSpace(c.Icon)))
                return "Category must have an icon.";

            if (ordered.Any(c => !IsSingleGrapheme(c.Icon)))
            {
                return "Icon must be a single grapheme.";
            }

            if (ordered.Any(c => c.MinWeightKg < MinWeightKg))
                return "Weights cannot be negative.";

            if (ordered.Any(c => c.MaxWeightKg.HasValue && c.MaxWeightKg.Value <= c.MinWeightKg))
                return "Maximum weight must be greater than minimum weight.";

            if (ordered[0].MinWeightKg != MinWeightKg)
                return "The first category must start at 0 kg.";

            for (var i = 0; i < ordered.Count - 1; i++)
            {
                var current = ordered[i];
                var next = ordered[i + 1];

                if (!current.MaxWeightKg.HasValue)
                    return "Only the final category may have no upper bound.";

                if (current.MaxWeightKg.Value != next.MinWeightKg)
                    return "Category ranges must be contiguous with no gaps or overlaps.";
            }
            
            return null;
        }

        public VehicleCategoryModel? FindCategory(IEnumerable<VehicleCategoryModel> categories, decimal weightKg)
        {
            if (weightKg <= 0) return null;

            return Sort(categories)
                .FirstOrDefault(c =>
                    weightKg >= c.MinWeightKg &&
                    (!c.MaxWeightKg.HasValue || weightKg < c.MaxWeightKg.Value));
        }
        private static bool IsSingleGrapheme(string? icon)
        {
            if (string.IsNullOrWhiteSpace(icon))
                return false;

            var indices = System.Globalization.StringInfo.ParseCombiningCharacters(icon);
            if (indices.Length != 1)
                return false;

            int start = indices[0];
            int length = icon.Length - start;
            var element = icon.Substring(start, length);

            // Reject letters, digits or whitespace (so plain text like "a" or "abc" fail)
            foreach (var ch in element)
            {
                if (char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch))
                    return false;
            }

            // Accept the grapheme if it contains at least one visible non-control character (symbols, emoji, etc.)
            foreach (var ch in element)
            {
                var cat = char.GetUnicodeCategory(ch);
                if (cat != System.Globalization.UnicodeCategory.Control &&
                    cat != System.Globalization.UnicodeCategory.Format)
                    return true;
            }

            return false;
        }
    }
}
