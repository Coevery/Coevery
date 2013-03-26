using Coevery.Dynamic;
using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Orchard.FileSystems.VirtualPath;

namespace Coevery.Metadata
{
    public interface IDynamicAssemblyBuilder : IDependency
    {
        IEnumerable<Type> Build(IEnumerable<DynamicTypeDefinition> typeDefinitions);
    }

    public class DynamicAssemblyBuilder : IDynamicAssemblyBuilder
    {
        internal const string AssemblyName = "Coevery.DynamicTypes";
        private readonly IVirtualPathProvider _virtualPathProvider;

        public DynamicAssemblyBuilder(IVirtualPathProvider virtualPathProvider) {
            _virtualPathProvider = virtualPathProvider;
        }

        public IEnumerable<Type> Build(IEnumerable<DynamicTypeDefinition> typeDefinitions)
        {
            var assemblyBuidler = BuildAssembly();
            var moduleBuidler = BuildModule(assemblyBuidler);
            foreach (var definition in typeDefinitions) {
                var typeBuidler = BuildType(definition, moduleBuidler);
                var fieldBuilders = BuildFields(definition, typeBuidler).ToList();
                BuildEmptyCtor(typeBuidler);
                BuildCtor(typeBuidler, fieldBuilders);
                BuildProperties(definition, typeBuidler, fieldBuilders);
                Type type = typeBuidler.CreateType();
            }
            assemblyBuidler.Save(AssemblyName + ".dll");
            var assembly = Assembly.Load(AssemblyName);
            return  assembly.GetTypes();
        }

        private AssemblyBuilder BuildAssembly() {
            AppDomain aDomain = AppDomain.CurrentDomain;

            // Build the assembly
            var asmName = new AssemblyName {Name = AssemblyName};
            var directory = GetAssemblyDirectory();
            var asmBuilder = aDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave, directory);
            return asmBuilder;
        }

        private string GetAssemblyDirectory()
        {
            var virtualPath = _virtualPathProvider.Combine("~/Modules/Coevery.Dynamic", "bin");
            return _virtualPathProvider.MapPath(virtualPath);
        }

        private ModuleBuilder BuildModule(AssemblyBuilder asmBuilder) {
            // Build the module
            var fileName = AssemblyName + ".dll";
            ModuleBuilder modBuilder = asmBuilder.DefineDynamicModule(AssemblyName, fileName);
            return modBuilder;
        }

        private static TypeBuilder BuildType(DynamicTypeDefinition definition, ModuleBuilder modBuilder) {
            // Build the type
            var typeName = string.Format("{0}.{1}.{2}PartRecord", AssemblyName, "Records", definition.Name);
            var typBuilder = modBuilder.DefineType(typeName,
                                                   TypeAttributes.Public |
                                                   TypeAttributes.Class |
                                                   TypeAttributes.AutoClass |
                                                   TypeAttributes.AnsiClass |
                                                   TypeAttributes.BeforeFieldInit |
                                                   TypeAttributes.AutoLayout);
            return typBuilder;
        }

        private static IEnumerable<FieldBuilder> BuildFields(DynamicTypeDefinition definition, TypeBuilder typBuilder) {
            return definition.Fields.Select(field => typBuilder.DefineField("_" + field.Name, field.Type,
                                                                            FieldAttributes.Private | FieldAttributes.InitOnly));
        }

        private static void BuildEmptyCtor(TypeBuilder typBuilder)
        {
            var ctorBuilder = typBuilder.DefineConstructor(
              MethodAttributes.Public |
              MethodAttributes.SpecialName |
              MethodAttributes.RTSpecialName,
              CallingConventions.Standard,
              new Type[0]);

            var generator = ctorBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            var baseCtorInfo = typeof(Object).GetConstructor(new Type[0]);
            generator.Emit(OpCodes.Call, baseCtorInfo);
            generator.Emit(OpCodes.Ret);
        }

        private static void BuildCtor(TypeBuilder typBuilder,
                                      List<FieldBuilder> fieldBuilders) {
            Type[] ctorParams = fieldBuilders.Select(f => f.FieldType).ToArray();
            var ctorBuilder = typBuilder.DefineConstructor(
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                ctorParams);

            ILGenerator ctorIL = ctorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);

            //Get the base constructor
            var baseCtorInfo = typeof (Object).GetConstructor(new Type[0]);
            ctorIL.Emit(OpCodes.Call, baseCtorInfo);

            for (byte i = 0; i < fieldBuilders.Count; i++) {
                ctorIL.Emit(OpCodes.Ldarg_0);
                if (i == 0) {
                    ctorIL.Emit(OpCodes.Ldarg_1);
                }
                else if (i == 1) {
                    ctorIL.Emit(OpCodes.Ldarg_2);
                }
                else if (i == 2) {
                    ctorIL.Emit(OpCodes.Ldarg_3);
                }
                else {
                    ctorIL.Emit(OpCodes.Ldarg_S, i + 1);
                }
                ctorIL.Emit(OpCodes.Stfld, fieldBuilders[i]);
            }
            ctorIL.Emit(OpCodes.Ret);
        }

        private static void BuildProperties(DynamicTypeDefinition definition,
                                            TypeBuilder typBuilder,
                                            List<FieldBuilder> fieldBuilders) {
            var fields = definition.Fields.ToList();
            for (int i = 0; i < fields.Count; i++) {
                var propBuilder = typBuilder.DefineProperty(
                    fields[i].Name, PropertyAttributes.HasDefault, fields[i].Type, Type.EmptyTypes);

                // Build Get prop
                var getMethBuilder = typBuilder.DefineMethod(
                    "get_" + fields[i].Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    fields[i].Type, Type.EmptyTypes);
                var generator = getMethBuilder.GetILGenerator();

                generator.Emit(OpCodes.Ldarg_0); // load 'this'
                generator.Emit(OpCodes.Ldfld, fieldBuilders[i]); // load the field
                generator.Emit(OpCodes.Ret);
                propBuilder.SetGetMethod(getMethBuilder);

                // Build Set prop
                var setMethBuilder = typBuilder.DefineMethod(
                    "set_" + fields[i].Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    typeof (void), new[] {fieldBuilders[i].FieldType});
                generator = setMethBuilder.GetILGenerator();

                generator.Emit(OpCodes.Ldarg_0); // load 'this'
                generator.Emit(OpCodes.Ldarg_1); // load value
                generator.Emit(OpCodes.Stfld, fieldBuilders[i]);
                generator.Emit(OpCodes.Ret);
                propBuilder.SetSetMethod(setMethBuilder);
            }
        }

    }
}
