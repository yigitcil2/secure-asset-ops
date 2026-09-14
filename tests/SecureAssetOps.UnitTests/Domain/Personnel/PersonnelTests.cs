using SecureAssetOps.Domain.Personnel;
using SecureAssetOps.Domain.Personnel.Events;
using Xunit;

using PersonnelAggregate =
    SecureAssetOps.Domain.Personnel.Personnel;

namespace SecureAssetOps.UnitTests.Domain.Personnel;

public sealed class PersonnelTests
{
    [Fact]
    public void Register_WithValidData_ShouldCreateActivePersonnel()
    {
        Guid id = Guid.NewGuid();

        var personnel = PersonnelAggregate.Register(
            id,
            "EMP-0012",
            "Ahmet Yılmaz",
            "ahmet.yilmaz@company.com");

        Assert.Equal(id, personnel.Id);
        Assert.Equal(
            "EMP-0012",
            personnel.EmployeeNumber);

        Assert.Equal(
            "Ahmet Yılmaz",
            personnel.FullName);

        Assert.Equal(
            "ahmet.yilmaz@company.com",
            personnel.Email);

        Assert.Equal(
            PersonnelStatus.Active,
            personnel.Status);
    }

    [Fact]
    public void Register_WithEmptyEmployeeNumber_ShouldThrowArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => PersonnelAggregate.Register(
                    Guid.NewGuid(),
                    "   ",
                    "Ahmet Yılmaz",
                    "ahmet.yilmaz@company.com"));

        Assert.Equal(
            "employeeNumber",
            exception.ParamName);
    }

    [Fact]
    public void Register_WithEmptyFullName_ShouldThrowArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => PersonnelAggregate.Register(
                    Guid.NewGuid(),
                    "EMP-0012",
                    "   ",
                    "ahmet.yilmaz@company.com"));

        Assert.Equal(
            "fullName",
            exception.ParamName);
    }

    [Fact]
    public void Register_WithEmptyEmail_ShouldThrowArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => PersonnelAggregate.Register(
                    Guid.NewGuid(),
                    "EMP-0012",
                    "Ahmet Yılmaz",
                    "   "));

        Assert.Equal(
            "email",
            exception.ParamName);
    }

    [Fact]
    public void Register_WithWhitespace_ShouldNormalizeValues()
    {
        var personnel = PersonnelAggregate.Register(
            Guid.NewGuid(),
            "  EMP-0012  ",
            "  Ahmet Yılmaz  ",
            "  Ahmet.Yilmaz@Company.COM  ");

        Assert.Equal(
            "EMP-0012",
            personnel.EmployeeNumber);

        Assert.Equal(
            "Ahmet Yılmaz",
            personnel.FullName);

        Assert.Equal(
            "ahmet.yilmaz@company.com",
            personnel.Email);
    }

    [Fact]
    public void Register_WithValidData_ShouldRaisePersonnelRegisteredDomainEvent()
    {
        Guid id = Guid.NewGuid();

        var personnel = PersonnelAggregate.Register(
            id,
            "EMP-0012",
            "Ahmet Yılmaz",
            "ahmet.yilmaz@company.com");

        PersonnelRegisteredDomainEvent domainEvent =
            Assert.IsType<PersonnelRegisteredDomainEvent>(
                Assert.Single(personnel.DomainEvents));

        Assert.Equal(
            id,
            domainEvent.PersonnelId);
    }
    [Fact]  
    public void Deactivate_WhenPersonnelIsActive_ShouldSetStatusToInactive()
    {
        var personnel = CreateActivePersonnel();
        personnel.ClearDomainEvents();
        personnel.Deactivate();

        Assert.Equal(PersonnelStatus.Inactive, personnel.Status);

        PersonnelDeactivatedDomainEvent domainEvent = Assert.IsType<PersonnelDeactivatedDomainEvent>(
            Assert.Single(personnel.DomainEvents));

        Assert.Equal(personnel.Id, domainEvent.PersonnelId);
    }
    [Fact]
    public void Deactivate_WhenPersonnelIsInactive_ShouldThrowInvalidOperationException()
    {
        var personnel = CreateActivePersonnel();

        personnel.Deactivate();
        personnel.ClearDomainEvents();

        Action act = () => personnel.Deactivate();

        Assert.Throws<InvalidOperationException>(act);

        Assert.Equal(
            PersonnelStatus.Inactive,
            personnel.Status);

        Assert.Empty(personnel.DomainEvents);
    }
    [Fact]
    public void Reactivate_WhenPersonnelIsInactive_ShouldSetStatusToActive()
    {
        var personnel = CreateActivePersonnel();

        personnel.Deactivate();
        personnel.ClearDomainEvents();

        personnel.Reactivate();

        Assert.Equal(
            PersonnelStatus.Active,
            personnel.Status);

        PersonnelReactivatedDomainEvent domainEvent =
            Assert.IsType<PersonnelReactivatedDomainEvent>(
                Assert.Single(personnel.DomainEvents));

        Assert.Equal(
            personnel.Id,
            domainEvent.PersonnelId);
    }
    private static PersonnelAggregate CreateActivePersonnel()
    {
        return PersonnelAggregate.Register(
            Guid.NewGuid(),
            "EMP-0012",
            "Ahmet Yılmaz",
            "ahmet.yilmaz@company.com");
    }


}
