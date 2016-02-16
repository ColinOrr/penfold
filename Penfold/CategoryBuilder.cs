using System;
using System.Linq;

namespace Penfold
{
    /// <summary>
    /// Allows steps in a specification to be categorised using the format
    /// @_["category a", "category b"] = keyword["step title"] () => { ... }
    /// </summary>
    public class CategoryBuilder
    {
        private readonly SpecificationBase specification;

        public CategoryBuilder(SpecificationBase specification)
        {
            this.specification = specification;
        }

        public Action this[params string[] categories]
        {
            set
            {
                var step = specification.Context.Steps.Last();
                step.Categories.AddRange(categories);
            }
        }
    }
}
