using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReflectionExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Reflection Example...");

            var type = typeof(ModelExample);

            // Class name
            Console.WriteLine($"Class: {type.Name}");

            // Namespace
            Console.WriteLine($"Namespace: {type.Namespace}");

            // Get a list of properties in a class (public)
            var listOfProperties = type.GetProperties();
            foreach (var p in listOfProperties)
            {
                Console.WriteLine($"Property {p.Name} from {type.Name}");
            }

            type = typeof(ClassExample);

            // Get list of constructors
            var listOfConstructors = type.GetConstructors();
            foreach (var c in listOfConstructors)
            {
                Console.WriteLine($"Constructor {c} from {type.Name}");
            }

            // Get list of public methods
            var listOfMethods = type.GetMethods();
            foreach (var m in listOfMethods)
            {
                Console.WriteLine($"Method {m} from {type.Name}");
            }


            var testClass = new ModelExample();
            testClass.Id = 1;
            testClass.StringProperty = "Hello";
            testClass.BoolProperty = false;

            var stringToChange = "This is a string that should be updated. Replace the token values with the values in the test class.\n" +
                "The id property is <ModelExample.Id> and the string property is <ModelExample.StringProperty>";

            // Before
            Console.WriteLine($"The resulting string is:\n\n{stringToChange}");

            // After
            TokenReplacer(ref stringToChange, testClass);
            Console.WriteLine($"The resulting string is:\n\n{stringToChange}");
        }

        // Some example of how to use reflection in the real world...
        // This example will only cater for properties to the depth of [1]. For example [Class.Property]
        public static void TokenReplacer(ref string stringToModify, ModelExample m)
        {
            // Extract the string between the wildcards < and >
            var s = stringToModify;
            Regex r = new Regex(@"<(.+?)>");
            MatchCollection mc = r.Matches(s);

            // Get the properties to replace in the format [Class.Property]
            var classAndProps = mc.Select(x => x.Groups[1].Value).ToList();

            foreach (var cp in classAndProps)
            {
                // Split the Class and Property
                var split = cp.Split(".");

                // Get the type
                // Todo: fix assembly part
                var t = GetType($"ReflectionExample.{split[0]}");

                // Get the property name
                var p = split[1];

                // Get the property value in a string
                var pv = t.GetProperty(p).GetValue(m).ToString();

                // Replace the tokens
                stringToModify = stringToModify.Replace($"<{cp}>", $"***{pv}***");
            }
        }

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}
