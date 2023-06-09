﻿namespace CourierKata
{
    public class DeliveryCostCalculator
    {
        private const decimal SmallSizeCost = 3m;
        private const decimal MediumSizeCost = 8m;
        private const decimal LargeSizeCost = 15m;
        private const decimal XLSizeCost = 25m;
        private const decimal HeavySizeCost = 50m;
        private const int SmallWeightLimit = 1;
        private const int MediumWeightLimit = 3;
        private const int LargeWeightLimit = 6;
        private const int XLWeightLimit = 10;
        private const decimal HeavyWeightLimit = 50;

        public Delivery CalculateDeliveryCost(Delivery delivery)
        {
            if (delivery.Parcels != null)
            {
                foreach (var parcel in delivery.Parcels)
                {
                    parcel.ParcelCost = CalculateParcelCost(parcel);

                    delivery.TotalCost += parcel.ParcelCost;
                }

                IsDiscountedShipping(delivery);

                if (delivery.DiscountedShipping)
                {
                    CalculateShippingDiscounts(delivery);
                }
            }

            if (delivery.SpeedyShipping)
            {
                CalculateSpeedyShippingParcelCost(delivery);
            }

            return delivery;
        }
        private static decimal CalculateParcelCost(Parcel parcel)
        {
            parcel.ParcelType = DefineParcelType(parcel);

            parcel.ParcelCost = GetParcelCost(parcel);

            if (IsParcelOverweight(parcel))
            {
                //TODO: weight limit for XL parcels

                parcel.OverweightCost = (parcel.ParcelWeight - parcel.ParcelWeightLimit) * 2m;

                parcel.ParcelCost += parcel.OverweightCost;
            }

            return parcel.ParcelCost;
        }

        private static ParcelType DefineParcelType(Parcel parcel)
        {
            if (parcel.ParcelHeight <= 0 || parcel.ParcelHeight <= 0 || parcel.ParcelDepth <= 0)
            {
                throw new ArgumentException("Invalid parcel size");
            }

            if (parcel.ParcelWeight >= 50m)
            {
                return ParcelType.Heavy;
            }

            if (parcel.ParcelHeight < 10 && parcel.ParcelHeight < 10 && parcel.ParcelDepth < 10)
            {
                return ParcelType.Small;
            }
            else if (parcel.ParcelHeight < 50 && parcel.ParcelWidth < 50 && parcel.ParcelDepth < 50)
            {
                return ParcelType.Medium;
            }
            else if (parcel.ParcelHeight < 100 && parcel.ParcelWidth < 100 && parcel.ParcelDepth < 100)
            {
                return ParcelType.Large;
            }
            else if (parcel.ParcelHeight >= 100 || parcel.ParcelWidth >= 100 || parcel.ParcelDepth >= 100)
            {
                return ParcelType.XL;
            }

            throw new ArgumentException("Invalid parcel size");
        }

        private static decimal GetParcelCost(Parcel parcel)
        {
            switch (parcel.ParcelType)
            {
                case ParcelType.Small:
                    return SmallSizeCost;

                case ParcelType.Medium:
                    return MediumSizeCost;

                case ParcelType.Large:
                    return LargeSizeCost;

                case ParcelType.XL:
                    return XLSizeCost;

                case ParcelType.Heavy:
                    return HeavySizeCost;

                default:
                    throw new ArgumentException("Invalid parcel type");
            }
        }
        private static decimal CalculateSpeedyShippingParcelCost(Delivery delivery)
        {
            delivery.SpeedyShippingCost = delivery.TotalCost;

            delivery.TotalCost += delivery.SpeedyShippingCost;

            return delivery.TotalCost;
        }

        private static bool IsParcelOverweight(Parcel parcel)
        {
            parcel.ParcelWeightLimit = GetWeightLimit(parcel.ParcelType);

            parcel.IsOverweight = parcel.ParcelWeight > parcel.ParcelWeightLimit;

            return parcel.IsOverweight;
        }

        private static decimal GetWeightLimit(ParcelType ParcelType)
        {
            switch (ParcelType)
            {
                case ParcelType.Small:
                    return SmallWeightLimit;

                case ParcelType.Medium:
                    return MediumWeightLimit;

                case ParcelType.Large:
                    return LargeWeightLimit;

                case ParcelType.XL:
                    return XLWeightLimit;

                case ParcelType.Heavy:
                    return HeavyWeightLimit;

                default:
                    throw new Exception("Invalid parcel type");
            }
        }

        private static decimal CalculateShippingDiscounts(Delivery delivery)
        {
            IsDiscountedShipping(delivery);

            if (delivery.DiscountedShipping)
            {
                var discountedSmallParcelnumber = 0;
                var discountedMediumParcelnumber = 0;
                var discountedMixedParcelnumber = 0;
                var discountSmallParcel = 0m;
                var discountMediumParcel = 0m;
                var discountMixedParcel = 0m;

                if (delivery.Parcels.Where(p => p.ParcelType == ParcelType.Small).ToList().Count > 3)
                {
                    discountedSmallParcelnumber = delivery.Parcels.Count / 4;

                    discountSmallParcel = -delivery.Parcels.OrderBy(x => x.ParcelCost).Take(discountedSmallParcelnumber).Sum(y => y.ParcelCost);

                    delivery.ShippingDiscounts += discountSmallParcel;
                }
                if (delivery.Parcels.Where(p => p.ParcelType == ParcelType.Medium).ToList().Count > 2)
                {
                    discountedMediumParcelnumber = delivery.Parcels.Count / 3;

                    discountMediumParcel = -delivery.Parcels.OrderBy(x => x.ParcelCost).Take(discountedMediumParcelnumber).Sum(y => y.ParcelCost);

                    delivery.ShippingDiscounts += discountMediumParcel;
                }
                if (delivery.Parcels.Count > 4)
                {
                    discountedMixedParcelnumber = (delivery.Parcels.Count - (discountedSmallParcelnumber * 4 + discountedMediumParcelnumber * 3)) / 5;

                    if (discountedMixedParcelnumber > 0)
                    {
                        discountMixedParcel = -delivery.Parcels.OrderBy(x => x.ParcelCost).Take(discountedMixedParcelnumber).Sum(y => y.ParcelCost);
                    }

                    delivery.ShippingDiscounts += discountMixedParcel;
                }
            }
            return delivery.TotalCost = delivery.TotalCost + delivery.ShippingDiscounts;
        }

        private static bool IsDiscountedShipping(Delivery delivery)
        {
            if (delivery.Parcels.Count > 3)
            {
                return delivery.DiscountedShipping = true;

            }

            return delivery.DiscountedShipping = false;
        }
    }
}
