using Penfold;
using System.Linq;
using Should;

namespace Tests.Features
{
    public class CategorySpecification : Specification
    {
        public CategorySpecification()
        {
            describe["Writing a specification"] = () =>
            {
                context["with a categorised assertion"] = () =>
                {
                    var specification = new Specification();

                    before = () =>
                    {
                        specification.@_["category"] =
                            specification.it["does something"] = null;
                    };

                    it["adds the category to the test definition"] = () =>
                    {
                        specification.Tests.Single().Categories.Cast<string>().ShouldContain("category");
                    };
                };

                context["with a categorised context"] = () =>
                {
                    var specification = new Specification();

                    before = () =>
                    {
                        specification.@_["category"] =
                            specification.describe["something"] = () =>
                            {
                                specification.it["does something"] = null;
                            };
                    };

                    it["adds the category to the test definition"] = () =>
                    {
                        specification.Tests.Single().Categories.Cast<string>().ShouldContain("category");
                    };
                };

                context["with multiple categories"] = () =>
                {
                    var specification = new Specification();

                    before = () =>
                    {
                        specification.@_["category 1", "category 2"] =
                            specification.describe["something"] = () =>
                            {
                                specification.@_["category 2", "category 3"] =
                                    specification.it["does something"] = null;
                            };
                    };

                    it["adds each category to the test definition"] = () =>
                    {
                        specification.Tests.Single().Categories.Cast<string>().ShouldContain("category 1");
                        specification.Tests.Single().Categories.Cast<string>().ShouldContain("category 2");
                        specification.Tests.Single().Categories.Cast<string>().ShouldContain("category 3");
                    };


                    it["de-duplicates the categories"] = () =>
                    {
                        specification.Tests.Single().Categories.Cast<string>().Count(s => s == "category 2").ShouldEqual(1);
                    };
                };
            };
        }
    }
}
