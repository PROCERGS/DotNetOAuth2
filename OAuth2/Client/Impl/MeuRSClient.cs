using Newtonsoft.Json.Linq;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace OAuth2.Client.Impl
{
    /// <summary>
    /// MeuRS authentication client.
    /// </summary>
    public class MeuRSClient : OAuth2Client
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeuRSClient"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="configuration">The configuration.</param>
        public MeuRSClient(IRequestFactory factory, IClientConfiguration configuration)
            : base(factory, configuration)
        {
        }

        /// <summary>
        /// Defines URI of service which issues access code.
        /// </summary>
        protected override Endpoint AccessCodeServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://meu.rs.gov.br",
					Resource = "/openid/connect/authorize"
                };
            }
        }

        /// <summary>
        /// Defines URI of service which issues access token.
        /// </summary>
        protected override Endpoint AccessTokenServiceEndpoint
        {
            get
            {
                return new Endpoint
				{
					BaseUri = "https://meu.rs.gov.br",
					Resource = "/openid/connect/token"
                };
            }
        }

        /// <summary>
        /// Defines URI of service which allows to obtain information about user which is currently logged in.
        /// </summary>
        protected override Endpoint UserInfoServiceEndpoint
        {
            get
            {
                return new Endpoint
				{
					BaseUri = "https://meu.rs.gov.br",
                    Resource = "/api/v1/person.json"
                };
            }
        }

        /// <summary>
        /// Friendly name of provider (OAuth2 service).
        /// </summary>
        public override string Name
        {
            get { return "MeuRS"; }
        }


        /// <summary>
        /// Should return parsed <see cref="UserInfo"/> from content received from third-party service.
        /// </summary>
        /// <param name="content">The content which is received from third-party service.</param>
        protected override UserInfo ParseUserInfo(string content)
        {
            var response = JObject.Parse(content);
            var avatarUri = response["picture"].SafeGet(x => x.Value<string>());
            const string avatarUriTemplate = "{0}?sz={1}";
            return new UserInfo
            {
                Id = response["sub"].Value<string>(),
                Email = response["email"].Value<string>(),
                FirstName = response["first_name"].Value<string>(),
                LastName = response["given_name"].Value<string>(),
                AvatarUri =
                    {
                        Small = avatarUri,
                        Normal = avatarUri,
                        Large = avatarUri
                    }
            };
        }

        protected override void BeforeGetUserInfo(BeforeAfterRequestArgs args)
        {
            args.Client.Authenticator = null;
            args.Request.Parameters.Add(new Parameter
            {
                Name  = "access_token",
                Type  = ParameterType.GetOrPost,
                Value = AccessToken
            });
        }
    }
}