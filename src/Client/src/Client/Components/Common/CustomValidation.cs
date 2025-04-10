using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace Showmatics.Blazor.Client.Components.Common;

// See https://docs.microsoft.com/en-us/aspnet/core/blazor/forms-validation?view=aspnetcore-6.0#server-validation-with-a-validator-component
public class CustomValidation : ComponentBase
{
    private ValidationMessageStore? _messageStore;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    protected override void OnInitialized()
    {
        if (CurrentEditContext is null)
        {
            throw new InvalidOperationException(
                $"{nameof(CustomValidation)} requires a cascading " +
                $"parameter of type {nameof(EditContext)}. " +
                $"For example, you can use {nameof(CustomValidation)} " +
                $"inside an {nameof(EditForm)}.");
        }

        _messageStore = new(CurrentEditContext);

        CurrentEditContext.OnValidationRequested += (s, e) =>
            _messageStore?.Clear();
        CurrentEditContext.OnFieldChanged += (s, e) =>
            _messageStore?.Clear(e.FieldIdentifier);
    }

    public void DisplayErrors(IDictionary<string, ICollection<string>> errors)
    {
        if (CurrentEditContext is not null && errors is not null)
        {
            foreach (var err in errors)
            {
                _messageStore?.Add(CurrentEditContext.Field(err.Key), err.Value);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    public bool Validate(object request)
    {
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(request, new ValidationContext(request), results, true))
        {
            // Convert results to errors
            var errors = new Dictionary<string, ICollection<string>>();
            foreach (var result in results
                .Where(r => !string.IsNullOrWhiteSpace(r.ErrorMessage)))
            {
                foreach (string field in result.MemberNames)
                {
                    if (errors.ContainsKey(field))
                    {
                        errors[field].Add(result.ErrorMessage!);
                    }
                    else
                    {
                        errors.Add(field, new List<string>() { result.ErrorMessage! });
                    }
                }
            }

            DisplayErrors(errors);

            return false;
        }

        return true;
    }

    public void ClearErrors()
    {
        _messageStore?.Clear();
        CurrentEditContext?.NotifyValidationStateChanged();
    }
}