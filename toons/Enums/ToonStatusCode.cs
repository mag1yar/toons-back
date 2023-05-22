using System.ComponentModel;

namespace toons.Enums
{
    public enum ToonStatusCode
    {
        None = 0,

        AuthError = 100,
        AuthSignUp = 101,
        AuthSignIn = 102,

        EmailError = 200,
    }
}
