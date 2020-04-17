// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly FlightBookingRecognizer _luisRecognizer;
        protected readonly ILogger Logger;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(FlightBookingRecognizer luisRecognizer, BookingDialog bookingDialog, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(bookingDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
               // IntroStepAsync,
                ActStepAsync
                //FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? "What can I help you with today?\nSay something like \"Book a flight from Paris to Berlin on March 22, 2020\"";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(BookingDialog), new BookingDetails(), cancellationToken);
            }

            // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var luisResult = await _luisRecognizer.RecognizeAsync<FlightBooking>(stepContext.Context, cancellationToken);
          
            switch (luisResult.TopIntent().intent)
            {
                
                case FlightBooking.Intent.BookFlight:
                    await ShowWarningForUnsupportedCities(stepContext.Context, luisResult, cancellationToken);

                    // Initialize BookingDetails with any entities we may have found in the response.
                    var bookingDetails = new BookingDetails()
                    {
                        // Get destination and origin from the composite entities arrays.
                        Destination = luisResult.ToEntities.Airport,
                        Origin = luisResult.FromEntities.Airport,
                        TravelDate = luisResult.TravelDate,
                    };

                    // Run the BookingDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
                    return await stepContext.BeginDialogAsync(nameof(BookingDialog), bookingDetails, cancellationToken);

                case FlightBooking.Intent.Greeting:
                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    await stepContext.Context.SendActivityAsync("Hi, this is Jessica. I will be your Coach chatbot to help you augment your skills to upsell insurance offerings for Edge-Protect");
                    break;

                case FlightBooking.Intent.Stepone:
                    string actualAns = "Thanks Jessica. I too am interested in learning. Shall we start?";
                    string userAns = luisResult.Text;
                    Request rb = new Request();
                    rb.requestbody(actualAns, userAns);
                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    await stepContext.Context.SendActivityAsync("Let’s Start.");
                    await Task.Delay(4000);
                 
                    await stepContext.Context.SendActivityAsync("I shall be playing a role of customer who is trying to buy a home loan insurance.I would like you to attempt to bundle auto insurance with prospective purchase of Home insurance."
                      + "Basically, with this scenario, I plan to coach you to upsell our products & services.");

                    break;

                case FlightBooking.Intent.Steptwo:
                    //Request rb = new Request();
                    string actualAns1 = "Thank you for interacting with edge protect insurance. I’m Tami and I will be your customer service representative.  Who am I speaking to?";
                    string userAns1 = luisResult.Text;
                    Request rb1 = new Request();
                    rb1.requestbody(actualAns1, userAns1);

                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    await stepContext.Context.SendActivityAsync("Hi I’m Jessica");

                    break;
                case FlightBooking.Intent.Stepthree:
                    string actualAns3 = "Hi Jessica.  Hope you are doing good. It’s a pleasure to serve you. Please may I know if you are an existing customer?";
                    string userAns3 = luisResult.Text;
                    Request rb3 = new Request();
                    rb3.requestbody(actualAns3, userAns3);


                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    await stepContext.Context.SendActivityAsync("No, I don’t have any existing relationship with Edge-Protect. ");

                    break;

                case FlightBooking.Intent.Stepfour:
                    string actualAns4 = "Oh OK! no problem.  How may I help you, today?";
                    string userAns4 = luisResult.Text;
                    Request rb4 = new Request();
                    rb4.requestbody(actualAns4, userAns4);
                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    await stepContext.Context.SendActivityAsync("I’m interested in getting an insurance quote for my home.");

                    break;

                case FlightBooking.Intent.Stepfive:
                    string actualAns5 = "Wonderful.  I’d be happy to help.  Jessica, while I find those rates build the quotation for you, Please may I ask who provides your auto insurance?";
                    string userAns5 = luisResult.Text;
                    Request rb5 = new Request();
                    rb5.requestbody(actualAns5, userAns5);
                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    await stepContext.Context.SendActivityAsync("Another company?  Why?");

                    break;

                case FlightBooking.Intent.Stepsix:
                    string actualAns6 = "Well, we provide auto insurance to over 10 million drivers each year and have saved them an average of around $150 annually by pooling their home and auto insurance.  Would you be interested to explore upto 20% by pooling your auto and home insurance?";
                    string userAns6 = luisResult.Text;
                    Request rb6 = new Request();
                    rb6.requestbody(actualAns6, userAns6);
                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    await stepContext.Context.SendActivityAsync("I don’t know.  I just don’t want to be locked into a year-long commitment.");

                    break;
                case FlightBooking.Intent.Stepseven:
                    
                    string actualAns7 = "Jessica, I totally understand! That’s why we offer a “no strings attached” deal.  Cancel anytime and we’ll refund the remainder of your policy.";
                    string userAns7 = luisResult.Text;
                    Request rb7 = new Request();
                    rb7.requestbody(actualAns7, userAns7);

                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    await stepContext.Context.SendActivityAsync("I don’t know.  I don’t have the time to cancel my current auto insurance.");

                    break;
                case FlightBooking.Intent.Stepeight:
                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.

                    string actualAns8 = "That’s why we will call your current insurance company and cancel for you.  What do you think?  Can I show you how much you can save by bundling?";
                    string userAns8 = luisResult.Text;
                    Request rb8 = new Request();
                    rb8.requestbody(actualAns8, userAns8);


                    await stepContext.Context.SendActivityAsync("How long will it take?");

                    break;
                case FlightBooking.Intent.Stepnine:

                    string actualAns9 = "Jessica, it will only take an extra 10 minutes.  Can I go ahead and show you how much you can save by bundling?";
                    string userAns9 = luisResult.Text;
                    Request rb9 = new Request();
                    rb9.requestbody(actualAns9, userAns9);

                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    await stepContext.Context.SendActivityAsync("Okay.  I just want to see how much I can save.  I don’t want to sign up today.");

                    break;

                case FlightBooking.Intent.Stepten:

                    string actualAns10 = "Of course.You could save upto 25% on your insurance premiums";
                    string userAns10 = luisResult.Text;
                    Request rb10 = new Request();
                    rb10.requestbody(actualAns10, userAns10);



                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    await stepContext.Context.SendActivityAsync("Okay. I’m ready to Purchase home insurance now, can I purchase auto policy at a later date?");

                    break;
                case FlightBooking.Intent.Stepeleven:

                    string actualAns11 = "Jessica, it is just a phone call away. We can call your insurance company and cancel the auto policy for you and within 24 hours auto policy gets attached to our company.";
                    string userAns11 = luisResult.Text;
                    Request rb11 = new Request();
                    rb11.requestbody(actualAns11, userAns11);


                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    await stepContext.Context.SendActivityAsync("Okay. What details do you need?");

                    break;

                case FlightBooking.Intent.Steptwelve:
                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.

                    string actualAns12 = "Great. Just provide your Zip code along with your phone number. Our Authorized Agent will reach-out to you and help you with the paper- work.";
                    string userAns12 = luisResult.Text;
                    Request rb12 = new Request();
                    rb12.requestbody(actualAns12, userAns12);


                    await stepContext.Context.SendActivityAsync("Zip Code: OH 43206 ");
                    await Task.Delay(2000);
                    await stepContext.Context.SendActivityAsync("Phone Number: +1-999-999-999");

                    break;

                case FlightBooking.Intent.Stepthirteen:
                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                   
                    await Task.Delay(2000);
                    await stepContext.Context.SendActivityAsync("The text-based simulation chat session has ended. Please click on the ‘Evaluate’ button below to get your score. ");
                    await Task.Delay(2000);
                    HeroCard greeting = new HeroCard()
                    {
                        Text = "Please click the Evaluate button.",
                        Buttons = new List<CardAction>
                         {
                            new CardAction(ActionTypes.PostBack, "Evaluate", value: "Looking for Home Insurance"),
                         }
                    };

                    var greetingreply = MessageFactory.Attachment(greeting.ToAttachment());
                    await stepContext.Context.SendActivityAsync(greetingreply, cancellationToken);
                    break;

                case FlightBooking.Intent.Stepfourteen:

                    await stepContext.Context.SendActivityAsync("We are evaluating your scores.Please wait for few seconds");
                    DBAccess db = new DBAccess();
                    int sessionid = db.selectSessionID();

                    var ressult=db.avgScore(sessionid);
                    var ressult1 = db.avgtechScore(sessionid);

                    string Coresco = "The Average value for soft score  " + ressult + "";
                    string techsco = "The Average value for tech score  " + ressult1 + "";
                    Dictionary<string, int> Mydict =new Dictionary<string, int>();
                    Mydict.Add("Core_Score", Convert.ToInt32(ressult));
                    Mydict.Add("Tech_Score", Convert.ToInt32(ressult1));
                    Result product = new Result();
                    product.vale = Convert.ToInt32(ressult);
                    product.vale1 = Convert.ToInt32(ressult1);
                    string json = JsonConvert.SerializeObject(product);
                    //string vaal = ressult.value.ToString();
                    await Task.Delay(3000);
                    await stepContext.Context.SendActivityAsync(Coresco);
                    await Task.Delay(3000);
                    await stepContext.Context.SendActivityAsync(techsco);
                    await Task.Delay(3000);
                    await stepContext.Context.SendActivityAsync("Thanks for using PrepTalk chat!");
                    break;
                case FlightBooking.Intent.Sendoff:
                    await stepContext.Context.SendActivityAsync("Thanks for using PrepTalk chat!");
                    break;
                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Oops! I didn't get that. Please try asking in a different way ";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        // Shows a warning if the requested From or To cities are recognized as entities but they are not in the Airport entity list.
        // In some cases LUIS will recognize the From and To composite entities as a valid cities but the From and To Airport values
        // will be empty if those entity values can't be mapped to a canonical item in the Airport.
        private static async Task ShowWarningForUnsupportedCities(ITurnContext context, FlightBooking luisResult, CancellationToken cancellationToken)
        {
            var unsupportedCities = new List<string>();

            var fromEntities = luisResult.FromEntities;
            if (!string.IsNullOrEmpty(fromEntities.From) && string.IsNullOrEmpty(fromEntities.Airport))
            {
                unsupportedCities.Add(fromEntities.From);
            }

            var toEntities = luisResult.ToEntities;
            if (!string.IsNullOrEmpty(toEntities.To) && string.IsNullOrEmpty(toEntities.Airport))
            {
                unsupportedCities.Add(toEntities.To);
            }

            if (unsupportedCities.Any())
            {
                var messageText = $"Sorry but the following airports are not supported: {string.Join(',', unsupportedCities)}";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await context.SendActivityAsync(message, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If the child dialog ("BookingDialog") was cancelled, the user failed to confirm or if the intent wasn't BookFlight
            // the Result here will be null.
            if (stepContext.Result is BookingDetails result)
            {
                // Now we have all the booking details call the booking service.

                // If the call to the booking service was successful tell the user.

                var timeProperty = new TimexProperty(result.TravelDate);
                var travelDateMsg = timeProperty.ToNaturalLanguage(DateTime.Now);
                var messageText = $"I have you booked to {result.Destination} from {result.Origin} on {travelDateMsg}";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            // Restart the main dialog with a different message the second time around
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }

     


    }
    public class Result
    {
        public int vale { get; set; }
        public int vale1 { get; set; }
    }
}
