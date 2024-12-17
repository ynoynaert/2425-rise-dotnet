namespace Rise.Client.Auth;

public class Auth0ErrorMapper
{
	public static string GetTranslatedErrorMessage(string errorMessage)
	{
		if (errorMessage.Contains("The user already exists"))
		{
			return "Deze gebruiker bestaat al.";
		}
		else if (errorMessage.Contains("didn't pass validation for format email"))
		{
			return "Het emailadres is niet correct opgesteld (naam@domein.extensie).";
		}
		else if (errorMessage.Contains("phone_number is not valid"))
		{
			return "Het telefoonnummer is niet correct opgesteld.";
		}
		else if (errorMessage.Contains("phone_number already exists"))
		{
			return "Dit telefoonnummer is reeds in gebruik.";
		}
		return errorMessage;
	}
}
