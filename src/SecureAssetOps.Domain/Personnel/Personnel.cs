using SecureAssetOps.Domain.Common;
using SecureAssetOps.Domain.Personnel.Events;

namespace SecureAssetOps.Domain.Personnel;

public sealed class Personnel : AggregateRoot
{
    public string EmployeeNumber { get; private set; }

    public string FullName { get; private set; }

    public string Email { get; private set; }

    public PersonnelStatus Status { get; private set; }

    private Personnel(
        Guid id,
        string employeeNumber,
        string fullName,
        string email)
        : base(id)
    {
        EmployeeNumber = employeeNumber;
        FullName = fullName;
        Email = email;
        Status = PersonnelStatus.Active;
    }

    public static Personnel Register(
        Guid id,
        string employeeNumber,
        string fullName,
        string email)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
        {
            throw new ArgumentException(
                "Employee number cannot be empty.",
                nameof(employeeNumber));
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException(
                "Full name cannot be empty.",
                nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email cannot be empty.",
                nameof(email));
        }

        string normalizedEmployeeNumber =
            employeeNumber.Trim();

        string normalizedFullName =
            fullName.Trim();

        string normalizedEmail =
            email.Trim().ToLowerInvariant();

        var personnel = new Personnel(
            id,
            normalizedEmployeeNumber,
            normalizedFullName,
            normalizedEmail);

        personnel.RaiseDomainEvent(
            new PersonnelRegisteredDomainEvent(
                personnel.Id));

        return personnel;
    }
}
