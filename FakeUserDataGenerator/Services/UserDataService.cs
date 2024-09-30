using Bogus;
using FakeUserDataGenerator.Models;

public class UserDataService
{
    public Faker<User> _faker;
    private Random _random;
    private int seed;
    private string _locale;

    public UserDataService(string locale = "en_US")
    {
        _locale = locale;
        _faker = new Faker<User>(locale)
            .RuleFor(u => u.Index, f => f.IndexGlobal)
            .RuleFor(u => u.RandomIdentifier, f => f.Random.Guid().ToString())
            .RuleFor(u => u.FullName, f => f.Name.FullName())
            .RuleFor(u => u.Address, f => $"{f.Address.StreetAddress()} {f.Address.City()}, {f.Address.StateAbbr()} {f.Address.ZipCode()}")
            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber());
    }

    public List<User> GenerateUsers(int count, double errors, int seed, int page)
    {
        _random = new Random(seed);
        var randomSeed = GenerateRandomPage(seed, page);
        var users = _faker.UseSeed(randomSeed).Generate(count);
        var appliedUsers = users.Select(user => ApplyErrors(user, errors)).ToList();
        return users;
    }

    private User ApplyErrors(User user, double errorRate)
    {
        int baseErrors = (int)Math.Floor(errorRate);
        double fractionalPart = errorRate - baseErrors;

        bool applyExtraError = _random.NextDouble() < fractionalPart;

        int totalErrors = baseErrors + (applyExtraError ? 1 : 0);

        List<string> fields = new List<string> { "FullName", "Address", "Phone" };

        for (int i = 0; i < totalErrors; i++)
        {
            string selectedField = fields[_random.Next(fields.Count)];
            int errorType = _random.Next(3); // 0 for delete, 1 for add, 2 for swap

            switch (selectedField)
            {
                case "FullName":
                    user.FullName = ApplyFieldError(user.FullName, errorType);
                    break;
                case "Address":
                    user.Address = ApplyFieldError(user.Address, errorType);
                    break;
                case "Phone":
                    user.Phone = ApplyFieldError(user.Phone, errorType);
                    break;
            }
        }

        return user;
    }

    private string ApplyFieldError(string fieldValue, int errorType)
    {
        if (string.IsNullOrEmpty(fieldValue))
        {
            return fieldValue;
        }

        int position;

        List<char> alphabet = GetAlphabet();

        switch (errorType)
        {
            case 0: // Delete character
                if (fieldValue.Length > 1)
                {
                    position = _random.Next(fieldValue.Length);
                    fieldValue = fieldValue.Remove(position, 1);
                }
                break;

            case 1: // Add random character
                position = _random.Next(fieldValue.Length + 1);
                char randomChar = alphabet[_random.Next(alphabet.Count)];
                fieldValue = fieldValue.Insert(position, randomChar.ToString());
                break;

            case 2: // Swap adjacent characters
                if (fieldValue.Length > 1)
                {
                    position = _random.Next(fieldValue.Length - 1);
                    char[] chars = fieldValue.ToCharArray();
                    char temp = chars[position];
                    chars[position] = chars[position + 1];
                    chars[position + 1] = temp;
                    fieldValue = new string(chars);
                }
                break;
        }

        return fieldValue;
    }


    private List<char> GetAlphabet()
    {
        List<char> commonAlphabet = new List<char>
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
        'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
    };

        // Expanded alphabet for "pl" (Polish)
        List<char> polishAlphabet = new List<char>
    {
        'a', 'ą', 'b', 'c', 'ć', 'd', 'e', 'ę', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'ł',
        'm', 'n', 'ń', 'o', 'ó', 'p', 'r', 's', 'ś', 't', 'u', 'w', 'y', 'z', 'ź', 'ż',
        'A', 'Ą', 'B', 'C', 'Ć', 'D', 'E', 'Ę', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'Ł',
        'M', 'N', 'Ń', 'O', 'Ó', 'P', 'R', 'S', 'Ś', 'T', 'U', 'W', 'Y', 'Z', 'Ź', 'Ż'
    };

        switch (_locale)
        {
            case "pl":
                return polishAlphabet;
            case "en_US":
            case "es_MX":
            default:
                return commonAlphabet;
        }
    }




    private int GenerateRandomPage(int seed, int pageNumber)
{
    if (pageNumber <= 0)
    {
        throw new ArgumentException("Page number must be greater than zero.", nameof(pageNumber));
    }

    int randomSeed = (seed * 31 + pageNumber) % int.MaxValue;

    return Math.Abs(randomSeed);
}
}