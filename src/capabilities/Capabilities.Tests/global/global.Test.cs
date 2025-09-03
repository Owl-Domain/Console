global using System.Reflection;

global using Microsoft.VisualStudio.TestTools.UnitTesting;

global using OwlDomain.Testing.Assertions;
global using Assert = OwlDomain.Testing.MSTest.Assert;


#if DEBUG
[assembly: Parallelize(Scope = ExecutionScope.ClassLevel, Workers = 1)]
#else
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel, Workers = 0)]
#endif