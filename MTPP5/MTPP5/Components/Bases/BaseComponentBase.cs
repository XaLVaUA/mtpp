using Microsoft.AspNetCore.Components;

namespace BlazorAuth.Components.Bases
{
    public abstract class BaseComponentBase : ComponentBase
    {
        private Func<Task> _onParametersSetAsync = () => Task.CompletedTask;

        protected override sealed async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await OnComponentInitializedAsync();
            _onParametersSetAsync = TrueOnParametersSetAsync;
        }

        protected override sealed Task OnParametersSetAsync() => _onParametersSetAsync();

        protected virtual Task OnComponentInitializedAsync() => Task.CompletedTask;

        protected virtual Task OnComponentParametersSetAsync() => Task.CompletedTask;

        private async Task TrueOnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await OnComponentParametersSetAsync();
        }
    }
}
