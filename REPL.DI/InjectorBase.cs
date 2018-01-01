namespace REPL.DI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public enum ContainerMode
    {
        ReturnsLast,
        ReturnsFirst
    }

    public abstract class InjectorBase<T, InstanceType>
    {
        public Dictionary<Type, List<InstanceType>> Container = new Dictionary<Type, List<InstanceType>>();

        private ContainerMode _containerMode = ContainerMode.ReturnsLast;
        public ContainerMode ContainerMode
        {
            get { return _containerMode; }
            set { _containerMode = value; }
        }

        public void Bind(Type type, InstanceType instance) {
            if (!Container.ContainsKey(type)
                || !Container.TryGetValue(type, out var value)
                || value == null) {
                Container[type] = new List<InstanceType>();
            }
            Container[type].Add(instance);
        }

        public InstanceType Get(Type type) {
            if (!Container.TryGetValue(type, out var instances) || instances == null) {
                return default(InstanceType);
            }
            switch (ContainerMode) {
                case ContainerMode.ReturnsLast:
                return instances.LastOrDefault();
                case ContainerMode.ReturnsFirst:
                return instances.FirstOrDefault();
                default:
                throw new NotSupportedException("Not supported mode: " + ContainerMode.ToString());
            }

        }
    }
}
