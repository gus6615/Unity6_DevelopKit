using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DevelopKit.Editor
{
    public enum HubButtonPriority
    {
        Default,
        BasicTemplate,
        Backend
    }
    
    public abstract class HubButton
    {
// test code
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract HubButtonPriority Priority { get; }

        protected List<PackageDependency> _dependencies;
        public abstract List<PackageDependency> Dependencies { get; }
        
        public virtual bool IsInstalled() => PackageInstaller.CheckSampleInstalled(Name);
        public virtual void OnClickedInstall() => PackageInstaller.CreateSample(Name);
        public virtual void OnClickedDelete() => PackageInstaller.RemoveSample(Name);
    }
}