using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Penfold.TestDriven
{
    class Utilities
    {
        public static string GetWhereForTarget(Assembly assembly, string ns)
        {
            if (string.IsNullOrEmpty(ns))
            {
                return null;
            }

            return toWhereClause(ns);
        }

        public static string GetWhereForTarget(Assembly assembly, MemberInfo member)
        {
            if (member is Type)
            {
                var whereClauseList = new List<string>();
                var targetType = (Type)member;
                var types = includeNestedTypes(targetType);
                types = includeConcreteTypes(assembly, types);
                foreach (var type in types)
                {
                    var whereClause = toWhereClause(type);
                    whereClauseList.Add(whereClause);
                }

                return orWhereClauses(whereClauseList);
            }

            if (member is MethodInfo)
            {
                var whereClauseList = new List<string>();
                MethodInfo methodInfo = (MethodInfo)member;
                var targetTypes = new Type[] { methodInfo.DeclaringType };
                var types = includeConcreteTypes(assembly, targetTypes);
                foreach (var type in types)
                {
                    var whereClause = toWhereClause(type, methodInfo);
                    whereClauseList.Add(whereClause);
                }

                return orWhereClauses(whereClauseList);
            }

            return null; // no tests
        }

        static string orWhereClauses(ICollection<string> whereClauses)
        {
            var where = string.Empty;
            foreach (var whereClause in whereClauses)
            {
                if (where != string.Empty)
                {
                    where += " || ";
                }

                where += whereClause;
            }

            return where;
        }

        static string toWhereClause(Type type)
        {
            return string.Format("(class == '{0}')", type.FullName);
        }

        static string toWhereClause(Type type, MethodInfo method)
        {
            if (type.IsGenericTypeDefinition)
            {
                // this doesn't work with explicit tests
                return string.Format("class == '{0}' && method == '{1}'", type.FullName, method.Name);
            }

            var testPath = toTestPath(type, method);
            return toWhereClause(testPath);
        }

        static string toTestPath(Type type, MethodInfo methodInfo)
        {
            return toTestPath(type) + "." + methodInfo.Name;
        }

        static string toTestPath(Type type)
        {
            var testPath = type.FullName;
            if (!type.IsGenericTypeDefinition)
            {
                return testPath;
            }

            testPath = type.FullName.Split('`')[0];
            testPath += "<";
            foreach (var arg in type.GetGenericArguments())
            {
                if (!testPath.EndsWith("<"))
                {
                    testPath += ",";
                }

                testPath += arg.Name;
            }
            testPath += ">";
            return testPath;
        }

        static Type[] includeNestedTypes(Type type)
        {
            var types = new List<Type>();
            includeNestedTypes(types, type);
            return types.ToArray();
        }

        static void includeNestedTypes(List<Type> typeList, Type type)
        {
            typeList.Add(type);
            foreach (var nestedType in type.GetNestedTypes())
            {
                includeNestedTypes(typeList, nestedType);
            }
        }

        static Type[] includeConcreteTypes(Assembly assembly, Type[] targetTypes)
        {
            var typeList = new List<Type>();
            foreach (var targetType in targetTypes)
            {
                includeConcreteTypes(typeList, assembly, targetType);
            }

            return typeList.ToArray();
        }

        static void includeConcreteTypes(List<Type> typeList, Assembly assembly, Type targetType)
        {
            if (targetType.IsAbstract && !targetType.IsSealed) // static classes are abstract and sealed
            {
                foreach (Type candidateType in assembly.GetExportedTypes())
                {
                    if (targetType.IsAssignableFrom(candidateType) && !candidateType.IsAbstract)
                    {
                        typeList.Add(candidateType);
                    }
                }

                return;
            }

            typeList.Add(targetType);
        }

        static string toWhereClause(string testPath)
        {
            return string.Format("(test == '{0}')", testPath);
        }
    }
}
