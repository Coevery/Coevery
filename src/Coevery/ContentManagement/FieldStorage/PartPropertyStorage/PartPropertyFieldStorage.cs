using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Coevery.ContentManagement.MetaData.Models;
using Microsoft.CSharp.RuntimeBinder;

namespace Coevery.ContentManagement.FieldStorage.PartPropertyStorage {
    public class PartPropertyFieldStorage : IFieldStorage {
        private readonly ContentPart _contentPart;
        private readonly ContentPartFieldDefinition _partFieldDefinition;

        public PartPropertyFieldStorage(ContentPart contentPart, ContentPartFieldDefinition partFieldDefinition) {
            _contentPart = contentPart;
            _partFieldDefinition = partFieldDefinition;
        }

        public T Get<T>(string name) {
            var getter = _getters.GetOrAdd(_partFieldDefinition.Name, n =>
                                                 CallSite<Func<CallSite, object, dynamic>>.Create(
                                                     Binder.GetMember(CSharpBinderFlags.None, n, null,
                                                                      new[] {CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)})));

            var result = getter.Target(getter, _contentPart);

            if (result == null)
                return default(T);

            var converter = _converters.GetOrAdd(typeof (T), CompileConverter);
            var argument = converter.Invoke(result);
            return argument;
        }

        public void Set<T>(string name, T value) {
            var setter = _setters.GetOrAdd(_partFieldDefinition.Name, CompileSetter);
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
