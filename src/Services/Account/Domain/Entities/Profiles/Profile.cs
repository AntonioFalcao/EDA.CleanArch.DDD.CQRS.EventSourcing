using Contracts.DataTransferObjects;
using Domain.Abstractions.Entities;

namespace Domain.Entities.Profiles;

public class Profile : Entity<Guid, ProfileValidator>
{
    public Profile(string email, string firstName, string lastName)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public DateOnly? Birthdate { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }


    public void ChangeFirstName(string firstName)
        => FirstName = firstName;

    public void ChangeLastName(string lastName)
        => LastName = lastName;

    public void ChangeBirthdate(DateOnly birthdate)
        => Birthdate = birthdate;

    public void ChangeEmail(string email)
        => Email = email;

    public static implicit operator Profile(Dto.Profile profile)
        => new(profile.Email, profile.FirstName, profile.LastName);

    public static implicit operator Dto.Profile(Profile profile)
        => new(profile.Email, profile.FirstName, profile.LastName);
}