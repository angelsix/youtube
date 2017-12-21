using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DelegatesMethodsLambdas
{
    class Program
    {
        // Defines a method that returns an int, and has one int as an input
        // Delegate defines the signature (return type and parameters)
        public delegate int Manipulate(int a);

        // Action is just a delegate
        public delegate void MyAction();

        // Func is just a delegate with a return type
        public delegate int MyFunc();

        static void Main(string[] args)
        {
            // Invoking a normal method
            var normalMethodInvokeResult = NormalMethod(2);

            // Create an instance of the delegate
            var normalMethodDelegate = new Manipulate(NormalMethod);
            var normalResult = normalMethodDelegate(3);

            // Pass a delegate method as a variable
            var anotherResult = RunAnotherMethod(normalMethodDelegate, 4);

            // Anonymous method is a a delegate() { } and it returns a delegate
            Manipulate anonymousMethodDelegate = delegate (int a) { return a * 2; };
            var anonymousResult = anonymousMethodDelegate(3);

            // Lambda expressions are anything with => and a left/right value
            // They can return a delegate (so a method that can be invoked)
            // or an Expression of a delegate (so it can be compiled and then executed)
            Manipulate lambaDelegate = a => a * 2;
            var lambaResult = lambaDelegate(5);

            // Nicer way to write a lamba
            Manipulate nicerLambaDelegate = (a) => { return a * 2; };
            var nicerLambaResult = nicerLambaDelegate(6);

            // Lamba can return an Expression
            Expression<Manipulate> expressionLambda = a => a * 2;

            // An Action is just a delegate with no return type and optional input
            Action actionDelegate = () => { lambaDelegate(2); };
            Action<int> action2Delegate = (a) => { var b = a * 2; };
            
            // A Func is just a delegate with a return type
            Func<int> myFunc = () => 2;

            // Replace Manipulate with a Func
            Func<int, int> funcDelegate = a => a * 2;
            var funcResult = funcDelegate(5);

            // Mimic the FirstOrDefault Linq expression
            var items = new List<string>(new[] { "a", "b", "c", "d", "e", "f", "g"});
            var itemInts = Enumerable.Range(1, 10).ToList();

            // Calling the nuilt in Linq FirstOrDefault
            var foundItem = items.FirstOrDefault(item => item == "c");

            // Calling our version
            var foundItem2 = items.GetFirstOrDefault(item => item == "c");
            var foundItem3 = itemInts.GetFirstOrDefault(item => item > 4);
        }

        /// <summary>
        /// A normal looking method
        /// </summary>
        /// <param name="a">The input value</param>
        /// <returns>Returns twice the input value</returns>
        public static int NormalMethod(int a)
        {
            return a * 2;
        }

        /// <summary>
        /// Accept a method (delegate) as an input
        /// </summary>
        /// <param name="theMethod"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int RunAnotherMethod(Manipulate theMethod, int a)
        {
            return theMethod(a);
        }
    }

    /// <summary>
    /// Helpers classes
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Mimic the behaviour of the FirstOrDefault Linq expression
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="items"></param>
        /// <param name="findMatch"></param>
        /// <returns></returns>
        public static TResult GetFirstOrDefault<TResult>(this List<TResult> items, Func<TResult, bool> findMatch)
        {
            // Loop each item
            foreach (var item in items)
            {
                // See if caller method has found a match
                if (findMatch(item))
                    // If so return it
                    return item;
            }

            // If there was no match, return default value of return type
            return default(TResult);
        }
    }
}
