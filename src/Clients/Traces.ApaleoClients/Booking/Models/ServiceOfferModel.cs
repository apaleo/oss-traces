// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Traces.ApaleoClients.Booking.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class ServiceOfferModel
    {
        /// <summary>
        /// Initializes a new instance of the ServiceOfferModel class.
        /// </summary>
        public ServiceOfferModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ServiceOfferModel class.
        /// </summary>
        /// <param name="count">The default count of offered services. For
        /// services whose pricing unit is 'Person' it will be based on the
        /// adults and children specified, otherwise 1.</param>
        /// <param name="dates">The dates the service will be delivered with
        /// its price</param>
        /// <param name="prePaymentAmount">The amount that needs to be
        /// pre-paid.</param>
        /// <param name="service">The service</param>
        /// <param name="totalAmount">The total price</param>
        public ServiceOfferModel(int count, IList<ServiceOfferItemModel> dates, MonetaryValueModel prePaymentAmount, ServiceModel service, AmountModel totalAmount)
        {
            Count = count;
            Dates = dates;
            PrePaymentAmount = prePaymentAmount;
            Service = service;
            TotalAmount = totalAmount;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the default count of offered services. For services
        /// whose pricing unit is 'Person' it will be based on the adults and
        /// children specified, otherwise 1.
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the dates the service will be delivered with its price
        /// </summary>
        [JsonProperty(PropertyName = "dates")]
        public IList<ServiceOfferItemModel> Dates { get; set; }

        /// <summary>
        /// Gets or sets the amount that needs to be pre-paid.
        /// </summary>
        [JsonProperty(PropertyName = "prePaymentAmount")]
        public MonetaryValueModel PrePaymentAmount { get; set; }

        /// <summary>
        /// Gets or sets the service
        /// </summary>
        [JsonProperty(PropertyName = "service")]
        public ServiceModel Service { get; set; }

        /// <summary>
        /// Gets the total price
        /// </summary>
        [JsonProperty(PropertyName = "totalAmount")]
        public AmountModel TotalAmount { get; private set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Dates == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Dates");
            }
            if (PrePaymentAmount == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "PrePaymentAmount");
            }
            if (Service == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Service");
            }
            if (Dates != null)
            {
                foreach (var element in Dates)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
            if (PrePaymentAmount != null)
            {
                PrePaymentAmount.Validate();
            }
            if (Service != null)
            {
                Service.Validate();
            }
            if (TotalAmount != null)
            {
                TotalAmount.Validate();
            }
        }
    }
}
