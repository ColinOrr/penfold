using Penfold;

namespace Tests.Features
{
    public class CategorySpecification : Specification
    {
        public CategorySpecification()
        {
            describe["Writing a specification"] = () =>
            {
                context["with a categorised assertion"] = null;
                context["with a categorised context"] = null;
                context["with multiple categories"] = null;
            };
        }
    }
}
