﻿namespace Tvl.VisualStudio.Language.Java.SourceData
{
    using System;
    using System.Diagnostics.Contracts;
    using Path = System.IO.Path;
    using System.Linq;

    public class CodePhysicalFile : CodeElement
    {
        internal CodePhysicalFile(string fileName)
            : base(Path.GetFileName(fileName), fileName, new CodeLocation(fileName), null)
        {
            Contract.Requires(!string.IsNullOrEmpty(fileName));
        }

        public string PackageName
        {
            get
            {
                CodePackageStatement packageStatement = Children.OfType<CodePackageStatement>().FirstOrDefault();
                if (packageStatement == null)
                    return string.Empty;

                return packageStatement.FullName;
            }
        }
    }
}