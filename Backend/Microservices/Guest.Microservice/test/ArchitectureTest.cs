using FluentAssertions;
using NetArchTest.Rules;

namespace test;

public class ArchitectureTest
{
    private const string DomainNamespace = "Domain";
    private const string InfrastructureNamespace = "Infrastructure";
    private const string ApplicationNamespace = "Application";
    private const string WebApiNamespace = "WebApi";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        var assembly = typeof(Domain.AssemblyReference).Assembly;

        var otherProjects = new[] { InfrastructureNamespace, ApplicationNamespace, WebApiNamespace };

        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Handlers_Should_Have_DependencyOnDomain(){
        var assembly = typeof(Application.AssemblyReference).Assembly;

        var testResult = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }


    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherProjects()
    {
        var assembly = typeof(Application.AssemblyReference).Assembly;

        var otherProjects = new[] { InfrastructureNamespace, ApplicationNamespace, WebApiNamespace };

        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }


    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnOtherProjects()
    {
        var assembly = typeof(Infrastructure.AssemblyReference).Assembly;

        var otherProjects = new[] { WebApiNamespace };

        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Controller_Should_have_DependencyOnMediatR()
    {
        var assembly = typeof(Infrastructure.AssemblyReference).Assembly;

        var testResult = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .HaveDependencyOn("MediatR")
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }
    
}
