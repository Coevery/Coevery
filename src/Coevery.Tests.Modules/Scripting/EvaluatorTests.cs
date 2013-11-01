using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Coevery.Scripting.Compiler;

namespace Coevery.Tests.Modules.Scripting {
    [TestFixture]
    public class EvaluatorTests : EvaluatorTestsBase {
        protected override EvaluationResult EvaluateSimpleExpression(string expression, Func<string, IList<object>, object> methodInvocationCallback) {
            var ast = new Parser(expression).Parse();
            foreach (var error in ast.GetErrors()) {
                Trace.WriteLine(string.Format("Error during parsing of '{0}': {1}", expression, error.Message));
            }

            if (ast.GetErrors().Any()) {
                return new EvaluationResult(new Error { Message = ast.GetErrors().First().Message });
            }

            var result = new Interpreter().Evalutate(new EvaluationContext {
                Tree = ast,
                MethodInvocationCallback = methodInvocationCallback
            });
            Trace.WriteLine(string.Format("Result of evaluation of '{0}': {1}", expression, result));
            return result;
        }
    }
}