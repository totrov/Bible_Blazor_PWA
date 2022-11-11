using Bible_Blazer_PWA.DataBase.DTO;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static Bible_Blazer_PWA.DataSources.LessonDS;

namespace Bible_Blazer_PWA.Helpers
{
    public static class WorkerMessageSerialization
    {
        private static PatternCollection patternCollection;
        static WorkerMessageSerialization()
        {
            patternCollection = new();
            patternCollection
                .Register<LessonDTO>(
                    (l => l.Name, '|'),
                    (l => l.UnitId, '|'),
                    (l => l.Id, '|'),
                    (l => l.Content, '|'))
                .Register<LessonLightweightDTO>(
                    (l => l.Name, '|'),
                    (l => l.UnitId, '|'),
                    (l => l.Id, '|')
                );
        }
        public static string GenerateJSSerializationString<T>(bool lightweight)
        {
            StringBuilder sb = new();
            foreach (var member in patternCollection.Resolve<T>())
            {
                (Expression<Func<T, string>> expression, char delimiter) = member;
                sb.Append(
                    $"+json.{(expression.Body as MemberExpression).Member.Name} + '{delimiter}'"
                    );
            }
            return sb.ToString();
        }

        public static T Deserialize<T>(string input) where T : new()
        {
            T t = new();
            int pos = 0;
            StringBuilder sb = new();
            char ch = 'a';
            foreach (var member in patternCollection.Resolve<T>())
            {
                (Expression<Func<T, string>> expression, char delimiter) = member;
                while (pos < input.Length)
                {
                    ch = input[pos++];
                    if (ch == delimiter) break;
                    else sb.Append(ch);
                }

                if (expression.Body is not MemberExpression memberexpression)
                {
                    throw new Exception("lambda for parsing must be presented as member expression");
                }
                Expression.Lambda<Action<T>>(
                    Expression.Assign(memberexpression, Expression.Constant(sb.ToString())),
                    expression.Parameters
                    ).Compile()(t);
                sb.Clear();
            }
            return t;
        }

        private class PatternCollection: Dictionary<Type, object>
        {
            public PatternCollection Register<T>(params (Expression<Func<T, string>>, char)[] pattern)
            {
                Add(typeof(T), pattern);
                return this;
            }
            public IEnumerable<(Expression<Func<T, string>>, char)> Resolve<T>()
                => (IEnumerable<(Expression<Func<T, string>>, char)>)this[typeof(T)];
        }
    }
}
