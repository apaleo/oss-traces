using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Traces.Common.Constants;
using Traces.Web.Services;
using Traces.Web.Services.Apaleo;

namespace Traces.Web
{
    public partial class App
    {
        private readonly Theme _theme = new Theme
        {
            ColorOptions = new ThemeColorOptions
            {
                Primary = StyleConstants.PrimaryColor
            }
        };

        [Parameter]
        public string AccessToken { get; set; }

        [Inject]
        private IApaleoUserClaimValidatorService UserClaimValidatorService { get; set; }

        [Inject]
        private ITokenStorageService TokenStorageService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            TokenStorageService.AccessToken = AccessToken;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await UserClaimValidatorService.AssertClaimAsync(AppConstants.AccountCodeQueryParameter, ApaleoClaims.AccountCode);
            await UserClaimValidatorService.AssertClaimAsync(AppConstants.SubjectIdQueryParameter, ApaleoClaims.SubjectId);

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
