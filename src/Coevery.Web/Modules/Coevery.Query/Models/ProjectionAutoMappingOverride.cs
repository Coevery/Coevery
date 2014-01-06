using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Coevery.ContentManagement.Records;
using Coevery.Environment.ShellBuilders.Models;
using Coevery.Projections.Models;
using Coevery.Users.Models;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Coevery.Query.Models {
    public class ProjectionAutoMappingOverride : IAutoMappingOverride<ProjectionPartRecord> {

        public void Override(AutoMapping<ProjectionPartRecord> mapping) {
            // public TPartRecord TPartRecord {get;set;}
            var dynamicMethod = new DynamicMethod("UserProjectionRecords", typeof (List<UserProjectionRecord>), null, typeof (ProjectionPartRecord));
            var syntheticMethod = new SyntheticMethodInfo(dynamicMethod, typeof (ProjectionPartRecord));
            var syntheticProperty = new SyntheticPropertyInfo(syntheticMethod);

            // record => record.TPartRecord
            var parameter = Expression.Parameter(typeof (ProjectionPartRecord), "record");
            var syntheticExpression = (Expression<Func<ProjectionPartRecord, IEnumerable<UserProjectionRecord>>>) Expression.Lambda(
                typeof (Func<ProjectionPartRecord, IEnumerable<UserProjectionRecord>>),
                Expression.Property(parameter, syntheticProperty),
                parameter);

            mapping.HasMany(syntheticExpression)
                .Access.NoOp()
                .KeyColumn("ProjectionId")
                .Cascade.AllDeleteOrphan();
        }

        /// <summary>
        /// Synthetic method around a dynamic method. We need this so that we can
        /// override the "static" method attributes, and also return a valid "DeclaringType".
        /// </summary>
        public class SyntheticMethodInfo : MethodInfo {
            private readonly DynamicMethod _dynamicMethod;
            private readonly Type _declaringType;

            public SyntheticMethodInfo(DynamicMethod dynamicMethod, Type declaringType) {
                _dynamicMethod = dynamicMethod;
                _declaringType = declaringType;
            }

            public override object[] GetCustomAttributes(bool inherit) {
                return _dynamicMethod.GetCustomAttributes(inherit);
            }

            public override bool IsDefined(Type attributeType, bool inherit) {
                return IsDefined(attributeType, inherit);
            }

            public override ParameterInfo[] GetParameters() {
                return _dynamicMethod.GetParameters();
            }

            public override MethodImplAttributes GetMethodImplementationFlags() {
                return _dynamicMethod.GetMethodImplementationFlags();
            }

            public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) {
                return _dynamicMethod.Invoke(obj, invokeAttr, binder, parameters, culture);
            }

            public override MethodInfo GetBaseDefinition() {
                return _dynamicMethod.GetBaseDefinition();
            }

            public override ICustomAttributeProvider ReturnTypeCustomAttributes {
                get { return ReturnTypeCustomAttributes; }
            }

            public override string Name {
                get { return _dynamicMethod.Name; }
            }

            public override Type DeclaringType {
                get { return _declaringType; }
            }

            public override Type ReflectedType {
                get { return _dynamicMethod.ReflectedType; }
            }

            public override RuntimeMethodHandle MethodHandle {
                get { return _dynamicMethod.MethodHandle; }
            }

            public override MethodAttributes Attributes {
                get { return _dynamicMethod.Attributes & ~MethodAttributes.Static; }
            }

            public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
                return Enumerable.Empty<Attribute>().ToArray();
                //return _dynamicMethod.GetCustomAttributes(attributeType, inherit);
            }

            public override Type ReturnType {
                get { return _dynamicMethod.ReturnType; }
            }
        }

        /// <summary>
        /// Synthetic property around a method info (the "getter" method).
        /// This is a minimal implementation enabling support for AutoMapping.References.
        /// </summary>
        public class SyntheticPropertyInfo : PropertyInfo {
            private readonly MethodInfo _getMethod;

            public SyntheticPropertyInfo(MethodInfo getMethod) {
                _getMethod = getMethod;
            }

            public override object[] GetCustomAttributes(bool inherit) {
                throw new NotImplementedException();
            }

            public override bool IsDefined(Type attributeType, bool inherit) {
                return false;
                //throw new NotImplementedException();
            }

            public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) {
                throw new NotImplementedException();
            }

            public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) {
                throw new NotImplementedException();
            }

            public override MethodInfo[] GetAccessors(bool nonPublic) {
                throw new NotImplementedException();
            }

            public override MethodInfo GetGetMethod(bool nonPublic) {
                return _getMethod;
            }

            public override MethodInfo GetSetMethod(bool nonPublic) {
                return null;
            }

            public override ParameterInfo[] GetIndexParameters() {
                throw new NotImplementedException();
            }

            public override string Name {
                get { return _getMethod.Name; }
            }

            public override Type DeclaringType {
                get { return _getMethod.DeclaringType; }
            }

            public override Type ReflectedType {
                get { return _getMethod.ReflectedType; }
            }

            public override Type PropertyType {
                get { return _getMethod.ReturnType; }
            }

            public override PropertyAttributes Attributes {
                get { throw new NotImplementedException(); }
            }

            public override bool CanRead {
                get { return true; }
            }

            public override bool CanWrite {
                get { throw new NotImplementedException(); }
            }

            public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
                return _getMethod.GetCustomAttributes(attributeType, inherit);
            }

            public override int MetadataToken {
                get { return 0; }
            }

            public override Module Module {
                get { return null; }
            }

            public override MemberTypes MemberType {
                get { return MemberTypes.Property; }
            }
        }
    }
}
