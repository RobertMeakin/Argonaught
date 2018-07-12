namespace Argonaut.Model
{
    public class RefreshResponse
    {
        public RefreshResponse(IRefreshToken refreshToken, IAudience audience)
        {
            this.RefreshToken = refreshToken;
            this.Audience = audience;
        }

        public IRefreshToken RefreshToken { get; private set; }

        public IAudience Audience { get; private set; }

    }
}