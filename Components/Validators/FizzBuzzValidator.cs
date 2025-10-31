using FizzBuzzBlazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FizzBuzzBlazor.Components.Validators
{
    // Non visual component, needs to inherit from ComponentBase in order to be used in razor files
    public class FizzBuzzValidator : ComponentBase
    {
        // Enables validation message
        private ValidationMessageStore validationMessageStore;

        [CascadingParameter]
        private EditContext CurrentEditContext { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(FizzBuzzValidator)} requires a cascading parameter of type {nameof(EditContext)}. For example, you can use {nameof(FizzBuzzValidator)} inside an EditForm");
            }

            validationMessageStore = new(CurrentEditContext);

            // Attach methods to validation events
            CurrentEditContext.OnFieldChanged += (sender, eventArgs) => ValidateField(eventArgs.FieldIdentifier);
            CurrentEditContext.OnValidationRequested += (sender, eventArgs) => ValidateAllFields();
        }

        private void ValidateField(FieldIdentifier fieldIdentifier)
        {
            var fizzBuzz = CurrentEditContext.Model as FizzBuzzModel ?? throw new InvalidOperationException($"{nameof(FizzBuzzValidator)} requires the model to be of type {nameof(FizzBuzzModel)}");

            // Clear previous errors for the field
            validationMessageStore.Clear(fieldIdentifier);

            // Validate the field
            if (fieldIdentifier.FieldName == nameof(FizzBuzzModel.FizzValue))
            {
                if (fizzBuzz.FizzValue >= fizzBuzz.BuzzValue)
                {
                    validationMessageStore.Add(fieldIdentifier, "The Fizz value must be less than Buzz value");
                }
            }
            else if (fieldIdentifier.FieldName == nameof(FizzBuzzModel.BuzzValue))
            {
                if (fizzBuzz.BuzzValue <= fizzBuzz.FizzValue)
                {
                    validationMessageStore.Add(fieldIdentifier, "The Buzz value must be greater than Fizz value");
                }
            }
            else if (fieldIdentifier.FieldName == nameof(FizzBuzzModel.StopValue))
            {
                int requiredMinimumStopValue = fizzBuzz.FizzValue * fizzBuzz.BuzzValue;

                if (fizzBuzz.StopValue < requiredMinimumStopValue)
                {
                    validationMessageStore.Add(fieldIdentifier, $"The Stop value must be greater than or equal to {requiredMinimumStopValue}");
                }
            }
        }

        private void ValidateAllFields()
        {
            // Clear all previous errors
            validationMessageStore.Clear();

            var fizzBuzz = CurrentEditContext.Model as FizzBuzzModel ?? throw new InvalidOperationException($"{nameof(FizzBuzzValidator)} requires the model to be of type {nameof(FizzBuzzModel)}");

            // Validate all fields
            ValidateField(new FieldIdentifier(fizzBuzz, nameof(FizzBuzzModel.FizzValue)));
            ValidateField(new FieldIdentifier(fizzBuzz, nameof(FizzBuzzModel.BuzzValue)));
            ValidateField(new FieldIdentifier(fizzBuzz, nameof(FizzBuzzModel.StopValue)));

            // Notify the EditContext that validation messages have changed
            CurrentEditContext.NotifyValidationStateChanged();
        }
    }
}