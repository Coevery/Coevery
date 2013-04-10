using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace Orchard.ContentManagement.FieldStorage.ContentPartFieldStorage {
    public class DefaultCotentPartFieldStorage : IFieldStorage {
        private readonly ContentPart _contentPart;

        public DefaultCotentPartFieldStorage(ContentPart contentPart) {
            _contentPart = contentPart;
        }

        public T Get<T>(string name) {
            if (string.IsNullOrEmpty(name))
                return default(T);
            var getter = _getters.GetOrAdd(name, n =>
                                                 CallSite<Func<CallSite, object, dynamic>>.Create(
                                                     Binder.GetMember(CSharpBinderFlags.None, n, _contentPart.GetType(),
                                                                      new[] {CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)})));

            var result = getter.Target(getter, _contentPart);

            if (result == null)
                return default(T);

            var converter = _converters.GetOrAdd(typeof (T), CompileConverter);
            var argument = converter.Invoke(result);
            return argument;
        }

        public void Set<T>(string name, T value) {

            var setter = _setters.GetOrAdd(name, CompileSetter);
            setter(_contentPart, value);
        }

        private static readonly ConcurrentDictionary<string, CallSite<Func<CallSite, object, dynamic>>> _getters =
            new ConcurrentDictionary<string, CallSite<Func<CallSite, object, dynamic>>>();

        private static readonly ConcurrentDictionary<string, Action<object,object>> _setters =
            new ConcurrentDictionary<string, Action<object,object>>();

        private static readonly ConcurrentDictionary<Type, Func<dynamic, object>> _converters =
            new ConcurrentDictionary<Type, Func<dynamic, object>>();

        private static Func<dynamic, object> CompileConverter(Type targetType) {
            var valueParameter = Expression.Parameter(typeof (object), "value");

            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.Dynamic(Binder.Convert(CSharpBinderFlags.ConvertExplicit, targetType, null),
                        targetType,
                        valueParameter),
                    typeof (object)),
                valueParameter).Compile();
        }

        // part=>part.Title = "title"
        private static Action<object, object> CompileSetter(string propertyName) {
            var targetParameter = Expression.Parameter(typeof (object), "part");
            var valueParameter = Expression.Parameter(typeof (object), "value");

            return Expression.Lambda<Action<object, object>>(
                Expression.Dynamic(
                    Binder.SetMember(
                        CSharpBinderFlags.None,
                        propertyName,
                        typeof (void),
                        new[] {
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                        }),
                    typeof (void),
                    targetParameter,
                    valueParameter),
                targetParameter,
                valueParameter).Compile();
        }
    }
}
